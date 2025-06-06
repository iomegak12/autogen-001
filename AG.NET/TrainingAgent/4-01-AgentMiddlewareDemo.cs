namespace TrainingAgent;

using System;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FluentAssertions;
using OpenAI.Chat;

public static class AgentMiddlewareDemo
{
    public async static Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();
        var totalTokenCount = 0;

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "AzureOpenAIAgent",
            systemMessage: "You are a helpful assistant."
        )
            .RegisterMiddleware(
                async (messages, option, innerAgent, ct) =>
                {
                    var reply = await innerAgent.GenerateReplyAsync(messages, option, ct);

                    if (reply is MessageEnvelope<ChatCompletion> chatCompletions)
                    {
                        var tokenCount = chatCompletions.Content.Usage.TotalTokenCount;

                        totalTokenCount += tokenCount;
                    }

                    return reply;
                }
            )
            .RegisterMiddleware(new OpenAIChatRequestMessageConnector());

        var reply = await agent.SendAsync("Tell me a serious joke about Joker!");

        Console.WriteLine($"Reply: {reply.GetContent()}");
        Console.WriteLine($"Total Token Count: {totalTokenCount}");

        reply.Should().BeOfType<TextMessage>();
        totalTokenCount.Should().BeGreaterThan(0);
    }
}