from autogen_agentchat.agents import AssistantAgent, UserProxyAgent
from autogen_agentchat.conditions import MaxMessageTermination, TextMentionTermination
from autogen_agentchat.messages import AgentEvent, ChatMessage
from autogen_agentchat.teams import SelectorGroupChat
from autogen_agentchat.base import TaskResult
from autogen_ext.models.openai import AzureOpenAIChatCompletionClient, OpenAIChatCompletionClient
from autogen_agentchat.ui import Console
from autogen_agentchat.conditions import TextMentionTermination

import os
from dotenv import load_dotenv
import asyncio

from tools import find_closest_product, \
    product_inquiry_tool, order_placement_tool, order_status_tool, complaint_registration_tool

load_dotenv(override=True)

model_client = OpenAIChatCompletionClient(
    model="gpt-4o",
    api_key=os.getenv("OPENAI_API_KEY"),
)

planning_agent = AssistantAgent(
    "PlanningAgent",
    model_client=model_client,
    description="Planning agent for product inquiries, order placements, order status and complaint registrations.",
    system_message="""
        You are a planning agent.
        Your job is to identify customer requests and delegate them to the correct agent.
        Available agents:
            - ProductInquiryAgent: Handles product-related questions
            - OrderPlacementAgent: Handles order placement
            - OrderInquiryAgent: Checks order status
            - ComplaintAgent: Registers customer complaints
            - ResponseAgent: Responsible for responding to the user.

        Once a task is completed by another agent, delegate the response to the **ResponseAgent** to format and send the final reply to the user.
        
        Format task assignments as:
        1. <agent> : <task>



        Your job is to format the response provided by other agents and return it to the user in a clear and friendly manner.
        Always end the conversation with 'TERMINATE'.
        
        Example:
        - If an order is placed, confirm it and include the order ID.
        - If a product inquiry is made, summarize the details.
        - If an order status is checked, summarize the response.
        - If a complaint is registered, confirm it with the complaint ID.	
    """,
)

product_inquiry_agent = AssistantAgent(
    "ProductInquiryAgent",
    model_client=model_client,
    description="Handles product-related questions.",
    system_message="""
        You are a product inquiry agent.
        Your job is to answer product-related questions using the product catalog.
        If the product is not found, return a friendly message indicating that the product is not available.
        Always use the `product_inquiry_tool` to check product information.
    """,
    tools=[
        product_inquiry_tool,
    ]
)

order_placement_agent = AssistantAgent(
    "OrderPlacementAgent",
    model_client=model_client,
    description="Handles order placement.",
    system_message="""
        You are an order placement agent.
        Your job is to place orders for products.
        Use the `order_placement_tool` to place orders.
        If the product is not available, return a friendly message indicating that the product is not available.
        If the quantity is not provided, return a message asking for the quantity.
    """,
    tools=[
        order_placement_tool,
    ]
)

order_inquiry_agent = AssistantAgent(
    "OrderInquiryAgent",
    model_client=model_client,
    description="Handles order status inquiries.",
    system_message="""
        You are an order inquiry agent.
        Your job is to check the status of orders.
        Use the `order_status_tool` to check order status.
        If the order ID is invalid, return a friendly message indicating that the order ID is invalid.
    """,
    tools=[
        order_status_tool,
    ]
)

complaint_agent = AssistantAgent(
    "ComplaintAgent",
    model_client=model_client,
    description="Handles customer complaints.",
    system_message="""
        You are a complaint agent.
        Your job is to register customer complaints.
        Use the `complaint_registration_tool` to register complaints.
        If the order ID is invalid, return a friendly message indicating that the order ID is invalid.
    """,
    tools=[
        complaint_registration_tool,
    ]
)

response_agent = AssistantAgent(
    "ResponseAgent",
    model_client=model_client,
    description="Responsible for responding to the user.",
    system_message="""
        Your job is to format the response provided by other agents and return it to the user in a clear and friendly manner.
        Always end the conversation with 'TERMINATE'.
        Example:
        - If an order is placed, confirm it and include the order ID.
        - If a product inquiry is made, summarize the details.
        - If an order status is checked, summarize the response.
        - If a complaint is registered, confirm it with the complaint ID.
    """,
)

text_mention_termination = TextMentionTermination("TERMINATE")
max_messages_termination = MaxMessageTermination(max_messages=10)
termination = text_mention_termination | max_messages_termination

team = SelectorGroupChat(
    [
        planning_agent,
        product_inquiry_agent,
        order_placement_agent,
        order_inquiry_agent,
        complaint_agent,
        response_agent
    ],
    model_client=model_client,
    termination_condition=termination,
    allow_repeated_speaker=True
)


async def main(query):
    response = await team.run(task=query)
      
    result = response.messages[-1].content

    if result.endswith("TERMINATE"):
        result = result.removesuffix("TERMINATE").strip()

    return result

if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser(
        description="Run the planning agent team.")
    parser.add_argument("query", type=str, help="The query to process.")
    args = parser.parse_args()

    query = args.query

    loop = asyncio.get_event_loop()
    result = loop.run_until_complete(main(query))
    
    print(result)
