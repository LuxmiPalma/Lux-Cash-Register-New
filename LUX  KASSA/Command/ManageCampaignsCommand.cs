using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LUX__KASSA;
using Lux_Cash_Register.Models;

namespace Lux_Cash_Register.Command
{
    public class ManageCampaignsCommand : IAdminCommand
    {
        private readonly List<Product> _products;
        private readonly SalesManager _salesManager;
        private readonly ProductFileHandler _fileHandler;
        private readonly IErrorHandler _errorHandler;


        public ManageCampaignsCommand(List<Product> products, SalesManager salesManager, ProductFileHandler fileHandler, IErrorHandler errorHandler)
        {
            _products = products;
            _salesManager = salesManager;
            _fileHandler = fileHandler;
            _errorHandler = errorHandler;
            LoadCampaignsFromFile(); // Load campaigns on startup
        }

        public void Execute()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Manage Campaigns");

                Console.Write("Enter the product ID for campaign management: ");
                string? productId = Console.ReadLine()?.Trim();
                Product? product = _products.Find(p => p.ProductId == productId);

                if (product == null)
                {
                    _errorHandler.ShowError("Product not found.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Product found.");

                Console.WriteLine("1. Add Campaign\n2. Remove Campaign");
                Console.Write("Enter your choice: ");
                string? choice = Console.ReadLine();

                if (choice == "1") AddCampaign(product);
                else if (choice == "2") RemoveCampaign(product);
                else _errorHandler.ShowError("Invalid choice.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                _errorHandler.LogError("Error executing ManageCampaignsCommand: " + ex.Message);
                _errorHandler.ShowError("An error occurred during campaign management.");
            }
        }

        private void AddCampaign(Product product)
        {
            try
            {
                // Initialize variables to avoid CS0165 error
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                bool validDates = false;

                // Step 1: Loop until valid, non-overlapping dates are entered
                while (!validDates)
                {
                    // Prompt user to enter dates and check their validity
                    if (!TryGetDate("Enter the campaign start date (yyyy-MM-dd): ", out startDate) ||
                        !TryGetDate("Enter the campaign end date (yyyy-MM-dd): ", out endDate) ||
                        startDate > endDate)
                    {
                        _errorHandler.ShowError("Invalid dates. End date must be later than start date.");
                        Console.ReadLine();
                        return;
                    }

                    // Check for existing campaigns in the specified date range
                    if (product.Campaigns.Any(c => c.StartDate <= endDate && c.EndDate >= startDate))
                    {
                        _errorHandler.ShowError("A campaign already exists in the specified date range. Please try a different date range.");
                        Console.ReadLine();

                    }
                    else
                    {
                        validDates = true; // Dates are valid and non-overlapping
                    }
                }

                // Step 2: Prompt user for campaign price
                Console.Write("Enter campaign price: ");
                if (!double.TryParse(Console.ReadLine(), out double campaignPrice))
                {
                    _errorHandler.ShowError("Invalid campaign price.");
                    return;
                }

                // Step 3: Add the new campaign if all inputs are valid
                product.AddCampaign(new Campaign(startDate, endDate, campaignPrice));
                Console.WriteLine("Campaign has been added.");

                // Step 4: Save updated campaigns to file
                _fileHandler.SaveCampaigns(_products);
            }
            catch (Exception ex)
            {
                _errorHandler.LogError("Error adding campaign: " + ex.Message);
                _errorHandler.ShowError("An error occurred while adding the campaign.");
            }
        }
        private void RemoveCampaign(Product product)
        {
            try
            {
                if (product == null || product.Campaigns == null || product.Campaigns.Count == 0)
                {
                    _errorHandler.ShowError("No active campaigns for this product.");
                    Console.ReadLine();
                    return;
                }

                // Display the most recent campaign details to confirm removal
                var lastCampaign = product.Campaigns.LastOrDefault();
                if (lastCampaign != null)
                {
                    Console.WriteLine($"The most recent campaign: {lastCampaign.StartDate:yyyy-MM-dd} to {lastCampaign.EndDate:yyyy-MM-dd} - Price: {lastCampaign.CampaignPrice}");
                    Console.Write("Do you want to permanently remove this campaign? (y/n): ");

                    // Check if user confirms removal
                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        // Remove the campaign from the in-memory list
                        product.RemoveCampaign(lastCampaign);

                        // Save the updated campaigns to the file to make the removal permanent
                        _fileHandler.SaveCampaigns(_products);

                        Console.WriteLine("Campaign has been permanently removed.");

                        // Update any dependent components, like the SalesManager
                        _salesManager?.UpdateProductInSales(product);
                    }
                    else
                    {
                        Console.WriteLine("Campaign removal cancelled.");
                    }
                }
                else
                {
                    _errorHandler.ShowError("No recent campaign found to remove.");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError("Error removing campaign: " + ex.Message);
                _errorHandler.ShowError("An error occurred while removing the campaign.");
            }
        }
        private bool TryGetDate(string message, out DateTime date)
        {
            Console.Write(message);
            bool result = DateTime.TryParse(Console.ReadLine(), out date);
            Console.WriteLine(result ? $"Date entered: {date}" : "Invalid date format.");
            return result;
        }
        // Private method to check if a campaign is active
        private bool IsCampaignActive(Campaign campaign)
        {
            DateTime currentDate = DateTime.Now;
            return currentDate >= campaign.StartDate && currentDate <= campaign.EndDate;
        }
        private void SaveCampaignsToFile()
        {
            _fileHandler.SaveProducts(_products);
        }
        // Method to load campaigns from file into product campaigns
        private void LoadCampaignsFromFile()
        {
            try
            {
                Console.WriteLine("Loading campaigns from file...");
                _fileHandler.LoadProducts();

            }
            catch (Exception ex)
            {
                _errorHandler.LogError("Error loading campaigns from file: " + ex.Message);
                _errorHandler.ShowError("Failed to load campaigns from file.");
            }
        }
    }
}