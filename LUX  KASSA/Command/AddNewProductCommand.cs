using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LUX__KASSA;
using Lux_Cash_Register.Models;

namespace Lux_Cash_Register.Command
{
    public class AddNewProductCommand : IAdminCommand
    {
        private List<Product> _products;
        private readonly ProductFileHandler _fileHandler;
        private readonly IErrorHandler _errorHandler;



        public AddNewProductCommand(List<Product> products, ProductFileHandler fileHandler, IErrorHandler errorHandler)
        {
            _fileHandler = fileHandler;
            _errorHandler = errorHandler;

            _products = products;

            // Only load products if the list is empty to prevent duplicates
            if (products.Count == 0)
            {
                try
                {
                    _products = _fileHandler.LoadProducts();
                    products.AddRange(_products); // Initialize the passed list if empty
                    Console.WriteLine("Loaded Products in AddNewProductCommand:");
                    foreach (var product in _products)
                    {
                        Console.WriteLine($"ID: {product.ProductId}, Name: {product.Name}");
                    }
                }
                catch (Exception ex)
                {
                    _errorHandler.LogError($"Error loading products: {ex.Message}");
                    _errorHandler.ShowError("An error occurred while loading products.");
                }
            }
            else
            {
                _products = products; // Reference to already loaded products
            }
        }

        public void Execute()
        {
            while (true)
            {
                Console.Clear();
                string productId = PromptForInput("Enter new product ID: ");
                Console.WriteLine($"Entered Product ID: {productId}"); // Debugging: Display entered product ID

                if (_products.Exists(p => p.ProductId == productId))
                {
                    _errorHandler.ShowError("A product with this ID already exists.");
                    Console.ReadLine();
                    continue;
                }

                if (string.IsNullOrEmpty(productId))
                {
                    _errorHandler.ShowError("Product ID cannot be empty.");
                    continue;
                }


                Console.Write("Enter product name: ");
                string name = Console.ReadLine() ?? string.Empty;

                // Check for Duplicate Product Name
                if (_products.Exists(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Debug: Duplicate product ID detected"); // Verify if this block is reached
                    _errorHandler.ShowError("A product with this name/Id already exists.");
                    Console.ReadLine();
                    continue;
                }

                Console.Write("Enter product price: ");
                if (!double.TryParse(Console.ReadLine(), out double price))
                {
                    _errorHandler.ShowError("Invalid price.");
                    Console.ReadLine();
                    continue;
                }

                Console.Write("Enter price type (e.g., 'kg', 'Piece'): ");
                string priceType = Console.ReadLine() ?? string.Empty;

                Console.Write("Enter Category (e.g., fruit, meat, dairy): ");
                string category = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(category))
                {
                    Console.WriteLine("Category cannot be empty. Setting to 'Uncategorized'.");
                    category = "Uncategorized";

                }


                var newProduct = new Product(productId, name, price, priceType, category);
                _products.Add(newProduct);

                SaveProductsToFile(); // Save once at the end of adding product
                Console.WriteLine("Product added successfully.");
                Console.ReadLine();
                break;
            }
        }

        private string PromptForInput(string message, string defaultValue = "")
        {
            Console.Write(message);
            string input = Console.ReadLine() ?? defaultValue;
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }

        private double PromptForPrice(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (double.TryParse(input, out double price)) return price;

            _errorHandler.ShowError("Invalid price format.");
            return -1; // Invalid price
        }

        private void SaveProductsToFile()
        {
            try
            {
                _fileHandler.SaveProducts(_products);
            }
            catch (Exception ex)
            {
                _errorHandler.LogError($"Error saving products to file: {ex.Message}");
                _errorHandler.ShowError("An error occurred while saving products.");
            }
        }




    }
}
