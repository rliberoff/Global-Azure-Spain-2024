#pragma warning disable SKEXP0101 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Encamina.Enmarcha.AI.OpenAI.Azure;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace GlobalAzureSpain.Agents.Demo.Agents;

internal abstract class BaseAgentsDemo
{
    protected const ConsoleColor ColorTrace = ConsoleColor.DarkGray;
    protected const ConsoleColor ColorUser = ConsoleColor.Green;
    protected const ConsoleColor ColorAssistant = ConsoleColor.Cyan;
    protected const ConsoleColor ColorAction = ConsoleColor.DarkRed;

    protected BaseAgentsDemo(Kernel kernel, AzureOpenAIOptions azureOpenAIOptions)
    {
        Kernel = kernel;
        AzureOpenAIOptions = azureOpenAIOptions;
    }

    protected AzureOpenAIOptions AzureOpenAIOptions { get; init; }

    protected Kernel Kernel { get; init; }

    protected List<IAgent> TrackedAgents { get; } = [];

    public abstract Task RunAsync(CancellationToken cancellationToken);

    protected IAgent TrackAgent(IAgent agent)
    {
        TrackedAgents.Add(agent);

        return agent;
    }
}

#pragma warning restore SKEXP0101 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
