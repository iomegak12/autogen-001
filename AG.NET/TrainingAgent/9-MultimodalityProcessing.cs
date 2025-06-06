using System.ComponentModel;
using System.Text.Json;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FluentAssertions;

namespace TrainingAgent;

public static class MultimodalityProcessing
{
    public static async Task RunAsync()
    {
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
            .RegisterPrintMessage();

        var imagePath = Path.Combine(
            Directory.GetCurrentDirectory(), "resource", "images", "background.jpg");

        var imagePath1 = Path.Combine(
            Directory.GetCurrentDirectory(), "resource", "images", "USMortgageRate.png");

        Console.WriteLine($"Processing image: {imagePath}");

        var imageBytes = await File.ReadAllBytesAsync(imagePath);
        var imageBytes1 = await File.ReadAllBytesAsync(imagePath1);
        var imageMessage = new ImageMessage(Role.User,
            BinaryData.FromBytes(imageBytes, "image/jpeg"));
        var imageMessage1 = new ImageMessage(Role.User,
            BinaryData.FromBytes(imageBytes1, "image/jpeg"));

        var textMessage = new TextMessage(Role.User, "Describe this image.");
        var multimodalMessage = new MultiModalMessage(Role.User, [
            textMessage,
            imageMessage
        ]);

        var multimodalMessage1 = new MultiModalMessage(Role.User, [
            textMessage,
            imageMessage1
        ]);

        var reply = await agent.SendAsync(multimodalMessage);
        var reply1 = await agent.SendAsync(multimodalMessage1);

        reply.Should().NotBeNull();
        reply.Should().BeOfType<TextMessage>();

        Console.WriteLine("Multimodal processing completed successfully.");
        Console.WriteLine($"Reply: {reply.GetContent()}");
        Console.WriteLine($"Reply: {reply1.GetContent()}");
    }
}