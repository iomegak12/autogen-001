using AutoGen.Core;
using AutoGen.SemanticKernel;
using AutoGen.SemanticKernel.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;

#pragma warning disable SKEXP0050 // Missing XML comment for publicly visible type or member

namespace TrainingAgent;

public static class BraveSearchSKPluginInAutoGenAgentDemo
{
    public static async Task RunAsync()
    {
        var config = Configuration.ConfigureAppSettings();
        var openAISettings = new OpenAIOptions();
        config.GetSection("OpenAI").Bind(openAISettings);

        var pluginSettings = new PluginOptions();
        config.GetSection("PluginConfig").Bind(pluginSettings);

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Warning);
            builder.AddConfiguration(config);
            builder.AddConsole();
        });

        var builder = Kernel.CreateBuilder();

        builder.Services.AddSingleton(loggerFactory);
        builder.AddChatCompletionService(openAISettings);
        builder.AddBraveConnector(pluginSettings, ApiLoggingLevel.ResponseAndRequest);
        builder.Plugins.AddFromType<WebSearchEnginePlugin>();

        var kernel = builder.Build();
        var skAgent = new SemanticKernelAgent(
            kernel: kernel,
            name: "BraveSearchSKPluginInAutoGenAgentDemo",
            systemMessage: "You are a helpful agent that can search the web using Brave Search."
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        var reply = await skAgent.SendAsync("Tell me about latest advancements in Solar Energy in the Year 2025. Summarize the result in a concise manner.");

        Console.WriteLine($"Agent Reply: {reply.GetContent()}");
    }
}