using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

namespace TrainingAgent;

public class CustomReActAgent : IAgent
{
    private readonly ChatClient _client;
    private FunctionContract[] tools;
    private readonly Dictionary<string, Func<string, Task<string>>> toolExecutors = new();
    private readonly IAgent reasoner;
    private readonly IAgent actor;
    private readonly IAgent helper;
    private readonly int maxSteps = 10;

    private const string REACT_SYSTEM_PROMPT = @"
        You are a ReAct agent that can reason and act. 
        Answer the following questions as best you can.
        You can invoke the following tools:
        {tools}

        NOTE: You should only use the tools when necessary.
        You should always think about what to do next.

        Use the following format:

        Question: the input question you must answer
        Thought: you should always think about what to do
        Tool: the tool to invoke
        Tool Input: the input to the tool
        Observation: the invoke result of the tool
        ... (this process can repeat multiple times)

        Once you have the final answer, provide the final answer in the following format:
        Thought: I now know the final answer or weather report
        Final Answer: the final answer to the original input question

        Begin!
        Question: {input}";

    public string Name { get; }

    private string CreateReActPrompt(string input)
    {
        var toolPrompt =
            tools
                .Select(tool => $"{tool.Name}: {tool.Description}")
                .Aggregate((current, next) => $"{current}\n{next}");

        var prompt = REACT_SYSTEM_PROMPT
            .Replace("{tools}", toolPrompt);

        prompt = prompt.Replace("{input}", input);

        return prompt;
    }

    public CustomReActAgent(ChatClient client, string name, FunctionContract[] tools, Dictionary<string, Func<string, Task<string>>> toolExecutors)
    {
        _client = client;

        this.Name = name;
        this.tools = tools;
        this.toolExecutors = toolExecutors;

        this.reasoner = CreateReasoner();
        this.actor = CreateActor();
        this.helper = CreateHelper();
    }

    private IAgent CreateHelper()
    {
        return new OpenAIChatAgent(
            chatClient: _client,
            name: $"{Name}-Helper"
        )
            .RegisterMessageConnector();
    }

    private IAgent CreateReasoner()
    {
        return new OpenAIChatAgent(
            chatClient: _client,
            name: $"{Name}-Reasoner"
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();
    }

    private IAgent CreateActor()
    {
        var functionCallMiddleware = new FunctionCallMiddleware(tools, toolExecutors);

        return new OpenAIChatAgent(
            chatClient: _client,
            name: $"{Name}-Actor"
        )
            .RegisterMessageConnector()
            .RegisterStreamingMiddleware(functionCallMiddleware)
            .RegisterPrintMessage();
    }

    public async Task<IMessage> GenerateReplyAsync(IEnumerable<IMessage> messages,
        GenerateReplyOptions? options = null, CancellationToken cancellationToken = default)
    {
        // var userQuestion = await helper.SendAsync("Extract the question from the chat history", chatHistory: messages);

        // if (userQuestion.GetContent() is not string question)
        // {
        //     return new TextMessage(
        //         Role.Assistant,
        //         "I cannot extract the question from the chat history. Please provide a clear question.",
        //         from: Name);
        // }

        // var reactPrompt = CreateReActPrompt(question);
        // var promptMessage = new TextMessage(Role.User, reactPrompt, from: Name);
        // var chatHistory = new List<IMessage> { promptMessage };

        // for (var i = 0; i != this.maxSteps; i++)
        // {
        //     var reasoning = await reasoner.SendAsync(chatHistory: chatHistory);

        //     if (reasoning.GetContent() is not string reasoningContent)
        //     {
        //         return new TextMessage(
        //             Role.Assistant,
        //             "I could not find a reasoning in the chat history. Please provide a reasoning.",
        //             from: Name);
        //     }

        //     if (reasoningContent.Contains("the final answer", StringComparison.OrdinalIgnoreCase) ||
        //         reasoningContent.Contains("weather information", StringComparison.OrdinalIgnoreCase))
        //     {
        //         return new TextMessage(Role.Assistant, reasoningContent, from: Name);
        //     }

        //     chatHistory.Add(reasoning);

        //     var action = await actor.SendAsync(reasoning);

        //     chatHistory.Add(action);
        // }

        // var summary = await helper.SendAsync(
        //     @"Summarize the chat history and Find out what's missing.",
        //     chatHistory: chatHistory);

        // summary.From = Name;

        // return summary;

        // step 1: extract the input question
        var userQuestion = await helper.SendAsync("Extract the question from chat history", chatHistory: messages);
        if (userQuestion.GetContent() is not string question)
        {
            return new TextMessage(Role.Assistant, "I couldn't find a question in the chat history. Please ask a question.", from: Name);
        }
        var reactPrompt = CreateReActPrompt(question);
        var promptMessage = new TextMessage(Role.User, reactPrompt);
        var chatHistory = new List<IMessage>() { promptMessage };

        // step 2: ReAct
        for (int i = 0; i != this.maxSteps; i++)
        {
            // reasoning
            var reasoning = await reasoner.SendAsync(chatHistory: chatHistory);
            if (reasoning.GetContent() is not string reasoningContent)
            {
                return new TextMessage(Role.Assistant, "I couldn't find a reasoning in the chat history. Please provide a reasoning.", from: Name);
            }
            if (reasoningContent.Contains("I now know the final answer"))
            {
                return new TextMessage(Role.Assistant, reasoningContent, from: Name);
            }

            chatHistory.Add(reasoning);

            // action
            var action = await actor.SendAsync(reasoning);
            chatHistory.Add(action);
        }

        // fail to find the final answer
        // return the summary of the chat history
        var summary = await helper.SendAsync("Summarize the chat history and find out what's missing", chatHistory: chatHistory);
        summary.From = Name;

        return summary;
    }
}

public partial class ReActAgentTools
{
    [Function("GetWeatherReport",
        "Get the weather report for a specific city on a specific date.")]
    public async Task<string> GetWeatherReport(string city, string date)
    {
        return $"Weather report for {city} on {date}: Sunny with a high of 25°C and a low of 15°C.";
    }

    [Function("GetCurrentLocation",
        "Get the current location of the user.")]
    public async Task<string> GetCurrentLocation(string dummy)
    {
        return "Paris";
    }

    [Function("GetTodayDetails",
        "Get today's date and details.")]
    public async Task<string> GetTodayDetails(string dummy)
    {
        return $"Today's date is {DateTime.Now:MMMM dd, yyyy}. ";
    }
}


public static class ReActAgentsDemo
{
    public static async Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();
        var tools = new ReActAgentTools();
        var agent = new CustomReActAgent(
            client: client.GetChatClient(deploymentName),
            name: "CustomReActAgent",
            tools:
            [
                tools.GetCurrentLocationFunctionContract,
                tools.GetWeatherReportFunctionContract,
                tools.GetTodayDetailsFunctionContract
            ],
            toolExecutors: new Dictionary<string, Func<string, Task<string>>>
            {
                { tools.GetCurrentLocationFunctionContract.Name, tools.GetCurrentLocationWrapper },
                { tools.GetWeatherReportFunctionContract.Name, tools.GetWeatherReportWrapper },
                { tools.GetTodayDetailsFunctionContract.Name, tools.GetTodayDetailsWrapper }
            }
        )
            .RegisterPrintMessage();

        var message = new TextMessage(Role.User, "what's today date? what is the weather here?");

        var response = await agent.SendAsync(message);

        Console.WriteLine("ReAct Agent Response:");
        Console.WriteLine($"Response: {response.GetContent()}");
    }
}