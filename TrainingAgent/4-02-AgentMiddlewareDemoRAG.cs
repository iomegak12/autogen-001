namespace TrainingAgent;

using System;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FluentAssertions;
using OpenAI.Chat;

public static class AgentMiddlewareDemoRAG
{
    public async static Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "AzureOpenAIAgent",
            systemMessage: "You are a helpful assistant."
        )
            .RegisterMessageConnector()
            .RegisterMiddleware(
                async (messages, option, innerAgent, ct) =>
                {
                    var today = DateTime.UtcNow;
                    var todayMessage = new TextMessage(
                        role: Role.System,
                        content: $"Today is {today:yyyy-MM-dd}."
                    );

                    messages = messages.Concat([todayMessage]);

                    return await innerAgent.GenerateReplyAsync(messages, option, ct);
                }
            )
            .RegisterPrintMessage();

        var reply = await agent.SendAsync("What is the date today?");

        Console.WriteLine($"Reply: {reply.GetContent()}");

        reply.Should().BeOfType<TextMessage>();
    }
}