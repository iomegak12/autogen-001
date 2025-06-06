namespace TrainingAgent;

using System;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FluentAssertions;

public static class AssistantAgentDemo
{
    public async static Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "AzureOpenAIAgent",
            systemMessage: "You are a helpful assistant. You convert what user said in uppercase."
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        var reply = await agent.SendAsync("Hello World");

        reply.Should().BeOfType<TextMessage>();
        reply.GetContent().Should().Be("HELLO WORLD");

        var conversationHistory = new List<IMessage>
        {
            new TextMessage(Role.User, "Hello World"),
            reply
        };

        reply = await agent.SendAsync("Hello World Again", conversationHistory);

        reply.Should().BeOfType<TextMessage>();
        reply.GetContent().Should().Be("HELLO WORLD AGAIN");
    }
}