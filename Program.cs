using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace Ultrasonic
{
    internal class Program
    {
        private static int TriggerPin = 14;
        private static int EchoPin = 10;

        private static int LoopDelay = 1000;

        private static bool KeepRunning = true;
        private static GpioController gpioController = new GpioController();

        /// <summary>
        /// Получение расстояния с ультразвукового датчика
        /// </summary>
        /// <returns>Расстояние в сантиметрах</returns>
        private static double GetDistance()
        {
            Console.WriteLine("1");
            var mre = new ManualResetEvent(false);
            var pulseLength = new Stopwatch();

            Console.WriteLine("2");
            gpioController.Write(TriggerPin, PinValue.High);
            mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
            gpioController.Write(TriggerPin, PinValue.Low);

            Console.WriteLine("3");
            pulseLength.Start();
            while (gpioController.Read(EchoPin) == PinValue.Low)
            { }

            Console.WriteLine("4");
            while (gpioController.Read(EchoPin) == PinValue.High)
            { }
            pulseLength.Stop();

            Console.WriteLine("5");
            var distance = pulseLength.Elapsed.TotalSeconds * 17000;
            return distance;
        }

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                KeepRunning = false;
            };

            gpioController.OpenPin(TriggerPin, PinMode.Output);
            gpioController.OpenPin(EchoPin, PinMode.Input);

            while (KeepRunning)
            {
                try
                {
                    var distanceString = $"{GetDistance()} cm";
                    Console.WriteLine(distanceString);
                    Thread.Sleep(LoopDelay);
                }
                catch (Exception exception)
                {
                    Console.Write(exception.Message);
                }
            }

            gpioController.Write(EchoPin, PinValue.Low);
            gpioController.Write(TriggerPin, PinValue.Low);
            gpioController.ClosePin(EchoPin);
            gpioController.ClosePin(TriggerPin);
            Console.WriteLine($"{Environment.NewLine}Exited");
        }
    }
}
