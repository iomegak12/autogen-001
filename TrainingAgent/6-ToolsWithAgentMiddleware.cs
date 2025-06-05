using System.ComponentModel;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FluentAssertions;

namespace TrainingAgent;

public static class ToolsWithAgentsMiddleware
{
    public static async Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();

        var systemPrompt = @"You're a helpful assistant that can perform various tasks using tools.";
        var tools = new Tools();
        var toolsCallMiddleware = new FunctionCallMiddleware(
            functions:
            [
                tools.UpperCaseFunctionContract,
                tools.ConcatFunctionContract,
                tools.CalculateTaxFunctionContract
            ],
            functionMap: new Dictionary<string, Func<string, Task<string>>>
            {
                { nameof(tools.UpperCase), tools.UpperCaseWrapper },
                { nameof(tools.Concat), tools.ConcatWrapper },
                { nameof(tools.CalculateTax), tools.CalculateTaxWrapper }
            }
        );

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "ToolsAgent",
            temperature: 1.0f,
            systemMessage: systemPrompt
        )
            .RegisterMessageConnector()
            .RegisterStreamingMiddleware(toolsCallMiddleware)
            .RegisterPrintMessage();

        var reply = await agent.SendAsync("can you capitalize the following text: hello world");

        Console.WriteLine($"Agent Reply: {reply.GetContent()}");

        reply.GetContent()?.Should().Be("HELLO WORLD");
        reply.Should().BeOfType<ToolCallAggregateMessage>();
        reply.GetToolCalls().First().FunctionName.Should().Be("UpperCase");

        var calculateTaxReply = await agent.SendAsync(
            "can you calculate the tax on 1000 with a tax rate of 5%?; calculate tax 2000, 20%"
        );

        Console.WriteLine($"Agent Reply: {calculateTaxReply.GetContent()}");

        calculateTaxReply.GetToolCalls().Should().HaveCount(2);
        calculateTaxReply.GetToolCalls().First().FunctionName.Should().Be("CalculateTax");
        calculateTaxReply.Should().BeOfType<ToolCallAggregateMessage>();
    }
}