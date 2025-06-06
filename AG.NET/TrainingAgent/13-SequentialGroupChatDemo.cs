using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using AutoGen.SemanticKernel;
using AutoGen.SemanticKernel.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;

#pragma warning disable SKEXP0050 // Missing XML comment for publicly visible type or member

namespace TrainingAgent;

public static class SequentialGroupChatDemo
{
    public static async Task<IAgent> CreateBraveSearchAgentAsync()
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
        var systemPrompt = @"You are a helpful agent that can search the web using Brave Search.
            You put the original search results between ```brave and ```

            e.g.
            ```brave
            <search results>
            ```";

        var kernelAgent = new SemanticKernelAgent(
            kernel: kernel,
            name: "BraveSearchSKPluginInAutoGenAgentDemo",
            systemMessage: systemPrompt
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        return kernelAgent;
    }

    public static async Task<IAgent> CreateSummarizeAgentAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();
        var systemPrompt = @"You're a helpful assistant.
            You summarize the content of the brave web search results and return the summary in a single paragraph.
            Your summary should be concise and to the point.";

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "SummarizeAgent",
            temperature: 1.0f,
            systemMessage: systemPrompt
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        return agent;
    }

    public static async Task RunAsync()
    {
        var userProxyAgent = new UserProxyAgent(
            name: "User",
            humanInputMode: HumanInputMode.ALWAYS)
            .RegisterPrintMessage();

        var braveSearchAgent = await CreateBraveSearchAgentAsync();
        var summarizeAgent = await CreateSummarizeAgentAsync();
        var agents = new IAgent[] { userProxyAgent, braveSearchAgent, summarizeAgent };

        var groupChat = new RoundRobinGroupChat(agents);
        var groupChatManager = new GroupChatManager(groupChat);

        var history = await userProxyAgent.InitiateChatAsync(
            receiver: groupChatManager,
            message: "How do you deploy OpenAI Resources in Azure?",
            maxRound: 10);

        Console.WriteLine("Group Chat History:");

        foreach (var chat in history)
        {
            Console.WriteLine($"{chat.GetContent()}: {chat.From}");
        }
    }
}