using System;
using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Binance.Net.Objects.Models.Spot.Lending
{
    /// <summary>
    /// Savings product
    /// </summary>
    public class BinanceFlexibleProduct
    {
        /// <summary>
        /// The asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Average annual interest rage
        /// </summary>
        [JsonProperty("latestAnnualPercentageRate")]
        public decimal LatestAnnualInterestRate { get; set; }

        public Dictionary<string, decimal> TierAnnualPercentageRate { get; set; }

        public decimal AirDropPercentageRate { get; set; }
        /// <summary>
        /// Can purchase
        /// </summary>
        public bool CanPurchase { get; set; }
        /// <summary>
        /// Can redeem
        /// </summary>
        public bool CanRedeem { get; set; }

        /// <summary>
        /// Is sold out
        /// </summary>
        public bool IsSoldOut { get; set; }

        public bool Hot { get; set; }
        /// <summary>
        /// Minimal quantity to purchase
        /// </summary>
        [JsonProperty("minPurchaseAmount")]
        public decimal MinimalPurchaseAmount { get; set; }
        /// <summary>
        /// Product id
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime SubscriptionStartTime { get; set; }

        /// <summary>
        /// Status of the product
        /// </summary>
        public string Status { get; set; } = string.Empty;
        
    }

    public class BinanceFlexibleProductList
    {
        public int Total { get; set; }
        public List<BinanceFlexibleProduct> Rows { get; set; } = new List<BinanceFlexibleProduct>();
    }
}
