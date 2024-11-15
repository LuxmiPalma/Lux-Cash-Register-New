using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lux_Cash_Register.Models
{
    public class Campaign
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double CampaignPrice { get; set; }

        public Campaign(DateTime startDate, DateTime endDate, double campaignPrice)
        {
            StartDate = startDate;
            EndDate = endDate;
            CampaignPrice = campaignPrice;
        }

        public bool IsActive(DateTime date)
        {
            return date >= StartDate && date <= EndDate;
        }
    }
}
