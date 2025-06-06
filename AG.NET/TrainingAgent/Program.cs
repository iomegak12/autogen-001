using TrainingAgent;

public static class MainClass
{
    public static async Task Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Select an option to run:");
            Console.WriteLine("1. ConnectWithAzureOpenAIEx");
            Console.WriteLine("2. AssistantAgentDemo");
            Console.WriteLine("3. AgentMiddlewareDemo");
            Console.WriteLine("5. AgentMiddlewareDemoRAG");
            Console.WriteLine("6. TwoAgentsTalkingDemo");
            Console.WriteLine("7. ToolsWithAgentsMiddleware");
            Console.WriteLine("8. ToolsWithAgentsMiddlewareNoInvoke");
            Console.WriteLine("9. StructuralOutput");
            Console.WriteLine("10. MultimodalityProcessing");
            Console.WriteLine("11. SimpleSemanticKernelDemo");
            Console.WriteLine("12. SKPluginsInAutoGenMiddleware");
            Console.WriteLine("13. BraveSearchSKPluginInAutoGenAgentDemo");
            Console.WriteLine("14. SequentialGroupChatDemo");
            Console.WriteLine("15. ReActAgentsDemo");

            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice (1-7): ");

            var input = Console.ReadLine();

            try
            {
                switch (input)
                {
                    case "1":
                        await ConnectWithAzureOpenAIEx.RunAsync();
                        break;
                    case "2":
                        await AssistantAgentDemo.RunAsync();
                        break;
                    case "3":
                        await AgentMiddlewareDemo.RunAsync();
                        break;
                    case "0":
                        Console.WriteLine("Exiting...");
                        return;
                    case "5":
                        await AgentMiddlewareDemoRAG.RunAsync();
                        break;
                    case "6":
                        await TwoAgentsTalkingDemo.RunAsync();
                        break;
                    case "7":
                        await ToolsWithAgentsMiddleware.RunAsync();
                        break;
                    case "8":
                        await ToolsWithAgentsMiddlewareNoInvoke.RunAsync();
                        break;
                    case "9":
                        await StructuralOutput.RunAsync();
                        break;
                    case "10":
                        await MultimodalityProcessing.RunAsync();
                        break;
                    case "11":
                        await SimpleSemanticKernelDemo.RunAsync();
                        break;
                    case "12":
                        await SKPluginsInAutoGenMiddleware.RunAsync();
                        break;
                    case "13":
                        await BraveSearchSKPluginInAutoGenAgentDemo.RunAsync();
                        break;
                    case "14":
                        await SequentialGroupChatDemo.RunAsync();
                        break;
                    case "15":
                        await ReActAgentsDemo.RunAsync();
                        break;
                    default:
                        Console.WriteLine("Invalid option selected.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine(); // Add a blank line before showing the menu again
        }
    }
}