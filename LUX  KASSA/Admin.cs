using LUX__KASSA;
using LUX__KASSA.Command;
using Lux_Cash_Register.Command;
using Lux_Cash_Register.Models;
using System;
using System.Collections.Generic;

namespace Lux_Cash_Register.Command
{
    public class Admin
    {
        private readonly List<Product> _products; // Reference to the main product list
        private readonly SalesManager _salesManager; // Reference to SalesManager
        private readonly ProductFileHandler _fileHandler; // Reference to ProductFileHandler
        private readonly IErrorHandler _errorHandler;

        public Admin(List<Product> products, SalesManager salesManager, ProductFileHandler fileHandler, IErrorHandler errorHandler)
        {
            _products = products;
            _salesManager = salesManager;
            _fileHandler = fileHandler; // Initialize file handler
            _errorHandler = errorHandler;

        }
        public bool Login()
        {
            try
            {
                Console.Write("Enter password: ");
                string? password = Console.ReadLine();

                if (password == "admin") // Simplified password check for illustration
                {
                    return true;
                }
                else
                {
                    _errorHandler.ShowError("Incorrect password.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex.Message); // Log unexpected errors
                _errorHandler.ShowError("An error occurred during login.");
                return false;
            }
        }
        public void ShowAdminMenu()
        {

            try
            {
                while (true) // Keep the admin menu open until the user chooses to exit
                {
                    Console.Clear();
                    Console.WriteLine("ADMIN TOOLS");
                    Console.WriteLine("1. Update product name and price");
                    Console.WriteLine("2. Add new product");
                    Console.WriteLine("3. Remove a product");
                    Console.WriteLine("4. Manage campaigns");
                    Console.WriteLine("0. Back to main menu");
                    Console.Write("Enter your choice: ");
                    string choice = Console.ReadLine() ?? string.Empty;

                    if (choice == "1")
                    {
                        try
                        {
                            var updateProductCommand = new UpdateProductCommand(_products, _fileHandler);
                            updateProductCommand.Execute();
                        }
                        catch (Exception ex)
                        {
                            _errorHandler.LogError(ex.Message); // Log error if the command fails
                            _errorHandler.ShowError("Failed to update product.");
                        }
                    }
                    else if (choice == "2")
                    {
                        try
                        {
                            var addNewProductCommand = new AddNewProductCommand(_products, _fileHandler, _errorHandler);
                            addNewProductCommand.Execute();
                        }
                        catch (Exception ex)
                        {
                            _errorHandler.LogError(ex.Message); // Log error if the command fails
                            _errorHandler.ShowError("Failed to add new product.");
                        }
                    }
                    else if (choice == "3") // Call the new remove command
                    {
                        try
                        {
                            var removeProductCommand = new RemoveProductCommand(_products, _fileHandler, _errorHandler);
                            removeProductCommand.Execute();
                        }
                        catch (Exception ex)
                        {
                            _errorHandler.LogError(ex.Message); // Log error if the command fails
                            _errorHandler.ShowError("Failed to remove product.");
                        }
                    }
                    else if (choice == "4")
                    {
                        try
                        {
                            var manageCampaignsCommand = new ManageCampaignsCommand(_products, _salesManager, _fileHandler, _errorHandler);
                            manageCampaignsCommand.Execute();
                        }
                        catch (Exception ex)
                        {
                            _errorHandler.LogError(ex.Message); // Log error if the command fails
                            _errorHandler.ShowError("Failed to manage campaigns.");
                        }
                    }

                    else if (choice == "0")
                    {
                        Console.WriteLine("Returning to the main menu...");
                        break;
                    }
                    else
                    {
                        _errorHandler.ShowError("Invalid choice. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError("An error occurred in the admin menu: " + ex.Message);
                _errorHandler.ShowError("An error occurred in the admin menu.");
            }
        }
    }
}
