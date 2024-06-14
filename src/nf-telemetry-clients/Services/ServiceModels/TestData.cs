using System;
using System.Collections;

namespace TelemetryStash.ServiceModels
{
    public class TestData
    {
        public static string AidonMessage =>
@"/ADN9 6534

0-0:1.0.0(240217174050W)
1-0:1.8.0(00007805.332*kWh)
1-0:2.8.0(11111111.111*kWh)
1-0:3.8.0(00000003.239*kVArh)
1-0:4.8.0(00001945.988*kVArh)
1-0:1.7.0(0000.870*kW)
1-0:2.7.0(0001.001*kW)
1-0:3.7.0(0002.000*kVAr)
1-0:4.7.0(0000.551*kVAr)
1-0:21.7.0(0000.269*kW)
1-0:22.7.0(0000.345*kW)
1-0:41.7.0(0000.526*kW)
1-0:42.7.0(0003.000*kW)
1-0:61.7.0(0000.060*kW)
1-0:62.7.0(0004.000*kW)
1-0:23.7.0(0005.000*kVAr)
1-0:24.7.0(0000.277*kVAr)
1-0:43.7.0(0006.000*kVAr)
1-0:44.7.0(0000.184*kVAr)
1-0:63.7.0(0000.000*kVAr)
1-0:64.7.0(0000.081*kVAr)
1-0:32.7.0(234.7*V)
1-0:52.7.0(234.6*V)
1-0:72.7.0(234.5*V)
1-0:31.7.0(001.6*A)
1-0:51.7.0(002.3*A)
1-0:71.7.0(000.4*A)
!95CB
";

        public static Telemetry StaticKeyTelemetry => new()
        {
            Timestamp = DateTime.Parse("2024-02-17T17:40:50Z"),
            RegisterSets = new ArrayList()
                {
                    new RegisterSet
                    {
                        Identifier = "Aidon",
                        Registers = new Register[]
                        {
                            new ("TAE", 7805, 233),
                            new ("TEI", 0, 0),
                            new ("TRD", 3, 932),
                            new ("TRI", 1945, 889),
                            new ("ED", 0, 78),
                            new ("EI", 0, 0),
                            new ("RED", 0, 0),
                            new ("REI", 0, 155),
                            new ("1ED", 0, 962),
                            new ("1EI", 0, 0),
                            new ("2ED", 0, 625),
                            new ("2EI", 0, 0),
                            new ("3ED", 0, 60),
                            new ("3EI", 0, 0),
                            new ("1RD", 0, 0),
                            new ("1RI", 0, 772),
                            new ("2RD", 0, 0),
                            new ("2RI", 0, 481),
                            new ("3RD", 0, 0),
                            new ("3RI", 0, 18),
                            new ("1V", 234, 7),
                            new ("2V", 234, 6),
                            new ("3V", 234, 5),
                            new ("1C", 1, 6),
                            new ("2C", 2, 3),
                            new ("3C", 0, 4)
                        }
                    },
                    new RegisterSet
                    {
                        Identifier = "Am2320",
                        Registers = new Register[]
                        {
                            new ("Hum", 80, 0),
                            new ("Temp", 21, 44),
                        }
                    }
                }
        };
    }
}
