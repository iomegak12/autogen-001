namespace TrainingAgent;

using System;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Azure.AI.OpenAI;

public class ConnectWithAzureOpenAI
{
    public static async Task RunAsync()
    {
        var apiKey = Utils.GetConfigValue("AzureOpenAI:ApiKey");
        var endpoint = Utils.GetConfigValue("AzureOpenAI:Endpoint");
        var deploymentName = Utils.GetConfigValue("AzureOpenAI:DeploymentName");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(deploymentName))
        {
            Console.WriteLine("Please set the Azure OpenAI API key, endpoint, and deployment name in the configuration.");
            return;
        }

        var client = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey));

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