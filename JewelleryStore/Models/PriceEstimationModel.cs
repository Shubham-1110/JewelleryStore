using JewelleryStore.Attributes;
using JewelleryStore.Common;
using System.Runtime.Serialization;

namespace JewelleryStore.Models
{
    public class PriceEstimationModel
    {
        [RequiredGreaterThanZero(ErrorMessage = ErrorMessageConstants.InvalidGoldPrice)]
        public decimal? GoldPrice { get; set; }

        [RequiredGreaterThanZero(ErrorMessage = ErrorMessageConstants.InvalidWeight)]
        public decimal? Weight { get; set; }

        public decimal? Discount { get; set; } = 2;
    }
}
