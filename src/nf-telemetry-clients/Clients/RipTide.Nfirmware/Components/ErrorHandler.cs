using System;

namespace RipTide.Nfirmware.Components
{
    public class ErrorHandler
    {
        private Display _display;

        public void Initialize(Display display)
        {
            _display = display;
        }

        public void HandleError(string errorMessage)
        {
            // Log the error message
            Console.WriteLine($"Error: {errorMessage}");
            // Additional error handling logic can be added here
        }
    }
}
