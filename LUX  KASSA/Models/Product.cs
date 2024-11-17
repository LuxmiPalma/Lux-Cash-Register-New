using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lux_Cash_Register.Models
{
    public class Product
    {

        public string ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; } // Base price without any discounts
        public string PriceType { get; set; } // "per unit" eller "per kg"
        public string Category { get; set; }
        public List<Campaign> Campaigns { get; private set; } = new List<Campaign>(); // Lista over campa

        // Add the CurrentDiscount property
        public double CurrentDiscount { get; set; } = 0;


        public Product(string productId, string name, double price, string priceType, string category)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            PriceType = priceType;
            Category = category;
        }
        public void AddCampaign(Campaign campaign)
        {
            Campaigns.Add(campaign);
            UpdateCurrentDiscount();

        }
        // Method to update CurrentDiscount based on the latest campaign
        public void UpdateCurrentDiscount()
        {
            var activeCampaign = Campaigns.LastOrDefault(c => c.IsActive(DateTime.Now));
            if (activeCampaign != null && activeCampaign.CampaignPrice > 0)
            {
                CurrentDiscount = activeCampaign.CampaignPrice;
            }
            else
            {
                CurrentDiscount = Price; // Set to original price if no active campaign
            }
        }

        // Add and Remove campaign methods can call UpdateCurrentDiscount

        public void RemoveCampaign(Campaign campaign)
        {
            Campaigns.Remove(campaign);
            UpdateCurrentDiscount(); // Update discount after removing a campaign

        }

        public double GetEffectivePrice(DateTime date)
        {
            var activeCampaign = Campaigns.LastOrDefault(c => c.IsActive(date));
            if (activeCampaign != null && activeCampaign.CampaignPrice > 0)
            {
                return activeCampaign.CampaignPrice; // Use campaign price if active
            }

            return Price; // if no campaign is active return to orginal price

        }
    }
}