namespace GlobalAzureSpain.Demo.Services.FunCatalog.Models;

public sealed record FunInfo
{
    public required string Name { get; init; }

    public required decimal Price { get; init; }

    public required int Rating { get; init; }

    public required string Location { get; init; }
}
