using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lux_Cash_Register.Models;

namespace Lux_Cash_Register.Command
{
    public class UpdateProductCommand : IAdminCommand
    {
        private readonly List<Product> _products;
        private readonly ProductFileHandler _fileHandler;

        public UpdateProductCommand(List<Product> products, ProductFileHandler fileHandler)
        {
            _products = products;
            _fileHandler = fileHandler;
        }
        public void Execute()
        {
            Console.Clear();
            Console.Write("Enter the product ID to update: ");
            string? productId = Console.ReadLine();

            // Debug: Print the entered ID to ensure "0" is accepted
            Console.WriteLine($"Debug: Entered Product ID: {productId}");

            var product = _products.Find(p => p.ProductId == productId);

            if (product == null)
            {
                Console.WriteLine("Product not found.Enter for returning to main menu");
                Console.ReadKey(); // Pause to show the error message
                return;
            }

            // Update Product ID
            Console.Write("Enter new ID for the product (or press Enter to keep the current ID): ");
            string newId = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(newId))
            {
                // Check if the new ID already exists in the product list
                if (_products.Any(p => p.ProductId == newId))
                {
                    Console.WriteLine("A product with this ID already exists. Update canceled.");
                    return;
                }
                product.ProductId = newId;
            }


            // Update Product Name
            Console.Write("Enter new name for the product (or press Enter to keep the current name): ");
            string newName = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(newName))
            {
                product.Name = newName;
            }

            // Update Product Price
            Console.Write("Enter new price for the product (or press Enter to keep the current price): ");
            string? priceInput = Console.ReadLine();
            if (double.TryParse(priceInput, out double newPrice))
            {
                product.Price = newPrice;
            }
            else if (!string.IsNullOrWhiteSpace(priceInput))
            {
                Console.WriteLine("Invalid price. Update canceled.");
                return;
            }
            // Update Unit Type
            Console.Write("Enter new unit for the product (e.g., 'kg', 'piece') (or press Enter to keep the current unit): ");
            string newUnit = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(newUnit))
            {
                product.PriceType = newUnit;
            }

            // Update Category
            Console.Write("Enter new category for the product (e.g., 'fruit', 'dairy') (or press Enter to keep the current category): ");
            string newCategory = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(newCategory))
            {
                product.Category = newCategory;
            }

            Console.WriteLine("Product has been updated.");

            // Save changes to the file
            SaveProductsToFile();
        }

        private void SaveProductsToFile()
        {
            _fileHandler.SaveProducts(_products); // Use file handler to save products

        }
    }
}


