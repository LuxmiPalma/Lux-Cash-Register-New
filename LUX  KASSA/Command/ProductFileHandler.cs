using LUX__KASSA;
using Lux_Cash_Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lux_Cash_Register.Command
{
    public class ProductFileHandler
    {
        private readonly string _productFilePath;
        private readonly string _campaignFilePath;
        private readonly IErrorHandler _errorHandler;

        public ProductFileHandler(string productFilePath, string campaignFilePath, IErrorHandler errorHandler)
        {
            _productFilePath = productFilePath;
            _campaignFilePath = campaignFilePath;
            _errorHandler = errorHandler;
        }

        // Method to load products from file
        public List<Product> LoadProducts()
        {
            var products = new List<Product>();
            var productIds = new HashSet<string>();

            try
            {
                if (!File.Exists(_productFilePath))
                {
                    _errorHandler.ShowError("Product file does not exist.");
                    return products;
                }

                foreach (var line in File.ReadLines(_productFilePath))
                {
                    var parts = line.Split(',');

                    // Validate the format: ensure there are exactly 5 parts and a valid numeric product ID
                    if (parts.Length == 5 && !string.IsNullOrWhiteSpace(parts[0]) && parts[0].All(char.IsDigit))
                    {
                        string productId = parts[0];
                        string name = parts[1];
                        if (!double.TryParse(parts[2], out double price))
                        {
                            Console.WriteLine($"Debug: Skipping invalid price entry for product ID '{productId}'");
                            continue; // Skip if price is invalid
                        }
                        string priceType = parts[3];
                        string category = parts[4];

                        // Ensure the product ID is unique
                        if (!productIds.Contains(productId))
                        {
                            products.Add(new Product(productId, name, price, priceType, category));
                            productIds.Add(productId);

                        }
                    }
                    else
                    {
                        Console.WriteLine($"Skipping invalid entry '{line}'");
                        _errorHandler.ShowError($"Invalid product entry found and skipped: {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError("Error loading products from file: " + ex.Message);
                _errorHandler.ShowError("An error occurred while loading products.");
            }

            return products;
        }

        // Save unique products to file
        public void SaveProducts(List<Product> products)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_productFilePath, false))
                {
                    foreach (var product in products)
                    {


                        writer.WriteLine($"{product.ProductId},{product.Name},{product.Price},{product.PriceType},{product.Category}");
                    }
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError("Error saving products to file: " + ex.Message);
                _errorHandler.ShowError("An error occurred while saving products.");
            }
        }

        // Method to save active campaigns only
        public void SaveCampaigns(List<Product> products)
        {
            try
            {
                var uniqueCampaigns = new HashSet<string>();

                using (StreamWriter writer = new StreamWriter(_campaignFilePath, false))
                {
                    foreach (var product in products)
                    {
                        foreach (var campaign in product.Campaigns)
                        {
                            // Create a unique string representation of the campaign
                            string campaignKey = $"{product.ProductId},{campaign.StartDate:yyyy-MM-dd},{campaign.EndDate:yyyy-MM-dd},{campaign.CampaignPrice}";

                            // Only write the campaign if it hasn't been written before
                            if (uniqueCampaigns.Add(campaignKey))
                            {
                                writer.WriteLine(campaignKey);

                                // Debug: Print to console for verification
                                Console.WriteLine($"--Saved campaign: {campaignKey}--");
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError("Error saving campaigns to file: " + ex.Message);
                _errorHandler.ShowError("An error occurred while saving campaigns.");
            }
        }
        // Method to load campaigns from file and associate with products
        public void LoadCampaigns(List<Product> products)
        {
            if (!File.Exists(_campaignFilePath)) return;

            try
            {
                var lines = File.ReadAllLines(_campaignFilePath);
                foreach (var line in lines)
                {
                    var data = line.Split(',');
                    if (data.Length >= 4)
                    {
                        string productId = data[0];
                        DateTime startDate = DateTime.Parse(data[1]);
                        DateTime endDate = DateTime.Parse(data[2]);
                        double campaignPrice = double.Parse(data[3]);

                        // Find the corresponding product and add the campaign
                        var product = products.Find(p => p.ProductId == productId);
                        if (product != null)
                        {
                            var campaign = new Campaign(startDate, endDate, campaignPrice);
                            product.AddCampaign(campaign);
                        }
                    }
                }
                // Update discounts for each product to apply active campaigns
                foreach (var product in products)
                {
                    product.UpdateCurrentDiscount();
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError($"Error loading campaigns: {ex.Message}");
            }
        }
        // Check if a campaign is active based on the current date
        public bool IsCampaignActive(Campaign campaign)
        {
            DateTime currentDate = DateTime.Now;
            return currentDate >= campaign.StartDate && currentDate <= campaign.EndDate;
        }
    }
}