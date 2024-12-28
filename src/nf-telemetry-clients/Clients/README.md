# Internal notes

## nanoFramework

### Installing

```bash
dotnet tool install -g nanoff
dotnet tool update -g nanoff
```

### Using

- [nanoCLR](https://www.nuget.org/packages/nanoclr)
- [Running the virtual nanoCLR](https://github.com/nanoframework/nanoframework.github.io/blob/pages-source/content/getting-started-guides/virtual-device.md#running-the-virtual-nanoclr)

```bash
nanoff --listports
nanoff --nanodevice --devicedetails --serialport COM3
nanoclr virtualserial --install
nanoclr virtualserial --create COM99
nanoff --filedeployment C:\Temp\deploy.json
nanoff --platform esp32 --serialport COM3 --devicedetails
nanoff --nanodevice --update --serialport COM3
```

### Updating

*** Hold down the BOOT/FLASH button in ESP32 board ***

```bash
nanoff -v d --target ESP_WROVER_KIT --serialport COM8 --update --masserase
nanoff -v d --target XIAO_ESP32C3 --serialport COM9 --update  --masserase
nanoff -v d --target ESP32_C6_Thread --serialport COM5 --update --masserase

nanoff -v d --platform esp32 --serialport COM3 --update --masserase
nanoff -v d --target ESP32_PSRAM_REV0 --serialport COM13 --update --fwversion 1.9.1.7 --masserase
nanoff -v d --target ESP32_S3_BLE --serialport COM8 --update --masserase
nanoff -v d --target ESP32_S3_ALL --serialport COM3 --update --masserase
nanoff -v d --target ESP32_REV3 --serialport COM3 --update --masserase
nanoff -v d --target ESP32_PSRAM_BLE_GenericGraphic_REV3 --serialport COM8 --update --masserase

```

## Misc

### Tools

- https://www.scadacore.com/tools/programming-calculators/online-hex-converter/
- https://patorjk.com/software/taag/#p=display&f=Ogre&t=Telemetry%20Stash