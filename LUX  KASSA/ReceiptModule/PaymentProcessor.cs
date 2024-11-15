using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUX__KASSA.ReceiptModule
{
    public class PaymentProcessor
    {
        public string ChoosePaymentMethod()
        {
            Console.WriteLine("\nPlease choose a payment method:");
            Console.WriteLine("1. Card");
            Console.WriteLine("2. Cash");
            Console.Write("Enter your choice: ");

            string? paymentChoice = Console.ReadLine();
            return paymentChoice == "1" ? "Card" : "Cash";
        }

        public string GenerateCardPaymentDetails()
        {
            StringBuilder details = new StringBuilder();
            details.AppendLine($"{"Payment received:",-20} {"DEBIT/CREDIT",-20}");
            details.AppendLine($"{"MasterCard",-20} {"**********5837",-20}");
            details.AppendLine($"{"Trace:5086-577977",-20} {"Swe:43614677",-20}");
            details.AppendLine($"{"Site:",-20} {"410917",-20}");
            details.AppendLine($"{"TVR:0000001000",-20} {"TSI:F800",-20}");
            details.AppendLine($"{DateTime.Now,-20} {"AID:658F8687987H",-20}");
            details.AppendLine($"{"Ref:646890321",-20} {"Rsp:0504839967AB",-20}");

            return details.ToString();
        }
    }
}
