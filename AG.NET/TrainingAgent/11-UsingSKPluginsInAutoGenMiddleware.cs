using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using AutoGen.SemanticKernel;
using Google.Protobuf.Compiler;
using Microsoft.SemanticKernel;

namespace TrainingAgent;

public static class SKPluginsInAutoGenMiddleware
{
    public static async Task RunAsync()
    {
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                Utils.GetConfigValue("AzureOpenAI:DeploymentName"),
                Utils.GetConfigValue("AzureOpenAI:Endpoint"),
                Utils.GetConfigValue("AzureOpenAI:ApiKey"))
            .Build();

        var getWeatherFunction = KernelFunctionFactory.CreateFromMethod(
            method: (string location) =>
            {
                return Task.FromResult($"The weather in {location} is sunny with a high of 25°C.");
            },
            functionName: "GetWeather",
            description: "A simple plugin to get the weather for a given location."
        );

        var getWeatherPlugin = kernel.CreatePluginFromFunctions("my_plugin", [getWeatherFunction]);
        var kernelPluginMiddleware = new KernelPluginMiddleware(kernel, getWeatherPlugin);

        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();
        var systemPrompt = @"You're a helpful assistant.";

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "ToolsAgent",
            temperature: 1.0f,
            systemMessage: systemPrompt
        )
        .RegisterMessageConnector()
        .RegisterMiddleware(kernelPluginMiddleware)
        .RegisterPrintMessage();

        var toolAggregateMessage = await agent.SendAsync(
            new TextMessage(Role.User, "What is the weather in Seattle?"));

        Console.WriteLine("SK Plugins in AutoGen Middleware completed successfully.");
        Console.WriteLine($"Final Reply: {toolAggregateMessage.GetContent()}");
    }
}