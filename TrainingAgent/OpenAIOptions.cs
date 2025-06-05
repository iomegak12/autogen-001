#pragma warning disable IDE0073
// The code that's violating the rule is on this line.
#pragma warning restore IDE0073
public class OpenAIOptions
{
    public const string OpenAI = "OpenAI";

    public string Source { get; set; } = string.Empty;
    public string ChatModelId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ChatDeploymentName { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
}
