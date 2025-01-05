using System;
using System.Diagnostics;
using System.Threading;

namespace RipTide.Nfirmware
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
