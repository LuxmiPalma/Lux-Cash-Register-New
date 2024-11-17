using System;
using System.Collections.Generic;
using LUX__KASSA;
using Lux_Cash_Register.Manager;
using Lux_Cash_Register.Models;



public class SalesManager
{
    private readonly List<Product> products;
    private readonly ReceiptManager receiptManager;
    private readonly List<(Product, double)> cart;
    private readonly IErrorHandler errorHandler;
    private int receiptNumber;

    public SalesManager(List<Product> products, ReceiptManager receiptManager, IErrorHandler errorHandler)
    {
        this.products = products;
        this.receiptManager = receiptManager;
        this.cart = new List<(Product, double)>();
        this.errorHandler = errorHandler;
        receiptNumber = LoadLatestReceiptNumber();
    }

    public void StartNewSale()
    {
        cart.Clear();
        Console.WriteLine("\nNew sale started. Enter <productID> <quantity> or 'PAY' to finish and pay.");
        Console.WriteLine("Type 'CANCEL' to cancel and return to the main menu.");

        while (true)
        {
            Console.Write("Command: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                errorHandler.ShowError("Input cannot be empty. Please enter a valid command.");
                continue;
            }

            input = input.ToUpper();

            if (input == "CANCEL")
            {
                Console.WriteLine("Sale canceled. Returning to the main menu...");
                return;
            }
            else if (input == "PAY")
            {
                ProcessPayment();
                break;
            }
            else
            {
                ProcessProductCommand(input);
            }

        }
    }
    private void ProcessPayment()
    {
        Console.WriteLine("Processing payment...");
        receiptManager.PrintAndSaveReceipt(cart); // New method to print and save receipt
    }

    public void ProcessProductCommand(string input)
    {
        var parts = input.Split(' ');

        if (parts.Length == 2 && double.TryParse(parts[1], out double quantity))
        {
            string productId = parts[0];
            Product? product = products.Find(p => p.ProductId == productId);

            if (product != null)
            {
                // Use GetEffectivePrice to get either the campaign price or base price
                double price = product.GetEffectivePrice(DateTime.Now);

                cart.Add((product, quantity));
                Console.WriteLine($"{quantity} of {product.Name} has been added to the cart at {price:C} each.");
            }
            else
            {
                errorHandler.ShowError("Product not found.");
                errorHandler.LogError("Product not found.");
            }
        }
        else
        {
            errorHandler.ShowError("Invalid command. Use the format '<productID> <quantity>' or 'PAY' to pay.");
        }
    }
    private int CalculateTotalSale()
    {

        int total = 0;
        DateTime currentDate = DateTime.Now;


        foreach (var (product, quantity) in cart)
        {
            // Check if the product has an active discount; if not, use base price
            double price = product.CurrentDiscount > 0 ? product.CurrentDiscount : product.Price;
            total += (int)(price * quantity);
        }
        return (int)total;
    }

    // Update the product in the cart if its campaign or discount changes
    public void UpdateProductInSales(Product product)
    {
        // Reset discount if there's no active campaign for the product
        if (product.Campaigns == null || product.Campaigns.Count == 0)
        {
            product.CurrentDiscount = 0;
        }

        // If the product is in the cart, update its discount immediately
        for (int i = 0; i < cart.Count; i++)
        {
            if (cart[i].Item1.ProductId == product.ProductId)
            {
                cart[i] = (product, cart[i].Item2); // Update cart entry with the latest product data
            }
        }
    }

    // Save receipt to a file with date and receipt number in the filename
    private void SaveReceipt(List<(Product, double)> items, int totalAmount, int receiptNumber, string date)
    {
        string filePath = "../../../Receipt/Save Receipt";

        Directory.CreateDirectory(filePath); // Ensure directory exists
        string fileName = Path.Combine(filePath, $"Receipt-{date}-#{receiptNumber}.txt");

        using (StreamWriter writer = new StreamWriter(fileName, append: false))
        {
            writer.WriteLine($"Receipt #{receiptNumber}");
            writer.WriteLine($"Date: {date}");
            writer.WriteLine(new string('-', 30));

            foreach (var (product, quantity) in items)
            {
                double itemTotal = product.Price * quantity;
                writer.WriteLine($"{product.Name,-10} x{quantity} @ {product.Price:C} = {itemTotal:C}");
            }

            writer.WriteLine(new string('-', 30));
            writer.WriteLine($"Total Amount: {totalAmount:C}");
        }
    }

    // Load the latest receipt number from a file to ensure continuity
    private int LoadLatestReceiptNumber()
    {
        string filePath = "../../../Receipt/latest_receipt_number.txt";

        if (File.Exists(filePath) && int.TryParse(File.ReadAllText(filePath), out int number))
        {
            return number;
        }
        return 0; // Start from 0 if no file exists
    }

    // Save the latest receipt number to a file
    private void SaveLatestReceiptNumber(int number)
    {
        string filePath = "../../../Receipt/latest_receipt_number.txt";
        File.WriteAllText(filePath, number.ToString());
    }
}


