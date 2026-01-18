using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.EnergySpotPrice.Logic;

public interface IEnergySpotPriceService
{
    public Task ProcessEnergySpotPrice(CancellationToken cancellationToken);
}

public class EnergySpotPriceService(
    IOptions<EnergySpotPriceConfiguration> configuration,
    IEnergySpotPriceRepository energySpotPriceRepository,
    TimeProvider timeProvider,
    EnergySpotPriceHttpClient httpClient) : IEnergySpotPriceService
{
    private static readonly DateTimeOffset SpotPrice15MinutesTimeSlotBeginDate = DateTimeOffset.Parse("2025-09-01Z");

    private DateTimeOffset DeliveryStartMinStartTime => new(timeProvider.GetUtcNow().AddYears(-3).Date, TimeSpan.Zero); // API supports data from 3 years back
    private DateTimeOffset Tomorrow => new(timeProvider.GetUtcNow().AddDays(1).Date, TimeSpan.Zero);

    public async Task ProcessEnergySpotPrice(CancellationToken cancellationToken)
    {
        var config = configuration.Value;
        if (!config.Enabled)
        {
            return;
        }

        foreach (var deliveryArea in config.DeliveryAreas)
        {
            var mostRecent = await energySpotPriceRepository.GetMostRecentByDeliveryArea(deliveryArea, cancellationToken);
            if (mostRecent == null)
            {
                await Import(deliveryArea, DeliveryStartMinStartTime, cancellationToken);
                continue;
            }

            var fromDate = new DateTimeOffset(mostRecent.From.Date, TimeSpan.Zero);
            if (fromDate == Tomorrow)
            {
                continue; // Already imported
            }

            await Import(deliveryArea, fromDate, cancellationToken);
        }
    }

    private async Task Import(string deliveryArea, DateTimeOffset deliveryStart, CancellationToken cancellationToken)
    {
        const string timezone = "UTC";
        const string currency = "SEK";
        const string resolution60Minutes = "hourly";
        const string resolution15Minutes = "15mins";

        while (deliveryStart < Tomorrow)
        {
            var resolution = deliveryStart <= SpotPrice15MinutesTimeSlotBeginDate ? resolution60Minutes : resolution15Minutes;

            // Request data for full months
            deliveryStart = DateTimeOffset.Parse($"{deliveryStart.Year}-{deliveryStart.Month}-1Z");
            var deliveryEnd = deliveryStart.AddMonths(1);

            var start = deliveryStart.ToUnixTimeSeconds();
            var end = deliveryEnd.ToUnixTimeSeconds();
            var url = $"?deliveryAreas={deliveryArea}&currency={currency}&deliveryStart={start}&deliveryEnd={end}&resolution={resolution}&timezone={timezone}";
            var response = await httpClient.GetFromJsonAsync<SpotPriceResponse>(url, cancellationToken) ?? throw new Exception("Failed to get spot price response");

            var entries = MapSpotPriceResponse(response, deliveryArea, resolution == resolution15Minutes);
            await energySpotPriceRepository.AddOrUpdate(entries, cancellationToken);

            deliveryStart = deliveryEnd;
            await Task.Delay(1000, cancellationToken); // Avoid rate limiting
        }
    }

    private static List<Database.Repositories.EnergySpotPrice> MapSpotPriceResponse(SpotPriceResponse response, string deliveryArea, bool is15MinutesResolution)
    {
        var entries = new List<Database.Repositories.EnergySpotPrice>();
        foreach (var price in response.prices)
        {
            if (is15MinutesResolution)
            {
                entries.Add(new Database.Repositories.EnergySpotPrice
                (
                    deliveryArea,
                    From: new DateTimeOffset(price.year, price.month, price.day, price.hour, price.minute, 0, TimeSpan.Zero),
                    Price: price.measurement.value,
                    Unit: price.measurement.unit
                ));
            }
            else
            {
                // Convert hourly to 15 minutes entries
                for (var minute = 0; minute < 60; minute += 15)
                {
                    entries.Add(new Database.Repositories.EnergySpotPrice
                    (
                        deliveryArea,
                        From: new DateTimeOffset(price.year, price.month, price.day, price.hour, minute, 0, TimeSpan.Zero),
                        Price: price.measurement.value,
                        Unit: price.measurement.unit
                    ));
                }
            }
        }

        return entries;
    }
}

