# Telemetry Stash Functions

## Sample telemetry JSON

```
{
  "staticKeys": [
    {
      "ts": "230901101336494",
      "reg": {
        "P1": {
          "C1": 8,
          "C2": 8,
          "C3": 8
        },
        "Am2320": {
          "Hum": 80,
          "Temp": 21.44
        }
      }
    }
  ],

  "dynamicKeys": [
    {
      "ts": "230901101336494",
      "set":  "BT",
      "reg": {
        "fe:fe": {
          "name": "Huw",
          "strength": 8,
          "mac": "fe:fe"
        },
        "ff:f0": {
          "name": "Son",
          "strength": 8,
          "mac": "ff:f0"
        },
        "ff:f1": {
          "name": "Mot",
          "strength": 8,
          "mac": "ff:f1"
        }
      }
    }
  ]
}
```
