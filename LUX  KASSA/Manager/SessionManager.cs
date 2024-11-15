using Lux_Cash_Register.Command;
using Lux_Cash_Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUX__KASSA.Manager
{
    public class SessionManager
    {
        public IErrorHandler ErrorHandler { get; private set; }
        public ProductFileHandler ProductFileHandler { get; private set; }
        public List<Product> Products { get; private set; } = new List<Product>();

        public SessionManager()
        {
            ErrorHandler = new ErrorHandler("../../../Logs/ErrorLog.txt");
            ProductFileHandler = new ProductFileHandler("../../../Receipt/products.txt", "../../../Receipt/campaigns.txt", ErrorHandler);

            // Load products and campaigns
            Products = ProductFileHandler.LoadProducts();
            ProductFileHandler.LoadCampaigns(Products);
        }


    }
}
