﻿namespace GlobalAzureSpain.Demo.Services.FlightsCatalog.Models;

public sealed record FlightInfo
{
    public DateOnly FromDate { get; init; }

    public DateOnly ToDate { get; init; }

    public decimal Price { get; init; }

    public required string FromAirport { get; init; }

    public required string ToAirport { get; init; }
}
