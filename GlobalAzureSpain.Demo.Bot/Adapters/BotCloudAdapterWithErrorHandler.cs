using Encamina.Enmarcha.Bot.Abstractions.Adapters;
using Encamina.Enmarcha.Bot.Adapters;

using Microsoft.Bot.Builder;

namespace GlobalAzureSpain.Demo.Bot.Adapters;

internal sealed class BotCloudAdapterWithErrorHandler : BotCloudAdapterWithErrorHandlerBase
{
    public BotCloudAdapterWithErrorHandler(IBotAdapterOptions<BotCloudAdapterWithErrorHandlerBase> adapterOptions) : base(adapterOptions)
    {
        InitializeDefaultMiddlewares();
    }

    protected override Task ErrorHandlerAsync(ITurnContext turnContext, Exception exception) => base.ErrorHandlerAsync(turnContext, exception);
}