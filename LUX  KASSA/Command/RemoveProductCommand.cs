using Lux_Cash_Register.Command;
using Lux_Cash_Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUX__KASSA.Command
{
    public class RemoveProductCommand : IAdminCommand
    {
        private readonly List<Product> _products;
        private readonly ProductFileHandler _fileHandler;
        private readonly IErrorHandler _errorHandler;

        public RemoveProductCommand(List<Product> products, ProductFileHandler fileHandler, IErrorHandler errorHandler)
        {
            _products = products;
            _fileHandler = fileHandler;
            _errorHandler = errorHandler;
        }

        public void Execute()
        {
            Console.Clear();
            Console.WriteLine("REMOVE PRODUCT");

            string productId = PromptForInput("Enter the product ID to remove: ");

            // Debug: Print entered ID to verify correct input
            Console.WriteLine($" Entered Product ID: '{productId}'");

            // Check if the product ID was received as expected
            if (productId == "0")
            {
                Console.WriteLine(" Product ID '0' detected. Proceeding to search for it.");
            }

            var product = _products.Find(p => p.ProductId == productId);

            // Debug: Confirm if product was found
            if (product == null)
            {
                _errorHandler.ShowError("No product found with this ID.Press any key to return to the admin menu....");
                Console.ReadLine();
                return;
            }
            Console.WriteLine($" Found product with ID '{productId}' - Name: {product.Name}");

            // Confirm removal
            Console.WriteLine($"Are you sure you want to remove the product '{product.Name}' (ID: {product.ProductId})? (y/n): ");
            string? confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y")
            {
                // Remove the product from the list and save changes to file
                Console.WriteLine(" Confirmation received. Removing product from list.");
                _products.Remove(product);

                // Save to file and confirm save
                SaveProductsToFile();
                Console.WriteLine("Product removed successfully.");

                // Verify the product is no longer in the list
                if (_products.Any(p => p.ProductId == productId))
                {
                    Console.WriteLine(" Removal unsuccessful. Product still in list.");
                }
                else
                {
                    Console.WriteLine(" Product successfully removed from list.");
                }
            }
            else
            {
                Console.WriteLine("Product removal canceled.");
            }

            // Pause so the user can read the result message
            PauseBeforeReturning();
        }

        private string PromptForInput(string message)
        {
            Console.Write(message);
            string input = Console.ReadLine() ?? string.Empty;
            return input; // Directly return input to ensure "0" is valid
        }

        private void SaveProductsToFile()
        {
            Console.WriteLine(" Attempting to save updated product list to file...");
            _fileHandler.SaveProducts(_products); // Use the file handler to save the updated list
            Console.WriteLine(" Save operation complete.");
        }

        private void PauseBeforeReturning()
        {
            Console.WriteLine("Press any key to return to the admin menu...");
            Console.ReadKey(); // Pauses until a key is pressed
        }
    }
}

