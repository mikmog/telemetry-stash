using System;
using System.Collections;

namespace TelemetryStash.Shared
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

        public static Telemetry NumbersOnlyTelemetry => new()
        {
            Timestamp = new DateTime(2025, 01, 01, 12, 0, 0),
            RegisterSets = new ArrayList()
                {
                    new RegisterSet
                    {
                        Identifier = "Set1",
                        Tags = new string[] { "Tag1", "Tag2" },
                        Registers = new Register[]
                        {
                            new ("TAE", "7805.233", RegisterValueType.Number),
                            new ("TEI", 0),
                            new ("TRD", "3.239",    RegisterValueType.Number),
                            new ("TRI", "1945.998", RegisterValueType.Number),
                            new ("ED",  "0.87",     RegisterValueType.Number),
                            new ("EI",  0),
                            new ("RED", 0),
                            new ("REI", "0.155",    RegisterValueType.Number),
                            new ("1ED", "0.269",    RegisterValueType.Number),
                            new ("1EI", 0),
                            new ("2ED", "0.526",    RegisterValueType.Number),
                            new ("2EI", 0),
                            new ("3ED", "0.06",     RegisterValueType.Number),
                            new ("3EI", 0),
                            new ("1RD", 0),
                            new ("1RI", "0.277",    RegisterValueType.Number),
                            new ("2RD", 0),
                            new ("2RI", "0.184",    RegisterValueType.Number),
                            new ("3RD", 0),
                            new ("3RI", "0.81",     RegisterValueType.Number),
                            new ("1V",  "234.7",    RegisterValueType.Number),
                            new ("2V",  "234.6",    RegisterValueType.Number),
                            new ("3V",  "234.5",    RegisterValueType.Number),
                            new ("1C",  "1.6",      RegisterValueType.Number),
                            new ("2C",  "2.3",      RegisterValueType.Number),
                            new ("3C",  "0.4",      RegisterValueType.Number)
                        }
                    },
                    new RegisterSet
                    {
                        Identifier = "Set2",
                        Registers = new Register[]
                        {
                            new ("Hum", 80),
                            new ("Temp", "21.44", RegisterValueType.Number),
                        }
                    }
                }
        };

        public static Telemetry TextOnlyTelemetry => new()
        {
            Timestamp = new DateTime(2025, 01, 01, 12, 0, 0),
            RegisterSets = new ArrayList()
            {
                new RegisterSet
                {
                    Identifier = "TextOnlyTelemetry",
                    Tags = new string[] { "Tag1", "Tag2" },
                    Registers = new Register[]
                    {
                        new("Backslash",        "Text\\1"),
                        new("Backspace",        "Text\b1"),
                        new("CarriageReturn",   "Text\r1"),
                        new("DoubleQuote",      "Text1\""),
                        new("FormFeed",         "Text\f1"),
                        new("Newline",          "Text\n1"),
                        new("Tab",              "Text\t1"),
                        new("OctalNull",        "Text\01"),
                        new("Null",             "Tex\0t1"),
                        new("Array",            "['Text3', 1]"),
                        new("Json",             "{\"Text1\" : \"/Text1/\"}"),
                        new("Unicode",          "ä"),
                        new("Empty",            ""),
                    }
                }
            }
        };

        public static Telemetry LongTextOnlyTelemetry => new()
        {
            Timestamp = new DateTime(2025, 01, 01, 12, 0, 0),
            RegisterSets = new ArrayList()
            {
                new RegisterSet
                {
                    Identifier = "TextOnlyTelemetry",
                    Tags = new string[] { "Tag1", "Tag2" },
                    Registers = new Register[]
                    {
                        new("LongText", "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.")
                    }
                }
            }
        };

        public static string[] OneHundredWords => new string[]
        {
            "apple", "banana", "cherry", "date", "elderberry", "fig", "grape", "honeydew", "kiwi", "lemon",
            "mango", "nectarine", "orange", "papaya", "quince", "raspberry", "strawberry", "tangerine", "ugli", "vanilla",
            "watermelon", "xigua", "yellowfruit", "zucchini", "apricot", "blackberry", "blueberry", "cantaloupe", "dragonfruit", "grapefruit",
            "jackfruit", "kumquat", "lime", "lychee", "mandarin", "mulberry", "olive", "peach", "pear", "pineapple",
            "plum", "pomegranate", "pumpkin", "rhubarb", "starfruit", "tomato", "avocado", "coconut", "cranberry", "currant",
            "gooseberry", "guava", "kiwifruit", "loquat", "mangosteen", "passionfruit", "persimmon", "plantain", "prune", "sapodilla",
            "soursop", "tamarind", "yuzu", "bilberry", "boysenberry", "cloudberry", "dewberry", "elderflower", "feijoa", "huckleberry",
            "jabuticaba", "jostaberry", "longan", "marionberry", "medlar", "miraclefruit", "naranjilla", "pawpaw", "pitaya", "rambutan",
            "redcurrant", "salak", "santol", "serviceberry", "sloe", "sugarapple", "surinamcherry", "whitecurrant", "wineberry", "wolfberry",
            "yumberry", "ziziphus", "acerola", "ackee", "açaí", "bignay", "burdekinplum", "calamondin", "camu", "cempedak"
        };
    }
}
