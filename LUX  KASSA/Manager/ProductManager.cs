using Lux_Cash_Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUX__KASSA.Manager
{
    public class ProductManager
    {
        private readonly List<Product> _products;

        public ProductManager(List<Product> products)
        {
            _products = products;
        }

        public void ViewProductsByCategory()
        {
            var categories = _products.Select(p => p.Category).Distinct().ToList();

            if (categories.Count == 0)
            {
                Console.WriteLine("No categories available. Please add products with categories first.");
                return;
            }
            Console.Clear();
            Console.WriteLine("Choose a Category To View:");
            for (int i = 0; i < categories.Count; i++)
                Console.WriteLine($"{i + 1}. {categories[i]}");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= categories.Count)
            {
                string selectedCategory = categories[choice - 1];
                Console.WriteLine($"\nProducts in category: {selectedCategory}\n");

                foreach (var product in _products.Where(p => p.Category == selectedCategory))
                {
                    Console.WriteLine($"Name: {product.Name}");
                    Console.WriteLine($"Product ID: {product.ProductId}");
                    Console.WriteLine($"Price: {product.Price} kr");
                    Console.WriteLine($"Unit: {product.PriceType}\n");
                }
            }
            else
                Console.WriteLine("Invalid choice. Please try again.");

            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
        }
    }
}
