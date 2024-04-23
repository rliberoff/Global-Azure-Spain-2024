#pragma warning disable SKEXP0101 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Encamina.Enmarcha.AI.OpenAI.Azure;

using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace GlobalAzureSpain.Agents.Demo.Agents;

internal class AgentsDelegationDemo : BaseAgentsDemo
{
    public AgentsDelegationDemo(Kernel kernel, IOptionsSnapshot<AzureOpenAIOptions> azureOpenAIOptions)
        : base(kernel, azureOpenAIOptions.Get(Constants.Demo.Delegation))
    {
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        var codingPlugin = Kernel.ImportPluginFromPromptDirectory(Path.Combine(Directory.GetCurrentDirectory(), Constants.PluginsDirectory, Constants.Plugins.CodingPlugin), Constants.Plugins.CodingPlugin);
        var childrensBookPlugin = Kernel.ImportPluginFromPromptDirectory(Path.Combine(Directory.GetCurrentDirectory(), Constants.PluginsDirectory, Constants.Plugins.ChildrensBookPlugin), Constants.Plugins.ChildrensBookPlugin);

        var agentCoder = TrackAgent(await new AgentBuilder()
                                                .WithAzureOpenAIChatCompletion(AzureOpenAIOptions.Endpoint.AbsoluteUri, AzureOpenAIOptions.ChatModelDeploymentName, AzureOpenAIOptions.Key)
                                                .WithName(@"CoderAgent")
                                                .WithDescription(@"Creates code from natural language")
                                                .WithPlugin(codingPlugin)
                                                .BuildAsync(cancellationToken));

        var agentChildBookWritter = TrackAgent(await new AgentBuilder()
                                                .WithAzureOpenAIChatCompletion(AzureOpenAIOptions.Endpoint.AbsoluteUri, AzureOpenAIOptions.ChatModelDeploymentName, AzureOpenAIOptions.Key)
                                                .WithName(@"ChildBookWritterAgent")
                                                .WithDescription(@"Creates stories for children's books")
                                                .WithPlugin(childrensBookPlugin)
                                                .BuildAsync(cancellationToken));

        var agentDelegator = TrackAgent(await new AgentBuilder()
                                                .WithAzureOpenAIChatCompletion(AzureOpenAIOptions.Endpoint.AbsoluteUri, AzureOpenAIOptions.ChatModelDeploymentName, AzureOpenAIOptions.Key)
                                                .FromTemplatePath(Path.Combine(Directory.GetCurrentDirectory(), Constants.AgentTemplatesDirectory, @"ToolAgent.yaml"))
                                                .WithPlugin(agentCoder.AsPlugin())
                                                .WithPlugin(agentChildBookWritter.AsPlugin())
                                                .BuildAsync(cancellationToken));

        var messages = new string[]
        {
                @"Create a hello world in C#",
                @"Create a story about a fox",
                @"Create a hello world in Python",
                @"Crea una historia sobre un gato que sabe programar",
                @"With C# and using Entity Framework Core, how can I query a table called Entities by ID.",
                @"¿De qué sabor son las galaxias?",
        };

        IAgentThread? thread = null;

        try
        {
            thread = await agentDelegator.NewThreadAsync(cancellationToken);

            Utils.WriteLine(@"=====================================", ColorTrace);

            foreach (var message in messages)
            {
                await foreach (var responseMessage in thread.InvokeAsync(agentDelegator, message, cancellationToken: cancellationToken))
                {
                    Utils.WriteLine($"{responseMessage.Role} : {responseMessage.Content}", responseMessage.Role.Equals(@"user", StringComparison.OrdinalIgnoreCase) ? ColorUser : ColorAssistant);
                }

                Utils.WriteLine(@"=====================================", ColorTrace);
            }
        }
        finally
        {
            await Task.WhenAll(thread?.DeleteAsync(cancellationToken) ?? Task.CompletedTask, Task.WhenAll(TrackedAgents.Select(a => a.DeleteAsync())));
        }
    }
}

#pragma warning restore SKEXP0101 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
