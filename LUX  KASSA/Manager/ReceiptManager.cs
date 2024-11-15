using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LUX__KASSA.ReceiptModule;
using Lux_Cash_Register.Models;

namespace Lux_Cash_Register.Manager
{
    public class ReceiptManager
    {
        private const string ReceiptFilePath = "../../../Receipt";
        private int latestReceiptNumber;
        private ReceiptFormatter receiptFormatter;
        private PaymentProcessor paymentProcessor;

        public ReceiptManager()
        {
            receiptFormatter = new ReceiptFormatter();
            paymentProcessor = new PaymentProcessor();
            latestReceiptNumber = LoadLatestReceiptNumber();
        }

        private int LoadLatestReceiptNumber()
        {
            string path = Path.Combine(ReceiptFilePath, "latest_receipt_number.txt");
            if (File.Exists(path) && int.TryParse(File.ReadAllText(path), out int number))
            {
                return number;
            }
            return 0;
        }

        public int GetNextReceiptNumber()
        {
            latestReceiptNumber++;
            File.WriteAllText(Path.Combine(ReceiptFilePath, "latest_receipt_number.txt"), latestReceiptNumber.ToString());
            return latestReceiptNumber;
        }

        public void PrintAndSaveReceipt(List<(Product, double)> cart)
        {
            Console.Clear();
            double subtotal = 0;
            foreach (var item in cart)
            {
                subtotal += item.Item1.GetEffectivePrice(DateTime.Now) * item.Item2;
            }

            double taxAmount = subtotal * 0.25;
            double totalAmount = subtotal + taxAmount;

            Console.WriteLine($"\nTotal Amount Due: {totalAmount:C}");
            Console.WriteLine(new string('=', 60));

            string paymentMethod = paymentProcessor.ChoosePaymentMethod();
            string formattedReceipt = receiptFormatter.FormatReceipt(cart, paymentMethod, GetNextReceiptNumber());

            // Clear the console for the final receipt display
            Console.Clear();
            PrintCenteredReceipt(formattedReceipt);

            string fileName = Path.Combine(ReceiptFilePath, $"Receipt-{DateTime.Now:yyyy-MM-dd}.txt");
            Directory.CreateDirectory(ReceiptFilePath);

            using (StreamWriter writer = new StreamWriter(fileName, append: true))
            {
                writer.WriteLine(formattedReceipt);
                writer.WriteLine(new string('=', 60));
            }

            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }

        private void PrintCenteredReceipt(string receipt)
        {
            // Split the formatted receipt into lines
            var receiptLines = receipt.Split(Environment.NewLine);

            foreach (var line in receiptLines)
            {
                // Calculate padding for each line based on console width
                int padding = Math.Max((Console.WindowWidth - line.Length) / 2, 0);
                Console.SetCursorPosition(padding, Console.CursorTop);
                Console.WriteLine(line);
            }
        }
    }
}