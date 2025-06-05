namespace TrainingAgent;

using System;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FluentAssertions;

public static class TwoAgentsTalkingDemo
{
    public async static Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();
        var client = LLMConfiguration.GetAzureOpenAIClient();

        var systemPrompt = @"You are a teacher who creates mid-school random math questions for students and check answers.
            If the answer is correct, you will say Correct and 
            you stop the conversations by saying COMPLETE. 
            If the answer is wrong, you will say 'Wrong' and ask the student to fix it.";

        var teacher = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "SuperTeacher",
            temperature: 1.0f,
            systemMessage: systemPrompt
        )
            .RegisterMessageConnector()
            .RegisterMiddleware(
                async (messages, option, agent, _) =>
                {
                    var reply = await agent.GenerateReplyAsync(messages, option);

                    if (reply.GetContent()?.ToLower().Contains("complete") is true)
                    {
                        Console.WriteLine("Teacher has completed the conversation.");
                        
                        return new TextMessage(
                            Role.Assistant,
                            GroupChatExtension.TERMINATE,
                            from: reply.From
                        );
                    }

                    return reply;
                })
            .RegisterPrintMessage();

        var student = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "SuperStudent",
            temperature: 1.0f,
            systemMessage: "You are a student that answers math questions from the teacher."
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        var conversation = await student.InitiateChatAsync(
            receiver: teacher,
            message: @"Hello Teacher!, Please create math questions for me.",
            maxRound: 10
        );

        conversation.Count().Should().BeLessThan(10);
        conversation.Last().IsGroupChatTerminateMessage().Should().BeTrue();
    }
}