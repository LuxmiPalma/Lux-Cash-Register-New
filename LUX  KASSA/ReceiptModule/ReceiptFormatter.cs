using Lux_Cash_Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUX__KASSA.ReceiptModule
{
    public class ReceiptFormatter
    {
        private const int ReceiptWidth = 60;
        private readonly PaymentProcessor _paymentProcessor; // Declare and initialize the PaymentProcessor

        // Constructor to initialize the PaymentProcessor
        public ReceiptFormatter()
        {
            _paymentProcessor = new PaymentProcessor();
        }
        public string FormatReceipt(List<(Product, double)> cart, string paymentMethod, int receiptNumber)
        {
            StringBuilder receiptBuilder = new StringBuilder();

            // Header and Store Info
            receiptBuilder.AppendLine(new string('=', ReceiptWidth + 4));
            receiptBuilder.AppendLine(CenterTextWithBorder("- The Cherry Market -"));
            receiptBuilder.AppendLine(CenterTextWithBorder("Sjödalstorget 7, 141 24 Huddinge"));
            receiptBuilder.AppendLine(CenterTextWithBorder("Opening hrs: Mon-Fri: 07:00-22:00 | Sat-Sun: 08:00-22:00"));
            receiptBuilder.AppendLine(new string('=', ReceiptWidth + 4));
            receiptBuilder.AppendLine(CenterTextWithBorder($"Cashier: 1111       Register: 01       Receipt: #{receiptNumber}"));
            receiptBuilder.AppendLine(CenterTextWithBorder($"Date: {DateTime.Now:yyyy-MM-dd}   Time: {DateTime.Now:HH:mm:ss}"));
            receiptBuilder.AppendLine(new string('-', ReceiptWidth + 4));

            // Product Table Headers
            receiptBuilder.AppendLine(CenterTextWithBorder($"{"Product",-20} {"Qty",5} {"Amount + Price",20} {"Subtotal",10}"));
            receiptBuilder.AppendLine(new string('-', ReceiptWidth + 4));

            double subtotal = 0;
            double totalDiscount = 0;

            foreach (var item in cart)
            {
                Product product = item.Item1;
                double quantity = item.Item2;
                double effectivePrice = product.GetEffectivePrice(DateTime.Now);
                double lineTotal = effectivePrice * quantity;
                subtotal += lineTotal;

                double discount = (product.Price - effectivePrice) * quantity;
                bool hasDiscount = discount > 0;
                if (hasDiscount)
                {
                    totalDiscount += discount;
                }

                string productLabel = hasDiscount ? $"{product.Name} *Campaign*" : product.Name;
                string qtyPriceDisplay = $"{quantity}*{effectivePrice:C}";
                receiptBuilder.AppendLine(CenterTextWithBorder($"{productLabel,-20} {quantity,5} {qtyPriceDisplay,15} {lineTotal,10:C}"));
            }

            receiptBuilder.AppendLine(new string('-', ReceiptWidth + 4));
            receiptBuilder.AppendLine(CenterTextWithBorder($"{"Subtotal:",-40}{subtotal,20:C}"));

            if (totalDiscount > 0)
            {
                receiptBuilder.AppendLine(CenterTextWithBorder($"{"Total Discount:",-40}{-totalDiscount,20:C}"));
            }

            double taxAmount = subtotal * 0.25;
            double totalAmount = subtotal + taxAmount;

            receiptBuilder.AppendLine(CenterTextWithBorder($"{"VAT (25%):",-40}{taxAmount,20:C}"));
            receiptBuilder.AppendLine(CenterTextWithBorder($"{"Total:",-40}{totalAmount,20:C}"));
            receiptBuilder.AppendLine(new string('-', ReceiptWidth + 4));

            // Payment Method and Footer
            receiptBuilder.AppendLine(CenterTextWithBorder($"Payment Method: {paymentMethod}"));
            if (paymentMethod == "Card")
            {
                string cardDetails = _paymentProcessor.GenerateCardPaymentDetails();
                foreach (string line in cardDetails.Split(Environment.NewLine))
                {
                    receiptBuilder.AppendLine(CenterTextWithBorder(line));
                }
            }
            receiptBuilder.AppendLine(new string('-', ReceiptWidth + 4));
            receiptBuilder.AppendLine(CenterTextWithBorder("THANK YOU & WELCOME BACK!"));
            receiptBuilder.AppendLine(CenterTextWithBorder("Return policy only valid when receipt is present"));
            receiptBuilder.AppendLine(new string('=', ReceiptWidth + 4));

            // Final return statement
            return receiptBuilder.ToString();
        }

        private string CenterTextWithBorder(string text)
        {
            if (text.Length >= ReceiptWidth)
                return $"| {text} |";
            int padding = (ReceiptWidth - text.Length) / 2;
            string centeredText = new string(' ', padding) + text + new string(' ', ReceiptWidth - text.Length - padding);
            return $"| {centeredText} |";
        }
    }
}




