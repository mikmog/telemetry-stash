namespace TelemetryStash.Database.Models;

public record Register(
    int RegisterSetId, // id
    string RegisterIdentifier, // id
    string? Name = null,
    string? Type = null,
    string? Unit = null,
    string? Description = null
)
{
    public int Id { get; set; }
}

/* 
  Id    |   SetId    | RegisterIdentifier| Name      | Type      | Description
 1      |    1       |   "L1"            | "Number"  | "Volt"    | "Phase 1 Voltage"
 2      |    1       |   "L2"            | "Number"  | "Volt"    | "Phase 2 Voltage"
 3      |    1       |   "L3"            | "Number"  | "Volt"    | "Phase 3 Voltage"

 4      |    2       |   "Temp"          | "Number"  | "Celcius" | "Temperatur ute"
 5      |    2       |   "Hum"           | "Number"  | "%"       | "Luftfuktighet"

 6      |    3       |   "Name"          | "Text"    | "Text"    | "Namn"
 7      |    3       |   "Strength"      | "Number"  | "Number"  | "Signalstyrka"
 7      |    3       |   "Mac"           | "Text"    | "Text"    | "Macadress"

 */