public record SpotPriceResponse(string resolution, List<SpotPriceEntry> prices);
public record SpotPriceEntry(int year, int month, int week, int day, int hour, int minute, Measurement measurement);
public record Measurement(Decimal value, string unit);

/*

{
  "currency": "SEK",
  "unit": "SWEDISH_ORE/KWH",
  "averagePrice": 101.70800000000001,
  "minPrice": 63.13400000000001,
  "maxPrice": 149.401,
  "resolution": "15mins",
  "timezone": "UTC",
  "prices": [
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 0,
      "minute": 0,
      "measurement": {
        "value": 73.519,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 0,
      "minute": 15,
      "measurement": {
        "value": 71.275,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 0,
      "minute": 30,
      "measurement": {
        "value": 67.527,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 0,
      "minute": 45,
      "measurement": {
        "value": 63.134,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 1,
      "minute": 0,
      "measurement": {
        "value": 69.374,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 1,
      "minute": 15,
      "measurement": {
        "value": 67.559,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 1,
      "minute": 30,
      "measurement": {
        "value": 67.011,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 1,
      "minute": 45,
      "measurement": {
        "value": 66.012,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 2,
      "minute": 0,
      "measurement": {
        "value": 69.76,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 2,
      "minute": 15,
      "measurement": {
        "value": 68.031,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 2,
      "minute": 30,
      "measurement": {
        "value": 68.676,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 2,
      "minute": 45,
      "measurement": {
        "value": 67.226,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 3,
      "minute": 0,
      "measurement": {
        "value": 68.321,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 3,
      "minute": 15,
      "measurement": {
        "value": 68.933,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 3,
      "minute": 30,
      "measurement": {
        "value": 67.462,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 3,
      "minute": 45,
      "measurement": {
        "value": 68.525,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 4,
      "minute": 0,
      "measurement": {
        "value": 67.387,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 4,
      "minute": 15,
      "measurement": {
        "value": 68.88,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 4,
      "minute": 30,
      "measurement": {
        "value": 71.446,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 4,
      "minute": 45,
      "measurement": {
        "value": 73.68,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 5,
      "minute": 0,
      "measurement": {
        "value": 69.213,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 5,
      "minute": 15,
      "measurement": {
        "value": 72.939,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 5,
      "minute": 30,
      "measurement": {
        "value": 76.182,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 5,
      "minute": 45,
      "measurement": {
        "value": 78.523,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 6,
      "minute": 0,
      "measurement": {
        "value": 74.464,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 6,
      "minute": 15,
      "measurement": {
        "value": 80.918,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 6,
      "minute": 30,
      "measurement": {
        "value": 84.881,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 6,
      "minute": 45,
      "measurement": {
        "value": 87.866,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 7,
      "minute": 0,
      "measurement": {
        "value": 85.858,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 7,
      "minute": 15,
      "measurement": {
        "value": 87.426,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 7,
      "minute": 30,
      "measurement": {
        "value": 91.786,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 7,
      "minute": 45,
      "measurement": {
        "value": 95.341,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 8,
      "minute": 0,
      "measurement": {
        "value": 94.428,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 8,
      "minute": 15,
      "measurement": {
        "value": 97.478,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 8,
      "minute": 30,
      "measurement": {
        "value": 97.8,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 8,
      "minute": 45,
      "measurement": {
        "value": 96.597,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 9,
      "minute": 0,
      "measurement": {
        "value": 95.577,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 9,
      "minute": 15,
      "measurement": {
        "value": 98.573,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 9,
      "minute": 30,
      "measurement": {
        "value": 104.866,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 9,
      "minute": 45,
      "measurement": {
        "value": 107.207,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 10,
      "minute": 0,
      "measurement": {
        "value": 98.67,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 10,
      "minute": 15,
      "measurement": {
        "value": 105.918,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 10,
      "minute": 30,
      "measurement": {
        "value": 107.798,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 10,
      "minute": 45,
      "measurement": {
        "value": 110.332,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 11,
      "minute": 0,
      "measurement": {
        "value": 104.555,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 11,
      "minute": 15,
      "measurement": {
        "value": 107.626,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 11,
      "minute": 30,
      "measurement": {
        "value": 104.265,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 11,
      "minute": 45,
      "measurement": {
        "value": 106.316,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 12,
      "minute": 0,
      "measurement": {
        "value": 105.532,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 12,
      "minute": 15,
      "measurement": {
        "value": 107.25,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 12,
      "minute": 30,
      "measurement": {
        "value": 108.635,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 12,
      "minute": 45,
      "measurement": {
        "value": 115.605,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 13,
      "minute": 0,
      "measurement": {
        "value": 109.366,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 13,
      "minute": 15,
      "measurement": {
        "value": 112.018,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 13,
      "minute": 30,
      "measurement": {
        "value": 117.13,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 13,
      "minute": 45,
      "measurement": {
        "value": 120.255,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 14,
      "minute": 0,
      "measurement": {
        "value": 111.836,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 14,
      "minute": 15,
      "measurement": {
        "value": 119.89,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 14,
      "minute": 30,
      "measurement": {
        "value": 121.63,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 14,
      "minute": 45,
      "measurement": {
        "value": 134.119,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 15,
      "minute": 0,
      "measurement": {
        "value": 125.646,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 15,
      "minute": 15,
      "measurement": {
        "value": 130.027,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 15,
      "minute": 30,
      "measurement": {
        "value": 128.61,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 15,
      "minute": 45,
      "measurement": {
        "value": 148.541,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 16,
      "minute": 0,
      "measurement": {
        "value": 139.435,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 16,
      "minute": 15,
      "measurement": {
        "value": 144.718,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 16,
      "minute": 30,
      "measurement": {
        "value": 147.843,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 16,
      "minute": 45,
      "measurement": {
        "value": 148.767,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 17,
      "minute": 0,
      "measurement": {
        "value": 149.401,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 17,
      "minute": 15,
      "measurement": {
        "value": 143.591,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 17,
      "minute": 30,
      "measurement": {
        "value": 142.109,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 17,
      "minute": 45,
      "measurement": {
        "value": 134.774,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 18,
      "minute": 0,
      "measurement": {
        "value": 140.348,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 18,
      "minute": 15,
      "measurement": {
        "value": 134.774,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 18,
      "minute": 30,
      "measurement": {
        "value": 130.446,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 18,
      "minute": 45,
      "measurement": {
        "value": 128.75,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 19,
      "minute": 0,
      "measurement": {
        "value": 134.022,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 19,
      "minute": 15,
      "measurement": {
        "value": 124.755,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 19,
      "minute": 30,
      "measurement": {
        "value": 122.124,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 19,
      "minute": 45,
      "measurement": {
        "value": 116.733,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 20,
      "minute": 0,
      "measurement": {
        "value": 120.663,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 20,
      "minute": 15,
      "measurement": {
        "value": 116.722,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 20,
      "minute": 30,
      "measurement": {
        "value": 116.41,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 20,
      "minute": 45,
      "measurement": {
        "value": 108.603,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 21,
      "minute": 0,
      "measurement": {
        "value": 116.614,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 21,
      "minute": 15,
      "measurement": {
        "value": 105.478,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 21,
      "minute": 30,
      "measurement": {
        "value": 102.407,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 21,
      "minute": 45,
      "measurement": {
        "value": 96.479,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 22,
      "minute": 0,
      "measurement": {
        "value": 97.757,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 22,
      "minute": 15,
      "measurement": {
        "value": 94.439,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 22,
      "minute": 30,
      "measurement": {
        "value": 93.59,
        "unit": "SWEDISH_ORE/KWH"
      }
    },
    {
      "year": 2026,
      "month": 1,
      "week": 2,
      "day": 10,
      "hour": 22,
      "minute": 45,
      "measurement": {
        "value": 92.183,
        "unit": "SWEDISH_ORE/KWH"
      }
    }
  ]
}
 
 */
