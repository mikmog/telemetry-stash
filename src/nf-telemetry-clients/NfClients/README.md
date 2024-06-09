nanoFramework nanoCLR
https://www.nuget.org/packages/nanoclr

nanoclr run --assemblies "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\mscorlib.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\nanoFramework.Hardware.Esp32.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\nanoFramework.Runtime.Events.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\nanoFramework.System.Collections.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\nanoFramework.System.Text.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\P1MeterCollector.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\System.Device.Gpio.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\System.IO.Ports.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\System.IO.Streams.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\System.Math.pe" "C:\Projects\TeamCopyInsights\DataCollector\P1MeterCollector\bin\Debug\DataCollector.Application.pe"
nanoclr virtualserial --create COM99

dotnet tool install -g nanoff
dotnet tool update -g nanoff
nanoff --listports
nanoff --platform esp32 --serialport COM3 --devicedetails 

Update clr
nanoff --nanodevice --update --serialport COM3

nanoff --nanodevice --devicedetails --serialport COM3

*** Hold down the BOOT/FLASH button in ESP32 board ***
nanoff -v d --target ESP_WROVER_KIT --serialport COM8 --update --masserase
nanoff -v d --platform  esp32 --serialport COM3 --update --masserase



nanoff -v d --target XIAO_ESP32C3 --serialport COM9 --update  --masserase
nanoff -v d --target ESP32_PSRAM_REV0 --serialport COM13 --update --fwversion 1.9.1.7 --masserase
nanoff -v d --target ESP32_S3_BLE --serialport COM3 --update --masserase
nanoff -v d --target ESP32_S3_ALL --serialport COM3 --update --masserase


nanoclr virtualserial --install

nanoff -v d --target ESP32_REV3 --serialport COM3 --update --masserase


nanoff --filedeployment C:\Temp\deploy.json




https://www.scadacore.com/tools/programming-calculators/online-hex-converter/
https://patorjk.com/software/taag/#p=display&f=Ogre&t=TeamCopy

        // https://www.convertcase.com/hashing/crc-16-checksum-calculator
        // https://aidon.com/wp-content/uploads/2023/06/AIDONFD_RJ12_HAN_Interface_SV.pdf
        // Aidon 6442 S

vstest.console.exe .\Tests\NFUnitTestBitConverter\bin\Release\NFUnitTest.dll  /Settings:.\Tests\NFUnitTestAdapater\nano.runsettings /TestAdapterPath:.\nanoFramework.TestFramework\source\TestAdapter\bin\Debug\net4.8 /Diag:.\log.txt /Logger:trx


# Ignoring changes to tracked files
git update-index --skip-worktree AppSettings.Development.cs
git update-index --skip-worktree AppSettings.Production.cs

# Resuming tracking changes to tracked files
git update-index --no-skip-worktree AppSettings.Development.cs
git update-index --no-skip-worktree AppSettings.Production.cs


```
System Information
HAL build info: nanoCLR running @ ESP32_C3 built with ESP-IDF 38eeba2
  Target:   XIAO_ESP32C3
  Platform: ESP32

Firmware build Info:
  Date:        Mar 22 2024
  Type:        MinSizeRel build, chip rev. >= 3, without support for PSRAM
  CLR Version: 1.9.1.7
  Compiler:    GNU ARM GCC v8.4.0

OEM Product codes (vendor, model, SKU): 0, 0, 0

Serial Numbers (module, system):
  00000000000000000000000000000000
  0000000000000000

Target capabilities:
  Has nanoBooter: NO
  IFU capable: NO
  Has proprietary bootloader: YES

AppDomains:

Assemblies:

Native Assemblies:
  mscorlib v100.5.0.19, checksum 0x445C7AF9
  nanoFramework.Runtime.Native v100.0.9.0, checksum 0x109F6F22
  nanoFramework.Hardware.Esp32 v100.0.10.0, checksum 0x6A20A689
  nanoFramework.Networking.Sntp v100.0.4.4, checksum 0xE2D9BDED
  nanoFramework.ResourceManager v100.0.0.1, checksum 0xDCD7DF4D
  nanoFramework.System.Collections v100.0.1.0, checksum 0x2DC2B090
  nanoFramework.System.Text v100.0.0.1, checksum 0x8E6EB73D
  nanoFramework.System.IO.Hashing v100.0.0.1, checksum 0xEBD8ED20
  nanoFramework.System.Security.Cryptography v100.0.0.2, checksum 0xF4AEFE6C
  nanoFramework.Runtime.Events v100.0.8.0, checksum 0x0EAB00C9
  EventSink v1.0.0.0, checksum 0xF32F4C3E
  System.IO.FileSystem v1.0.0.2, checksum 0x545A6C79
  System.Math v100.0.5.5, checksum 0x9F9E2A7E
  System.Net v100.2.0.1, checksum 0xD82C1452
  System.Device.Adc v100.0.0.0, checksum 0xE5B80F0B
  System.Device.Gpio v100.1.0.6, checksum 0x097E7BC5
  System.Device.I2c v100.0.0.2, checksum 0xFA806D33
  System.Device.I2c.Slave v1.0.0.0, checksum 0x4238164B
  System.Device.I2s v100.0.0.1, checksum 0x478490FE
  System.Device.Pwm v100.1.0.4, checksum 0xABF532C3
  System.IO.Ports v100.1.6.1, checksum 0xB798CE30
  System.Device.Spi v100.1.2.0, checksum 0x3F6E2A7E
  System.Runtime.Serialization v100.0.0.0, checksum 0x0A066871
  System.Device.Wifi v100.0.6.4, checksum 0x00A058C6
  Windows.Storage v100.0.3.0, checksum 0xF0C37E1B



++++++++++++++++++++++++++++++++
++        Memory Map          ++
++++++++++++++++++++++++++++++++
  Type     Start       Size
++++++++++++++++++++++++++++++++
  RAM   0x3fca6340  0x0001d000
  FLASH 0x00000000  0x00400000



+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++                   Flash Sector Map                        ++
+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  Region     Start      Blocks   Bytes/Block    Usage
+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
      0    0x00010000       1      0x1A0000     nanoCLR
      1    0x001B0000       1      0x1F0000     Deployment
      2    0x003C0000       1      0x040000     Configuration


+++++++++++++++++++++++++++++++++++++++++++++++++++
++              Storage Usage Map                ++
+++++++++++++++++++++++++++++++++++++++++++++++++++
  Start        Size (kB)           Usage
+++++++++++++++++++++++++++++++++++++++++++++++++++
  0x003C0000    0x040000 (256kB)    Configuration
  0x00010000    0x1A0000 (1664kB)   nanoCLR
  0x001B0000    0x1F0000 (1984kB)   Deployment



Deployment Map
Empty
```