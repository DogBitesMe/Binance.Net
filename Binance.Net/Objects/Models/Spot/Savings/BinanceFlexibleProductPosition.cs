using Newtonsoft.Json;
using System.Collections.Generic;

namespace Binance.Net.Objects.Models.Spot.Lending
{
    /// <summary>
    /// Flexible product position
    /// </summary>
    public class BinanceFlexibleProductPosition
    {
        /// <summary>
        /// Total quantity
        /// </summary>
        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        public Dictionary<string, decimal> TierAnnualPercentageRate { get; set; } = new Dictionary<string, decimal>();

        public decimal LatestAnnualPercentageRate { get; set; }

        public decimal YesterdayAirdropPercentageRate { get; set; }

        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        public string AirDropAsset { get; set; } = string.Empty;

        /// <summary>
        /// Can redeem
        /// </summary>
        public bool CanRedeem { get; set; }

        public decimal CollateralAmount { get; set; }

        /// <summary>
        /// The product id
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        public decimal YesterdayRealTimeRewards { get; set; }

        public decimal CumulativeBonusRewards { get; set; }
        public decimal CumulativeRealTimeRewards { get; set; }
        public decimal CumulativeTotalRewards { get; set; }
        public bool AutoSubscribe { get; set; }
    }

    public class BinanceFlexibleProductPositionList
    {
        public int Total { get; set; }
        public List<BinanceFlexibleProductPosition> Rows { get; set; } = new List<BinanceFlexibleProductPosition>();
    }
}
