using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUX_Cash_Register

{
    public class ErrorHandler : IErrorHandler
    {
        private readonly string logFilePath;

        public ErrorHandler(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public void LogError(string errorMessage)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {errorMessage}");
                }
            }
            catch (Exception)
            {
                // Suppress logging errors silently

            }
        }

        public void ShowError(string errorMessage)
        {
            Console.WriteLine($"Error: {errorMessage}");
            LogError(errorMessage);
        }
    }
}

