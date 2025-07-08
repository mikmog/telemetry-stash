using System;

namespace RipTide.Nfirmware.Components
{
    public class ErrorHandler
    {
        private Display _display;
        private Buzzer _buzzer;

        public void Initialize(Display display, Buzzer buzzer)
        {
            _display = display;
            _buzzer = buzzer;
        }

        public void OnError(string errorMessage)
        {
            Console.WriteLine($"Error: {errorMessage}");
            _display.SetText(errorMessage);
            _buzzer.BuzzError();
        }
    }
}
