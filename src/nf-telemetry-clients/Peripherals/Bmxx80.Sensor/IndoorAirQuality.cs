using System;
using System.Collections;

namespace TelemetryStash.Bmxx80.Sensor
{
    internal class IndoorAirQuality
    {
        private readonly double _phSlope;
        private int _burnInCycles;
        private ArrayList _gasCalData;
        private double _gasCeil;
        private readonly int _gasRecalPeriod;
        private int _gasRecalStep;

        public IndoorAirQuality(int burnInCycles = 300, int gasRecalPeriod = 3600, double phSlope = 0.03)
        {
            _phSlope = phSlope;
            _burnInCycles = burnInCycles;
            _gasCalData = new ArrayList();
            _gasCeil = 0;
            _gasRecalPeriod = gasRecalPeriod;
            _gasRecalStep = 0;
        }

        public bool TryCalculateIAC(double temperature, double humidity, double gasResistance, out double airQuality)
        {
            // Calculate saturation density and absolute humidity
            var rhoMax = WaterSatDensity(temperature);
            var humAbs = humidity * 10 * rhoMax;

            // Compensate exponential impact of humidity on resistance
            var compGas = gasResistance * Math.Exp(_phSlope * humAbs);

            if (_burnInCycles > 0)
            {
                // Check if burn-in-cycles are recorded
                _burnInCycles--; // Count down cycles
                if (compGas > _gasCeil)
                {
                    // If value exceeds current ceiling, add to calibration list and update ceiling
                    _gasCalData = new ArrayList { compGas };
                    _gasCeil = compGas;
                }

                airQuality = 0;
                return false; // Return null as sensor burn-in is not yet completed
            }
            else
            {
                // Adapt calibration
                if (compGas > _gasCeil)
                {
                    _gasCalData.Add(compGas);
                    if (_gasCalData.Count > 100)
                    {
                        _gasCalData.RemoveAt(0);
                    }
                    _gasCeil = GetAverage(_gasCalData);
                }

                // Calculate and print relative air quality on a scale of 0-100%
                // Use quadratic ratio for steeper scaling at high air quality
                // Clip air quality at 100%
                airQuality = Math.Min(Math.Pow(compGas / _gasCeil, 2), 1) * 100;

                // For compensating negative drift (dropping resistance) of the gas sensor:
                // Delete oldest value from calibration list and add current value
                _gasRecalStep++;
                if (_gasRecalStep >= _gasRecalPeriod)
                {
                    _gasRecalStep = 0;
                    _gasCalData.Add(compGas);
                    _gasCalData.RemoveAt(0);
                    _gasCeil = GetAverage(_gasCalData);
                }


                return true;
            }
        }

        // https://github.com/thstielow/raspi-bme680-iaq/blob/main/bme680IAQ.py
        // Calculates the saturation water density of air at the current temperature (in °C)
        // Returns the saturation density rho_max in kg/m^3
        // This is equal to a relative humidity of 100% at the current temperature 
        private static double WaterSatDensity(double temp)
        {
            return (6.112 * 100 * Math.Exp((17.62 * temp) / (243.12 + temp))) / (461.52 * (temp + 273.15));
        }

        private static double GetAverage(ArrayList list)
        {
            double sum = 0;
            foreach (double value in list)
            {
                sum += value;
            }
            return sum / list.Count;
        }
    }
}
