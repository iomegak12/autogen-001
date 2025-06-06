namespace TrainingAgent;

using System;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Azure.AI.OpenAI;

public class ConnectWithAzureOpenAIEx
{
    public static async Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "Azure OpenAI Agent",
            systemMessage: "You are a helpful assistant that can answer questions and provide information based on the user's input."
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        await agent.SendAsync("Can you write a piece of C# code that calculates 100th Fibonacci number?");
    }
}