namespace TrainingAgent;

using System;
using Azure.AI.OpenAI;

public static class LLMConfiguration
{
    public static AzureOpenAIClient GetAzureOpenAIClient()
    {
        var apiKey = Utils.GetConfigValue("AzureOpenAI:ApiKey");
        var endpoint = Utils.GetConfigValue("AzureOpenAI:Endpoint");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
        {
            throw new InvalidOperationException("Azure OpenAI configuration is not properly set.");
        }

        var client = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey));

        return client;
    }

    public static string GetDeploymentName()
    {
        var deploymentName = Utils.GetConfigValue("AzureOpenAI:DeploymentName");

        if (string.IsNullOrEmpty(deploymentName))
        {
            throw new InvalidOperationException("Azure OpenAI deployment name is not set.");
        }

        return deploymentName;
    }
}