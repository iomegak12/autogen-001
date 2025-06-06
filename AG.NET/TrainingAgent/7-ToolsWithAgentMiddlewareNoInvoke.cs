using System.ComponentModel;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FluentAssertions;

namespace TrainingAgent;

public static class ToolsWithAgentsMiddlewareNoInvoke
{
    public static async Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();

        var systemPrompt = @"You're a helpful assistant that can perform various tasks using tools.
            Use Only the tools provided to you.
            You can use the tools to perform tasks like:
                - UpperCase: Convert text to uppercase, Use this tool to convert any text to uppercase especially English Text.
                - UpperCaseEx: Convert text to uppercase, Use this tool to convert any text to uppercase especially Non English Text. 
                - Concatenate two strings
                - Calculate tax on a given amount with a tax rate
            You will be provided with the tools and their descriptions.
            Do NOT use any other tools or external APIs.";
        var tools = new Tools();
        var toolsCallMiddleware = new FunctionCallMiddleware(
            functions:
            [
                tools.UpperCaseFunctionContract,
                tools.ConcatFunctionContract,
                tools.CalculateTaxFunctionContract
            ]
        );

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "ToolsAgent",
            temperature: 1.0f,
            systemMessage: systemPrompt
        )
            .RegisterMessageConnector()
            .RegisterStreamingMiddleware(toolsCallMiddleware)
            .RegisterPrintMessage();

        var reply = await agent.SendAsync("can you capitalize the following text: hello world");

        Console.WriteLine($"Agent Reply: {reply.GetContent()}");

        // reply.GetContent()?.Should().Be("HELLO WORLD");
        // reply.Should().BeOfType<ToolCallAggregateMessage>();
        // reply.GetToolCalls().First().FunctionName.Should().Be("UpperCase");

        var calculateTaxReply = await agent.SendAsync(
            "can you calculate the tax on 1000 with a tax rate of 5%?; calculate tax 2000, 20%"
        );

        Console.WriteLine($"Agent Reply: {calculateTaxReply.GetContent()}");

        // calculateTaxReply.GetToolCalls().Should().HaveCount(2);
        // calculateTaxReply.GetToolCalls().First().FunctionName.Should().Be("CalculateTax");
        // calculateTaxReply.Should().BeOfType<ToolCallAggregateMessage>();
    }
}