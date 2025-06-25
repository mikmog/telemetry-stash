using System;
using System.Collections;

namespace TelemetryStash.Shared
{
    [Serializable]
    public class Telemetry
    {
        public DateTime Timestamp { get; set; }

        public string[] Tags { get; set; }

        public ArrayList RegisterSets { get; set; }
    };

    [Serializable]
    public class RegisterSet
    {
        public string Identifier { get; set; }

        public string[] Tags { get; set; }

        public Register[] Registers { get; set; }
    }

    [Serializable]
    public class Register
    {
        public Register(string identifier, string value, RegisterValueType valueType = RegisterValueType.Text)
        {
            ValueType = valueType;
            Identifier = identifier;
            Value = value;
        }

        public Register(string identifier, int value)
        {
            ValueType = RegisterValueType.Number;
            Identifier = identifier;
            Value = value.ToString();
        }
        
        public Register(string identifier, uint value)
        {
            ValueType = RegisterValueType.Number;
            Identifier = identifier;
            Value = value.ToString();
        }

        public Register(string identifier, double value, DecimalPrecision precision)
        {
            ValueType = RegisterValueType.Number;
            Identifier = identifier;

            switch (precision)
            {
                case DecimalPrecision.None:
                    Value = Math.Round(value).ToString();
                    break;
                case DecimalPrecision.One:
                    Value = Round.TrimTrailingZeroes(value.ToString("f1"));
                    break;
                case DecimalPrecision.Two:
                    Value = Round.TrimTrailingZeroes(value.ToString("f2"));
                    break;
                case DecimalPrecision.Three:
                    Value = Round.TrimTrailingZeroes(value.ToString("f3"));
                    break;
                case DecimalPrecision.Half:
                    Value = Round.TrimTrailingZeroes(Round.ToHalf(value).ToString("f1"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(precision), precision.ToString());
            }
        }

        public object RegisterValue
        {
            get
            {
                switch (ValueType)
                {
                    case RegisterValueType.Number:
                        return Convert.ToDouble(Value);
                    case RegisterValueType.Text:
                        return Value;
                    default:
                        return Value;
                }
            }
        }

        public RegisterValueType ValueType { get; }
        public string Identifier { get; }
        public string Value { get; }
    }

    public enum RegisterValueType
    {
        Number = 0,
        Text = 1
    }

    public enum DecimalPrecision
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Half = 50
    }
}
