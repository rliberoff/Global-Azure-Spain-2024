namespace GlobalAzureSpain.Demo.Services.HotelsCatalog.Models;

public sealed record HotelInfo
{
    public required string Name { get; init; }
    
    public required DateOnly AvailableFromDate { get; init; }

    public required DateOnly AvailableToDate { get; init; }

    public required decimal Price { get; init; }

    public required int Rating { get; init; }

    public required string Location { get; init; }
}
