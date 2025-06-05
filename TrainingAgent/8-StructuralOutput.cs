using System.ComponentModel;
using System.Text.Json;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FluentAssertions;
using Json.Schema;
using Json.Schema.Generation;

namespace TrainingAgent;

public static class StructuralOutput
{
    public static async Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();
        var systemPrompt = @"You're a helpful assistant.";

        var schamaBuilder = new JsonSchemaBuilder().FromType<Person>();
        var schema = schamaBuilder.Build();

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "ToolsAgent",
            temperature: 1.0f,
            systemMessage: systemPrompt
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        var prompt = new TextMessage(
            Role.User,
                     "My Name is Ramkumar, I am almost 50 years old, and I live in T Nagar, Chennai, India. " +
                     "I am a software engineer and I love to play cricket and football. " +
                     "I also like to travel and explore new places. And I have opted for Space Tourism.");

        var reply = await agent.GenerateReplyAsync(
            messages: [prompt],
            options: new GenerateReplyOptions
            {
                OutputSchema = schema
            });

        var person = JsonSerializer.Deserialize<Person>(reply.GetContent());

        Console.WriteLine($"Agent Reply: {person}");
    }
}