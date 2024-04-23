#pragma warning disable SKEXP0101 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Encamina.Enmarcha.AI.OpenAI.Azure;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace GlobalAzureSpain.Agents.Demo.Agents;

internal class AgentsCollaborationDemo : BaseAgentsDemo
{
    private const string EoA = @"PRINT IT";

    public AgentsCollaborationDemo(Kernel kernel, IOptionsSnapshot<AzureOpenAIOptions> azureOpenAIOptions)
        : base(kernel, azureOpenAIOptions.Get(Constants.Demo.Collaboration))
    {
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            var agentCopywriter = TrackAgent(await new AgentBuilder()
                                                        .WithAzureOpenAIChatCompletion(AzureOpenAIOptions.Endpoint.AbsoluteUri, AzureOpenAIOptions.ChatModelDeploymentName, AzureOpenAIOptions.Key)
                                                        .WithInstructions(@"You are a copywriter with ten years of experience and are known for brevity and a dry humor. You're laser focused on the goal at hand. Don't waste time with chit chat. Your goal is to refine and decide on one single best copy as an expert in the field.  Just create one copy.  Consider suggestions when refining an idea.")
                                                        .WithName(@"Copywriter")
                                                        .WithDescription(@"Creates code from natural language")
                                                        .BuildAsync(cancellationToken));

            var agentCreativeDirector = TrackAgent(await new AgentBuilder()
                                                              .WithAzureOpenAIChatCompletion(AzureOpenAIOptions.Endpoint.AbsoluteUri, AzureOpenAIOptions.ChatModelDeploymentName, AzureOpenAIOptions.Key)
                                                              .WithInstructions($@"You are a creative director who has opinions about copywriting born of a love for David Ogilvy and Harrison McCann. Your goal is to determine if a given copy is acceptable to print, even if it isn't perfect.  If not, provide insight on how to refine suggested copy without example.  Always respond to the most recent message by evaluating and providing critique without example.  Always repeat the copy at the beginning.  If copy is acceptable and meets your criteria, say: {EoA}.")
                                                              .WithName(@"Creative Director")
                                                              .WithDescription($@"As an art director, evaluate and critique copy. If it’s good, say: {EoA}.")
                                                              .BuildAsync(cancellationToken));

            var agentCoordinator = TrackAgent(await new AgentBuilder()
                                                         .WithAzureOpenAIChatCompletion(AzureOpenAIOptions.Endpoint.AbsoluteUri, AzureOpenAIOptions.ChatModelDeploymentName, AzureOpenAIOptions.Key)
                                                         .WithInstructions($@"Reply the provided concept and have the Copywriter generate an marketing idea (copy) in the same language as the given concept.  Then have the Creative Director reply to the copywriter with a review of the copy.  Always include the source copy in any message.  Always include the Creative Director comments when interacting with the Copywriter.  Coordinate the repeated replies between the Copywriter and Creative Director until the Creative Director approves the copy saying {EoA}.")
                                                         .WithPlugin(agentCopywriter.AsPlugin())
                                                         .WithPlugin(agentCreativeDirector.AsPlugin())
                                                         .BuildAsync(cancellationToken));

            Utils.WriteLine("\nPress any key to start the demo showing how two agents are able to collaborate on a single thread...", ColorAction);
            Utils.ReadKey();

            await DemoSingleThreadCollaborationAsync(agentCopywriter, agentCreativeDirector, cancellationToken);

            Utils.WriteLine("\n Press any key to continue...", ColorAction);
            Utils.ReadKey();

            Utils.Write("\n=====================================\n", ColorTrace);

            Utils.WriteLine("\nPress any key to start the demo showing how two agents are able to collaborate using the plug-in model and not using a shared thread state for their interaction...", ColorAction);
            Utils.ReadKey();

            await DemoPluginCollaborationAsync(agentCoordinator, cancellationToken);

            Utils.WriteLine("\n Press any key to continue...", ColorAction);
            Utils.ReadKey();
        }
        finally
        {
            await Task.WhenAll(TrackedAgents.Select(a => a.DeleteAsync()));
        }
    }

    private static async Task DemoSingleThreadCollaborationAsync(IAgent agentCopywriter, IAgent agentArtDirector, CancellationToken cancellationToken)
    {
        IAgentThread? thread = null;

        try
        {
            thread = await agentCopywriter.NewThreadAsync(cancellationToken);
            ////var messageUser = await thread.AddUserMessageAsync(@"Concept: maps made out of egg cartons.", cancellationToken: cancellationToken);
            var messageUser = await thread.AddUserMessageAsync(@"Concepto: mapas hechos a partir de cartones de huevos.", cancellationToken: cancellationToken);

            DisplayMessage(messageUser);

            var isComplete = false;

            do
            {
                // Initiate copywriter input...
                var agentMessages = await thread.InvokeAsync(agentCopywriter, cancellationToken: cancellationToken).ToArrayAsync(cancellationToken);
                DisplayMessages(agentMessages, agentCopywriter);

                // Initiate art director input...
                agentMessages = await thread.InvokeAsync(agentArtDirector, cancellationToken: cancellationToken).ToArrayAsync(cancellationToken);
                DisplayMessages(agentMessages, agentArtDirector);

                // Evaluate if goal is met...
                isComplete = agentMessages[0].Content.Contains(EoA, StringComparison.OrdinalIgnoreCase);
            }
            while (!isComplete);
        }
        finally
        {
            if (thread is not null)
            {
                await thread.DeleteAsync(cancellationToken);
            }
        }
    }

    private static async Task DemoPluginCollaborationAsync(IAgent agentCoordinator, CancellationToken cancellationToken)
    {
        Utils.WriteLine("\n OK, this will take some time. Please be patient...", ColorAction);

        var response = await agentCoordinator.AsPlugin().InvokeAsync(@"Concept: maps made out of egg cartons.", cancellationToken);

        Utils.WriteLine(response, ColorAssistant);
    }

    private static void DisplayMessages(IEnumerable<IChatMessage> messages, IAgent? agent = null)
    {
        foreach (var message in messages)
        {
            DisplayMessage(message, agent);
        }
    }

    private static void DisplayMessage(IChatMessage message, IAgent? agent = null)
    {
        if (agent != null)
        {
            Utils.WriteLine($"\n # {message.Role}: ({agent.Name}) {message.Content}", ColorAssistant);
        }
        else
        {
            Utils.WriteLine($"\n # {message.Role}: {message.Content}", ColorUser);
        }
    }
}

#pragma warning restore SKEXP0101 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
