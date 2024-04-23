using Encamina.Enmarcha.AI.OpenAI.Azure;

using GlobalAzureSpain.Agents.Demo;
using GlobalAzureSpain.Agents.Demo.Agents;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = CreateHostBuilder().Build();
using var scope = host.Services.CreateScope();

try
{
    using var cancellationTokenSource = new CancellationTokenSource();
    var cancellationToken = cancellationTokenSource.Token;

    Utils.WriteLine("\n Press any key to start «Agents Delegation Demo»...", ConsoleColor.DarkYellow);
    Utils.ReadKey();

    await scope.ServiceProvider.GetRequiredService<AgentsDelegationDemo>().RunAsync(cancellationToken);

    Utils.WriteLine("\n Press any key to start «Agents Collaboration Demo»...", ConsoleColor.DarkYellow);
    Utils.ReadKey();

    await scope.ServiceProvider.GetRequiredService<AgentsCollaborationDemo>().RunAsync(cancellationToken);

    Utils.WriteLine("\n Press any key to end...", ConsoleColor.DarkYellow);
    Utils.ReadKey();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

static IHostBuilder CreateHostBuilder()
{
    return Host.CreateDefaultBuilder()
               .ConfigureAppConfiguration((_, configuration) =>
               {
                   configuration.SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                                .AddEnvironmentVariables();
               })
               .ConfigureServices((host, services) =>
               {
                   services.AddOptionsWithValidateOnStart<AzureOpenAIOptions>(Constants.Demo.Collaboration).Bind(host.Configuration.GetSection($@"{nameof(AzureOpenAIOptions)}-{Constants.Demo.Collaboration}")).ValidateDataAnnotations();
                   services.AddOptionsWithValidateOnStart<AzureOpenAIOptions>(Constants.Demo.Delegation).Bind(host.Configuration.GetSection($@"{nameof(AzureOpenAIOptions)}-{Constants.Demo.Delegation}")).ValidateDataAnnotations();

                   services.AddKernel();

                   // Register each agent services as «Transient» to match the Kernel’s registration scope.
                   services.AddTransient<AgentsCollaborationDemo>()
                           .AddTransient<AgentsDelegationDemo>()
                           ;
               });
}
