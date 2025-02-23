﻿using Iot.Device.Imu;
using nanoFramework.Hardware.Esp32;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.NfClient.Services;

namespace TelemetryStash.MpuxxxxGyro.Sensor
{
	public class Mpu6050Gyro
    {
        public void RunDemo()
        {
            Configuration.SetPinFunction(11, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(12, DeviceFunction.I2C1_CLOCK);

            var i2c = new I2cConnectionSettings(1, Mpu6050.DefaultI2cAddress);
            using var gyro = new Mpu6050(I2cDevice.Create(i2c));

            //Debug.WriteLine("Before::");
            //Debug.WriteLine($"Gyro X bias = {gyro.GyroscopeBias.X}");
            //Debug.WriteLine($"Gyro Y bias = {gyro.GyroscopeBias.Y}");
            //Debug.WriteLine($"Gyro Z bias = {gyro.GyroscopeBias.Z}");
            //Debug.WriteLine($"Acc X bias = {gyro.AccelerometerBias.X}");
            //Debug.WriteLine($"Acc Y bias = {gyro.AccelerometerBias.Y}");
            //Debug.WriteLine($"Acc Z bias = {gyro.AccelerometerBias.Z}");

            //Debug.WriteLine("Running Gyroscope and Accelerometer calibration");

            // TODO Calibration tries to reset to Bandwidth0184Hz but fails.
            //var result = gyro.CalibrateGyroscopeAccelerometer();
            //Debug.WriteLine($"Result:");
            //Debug.WriteLine($"Gyro X = {result.Gyroscope.X} vs >0.005");
            //Debug.WriteLine($"Gyro Y = {result.Gyroscope.Y} vs >0.005");
            //Debug.WriteLine($"Gyro Z = {result.Gyroscope.Z} vs >0.005");
            //Debug.WriteLine($"Acc X = {result.Accelerometer.X} vs >0.005 & <0.015");
            //Debug.WriteLine($"Acc Y = {result.Accelerometer.Y} vs >0.005 & <0.015");
            //Debug.WriteLine($"Acc Z = {result.Accelerometer.Z} vs >0.005 & <0.015");

            //Debug.WriteLine("After:");
            //Debug.WriteLine($"Gyro X bias = {gyro.GyroscopeBias.X}");
            //Debug.WriteLine($"Gyro Y bias = {gyro.GyroscopeBias.Y}");
            //Debug.WriteLine($"Gyro Z bias = {gyro.GyroscopeBias.Z}");
            //Debug.WriteLine($"Acc X bias = {gyro.AccelerometerBias.X}");
            //Debug.WriteLine($"Acc Y bias = {gyro.AccelerometerBias.Y}");
            //Debug.WriteLine($"Acc Z bias = {gyro.AccelerometerBias.Z}");

            Debug.WriteLine("GetGyroscopeReading");
            for (int i = 0; i < 100; i++)
            {
                var read = gyro.GetGyroscopeReading();
                Debug.WriteLine($"X:{Round.ToWhole(read.X)}, Y:{Round.ToWhole(read.Y)}, Z:{Round.ToWhole(read.Z)}");
                Thread.Sleep(100);
            }

            Debug.WriteLine("GetAccelerometer");
            for (int i = 0; i < 1000; i++)
            {
                var read = gyro.GetAccelerometer();
                Debug.WriteLine($"X:{Round.ToWhole(read.X)}, Y:{Round.ToWhole(read.Y)}, Z:{Round.ToWhole(read.Z)}");
                Thread.Sleep(100);
            }
        }
    }
}
