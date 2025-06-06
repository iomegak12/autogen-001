namespace TrainingAgent;

using AutoGen.Core;
using AutoGen.SemanticKernel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

public static class SimpleSemanticKernelDemo
{
    public static async Task RunAsync()
    {
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                Utils.GetConfigValue("AzureOpenAI:DeploymentName"),
                Utils.GetConfigValue("AzureOpenAI:Endpoint"),
                Utils.GetConfigValue("AzureOpenAI:ApiKey"))
            .Build();

        var chatAgent = new ChatCompletionAgent()
        {
            Kernel = kernel,
            Name = "ChatAgent",
            Description = "A simple chat agent that can answer questions."
        };

        var skAgent = new SemanticKernelChatCompletionAgent(chatAgent)
            .RegisterMiddleware(new SemanticKernelChatMessageContentConnector())
            .RegisterPrintMessage();

        var reply = await skAgent.SendAsync("What is the capital of France?");

        Console.WriteLine($"Agent Reply: {reply}");
    }
}