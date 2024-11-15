using LUX__KASSA.Manager;
using Lux_Cash_Register.Command;
using Lux_Cash_Register.Manager;
using Lux_Cash_Register.Models;
using System;
using System.Collections.Generic;

namespace LUX_Cash_Register
{
    public class Program
    {
        public static void Main()
        {
            var sessionManager = new SessionManager();
            var productManager = new ProductManager(sessionManager.Products);
            var receiptManager = new ReceiptManager();
            var salesManager = new SalesManager(sessionManager.Products, receiptManager, sessionManager.ErrorHandler);
            var admin = new Admin(sessionManager.Products, salesManager, sessionManager.ProductFileHandler, sessionManager.ErrorHandler);

            var menuManager = new MenuManager(salesManager, admin, productManager);
            menuManager.DisplayMainMenu();
        }


    }



}
