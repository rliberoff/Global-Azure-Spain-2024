using System.Reflection;

using Encamina.Enmarcha.DependencyInjection;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace GlobalAzureSpain.Demo.Bot.Dialogs.Greetings;

[AutoRegisterService(ServiceLifetime.Singleton, typeof(Dialog))]
internal sealed class GreetingsDialog : Dialog
{
    private readonly IMessageActivity greetingsMessageActivity;

    public GreetingsDialog()
    {
        greetingsMessageActivity = MessageFactory.Attachment(new HeroCard()
        {
            Images = new List<CardImage>()
            {
                new CardImage()
                {
                    Url = LoadGreetingsImageFromAssemblyEmbeddedResource(),
                },
            },
            Title = @"¡Hola, desde la Global Azure Spain 2024 en Madrid!",
            Subtitle = @"MICROAGENTES O LAS ARQUITECTURA EN TIEMPOS DE LA IA",
            Text = @"Desde la Global Azure Spain 2024 en Madrid, os queremos mostrar como usando Semantic Kernel, Plugins de IA y los Planners podemos conseguir orquestar acciones para crear agentes que permitan atender a cuestiones complejas que nos podrían solicitar los usuarios a partir de varias fuentes de datos, usando por ejemplo el patrón RAG (Retrieval-Augmented Generation).",
        }.ToAttachment());
    }

    /// <inheritdoc/>
    public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
    {
        var result = await dc.Context.SendActivityAsync(greetingsMessageActivity, cancellationToken);

        return await dc.EndDialogAsync(result, cancellationToken);
    }

    private static string LoadGreetingsImageFromAssemblyEmbeddedResource()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(@"GlobalAzureSpain.Demo.Bot.Resources.GlobalAzureSpain.png");
        var count = (int)stream!.Length;
        var data = new byte[count];
        stream.Read(data, 0, count);
        return $@"data:image/png;base64,{Convert.ToBase64String(data)}";
    }
}
