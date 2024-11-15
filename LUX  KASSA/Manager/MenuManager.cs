using Lux_Cash_Register.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUX__KASSA.Manager
{
    public class MenuManager
    {
        private readonly SalesManager _salesManager;
        private readonly Admin _admin;
        private readonly ProductManager _productManager;

        public MenuManager(SalesManager salesManager, Admin admin, ProductManager productManager)
        {
            _salesManager = salesManager;
            _admin = admin;
            _productManager = productManager;
        }

        public void DisplayMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("CASH REGISTER");
                Console.WriteLine("1. New Customer");
                Console.WriteLine("2. Admin Tools");
                Console.WriteLine("3. Product List");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _salesManager.StartNewSale();
                        break;
                    case "2":
                        if (_admin.Login()) _admin.ShowAdminMenu();
                        else ShowReturnMessage("Incorrect password.");
                        break;
                    case "3":
                        _productManager.ViewProductsByCategory();
                        break;
                    case "0":
                        Console.WriteLine("Exiting the program.");
                        return;
                    default:
                        ShowReturnMessage("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void ShowReturnMessage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
        }
    }
}

