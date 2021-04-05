using JewelleryStore.BusinessService.Interface;
using JewelleryStore.Models;

namespace JewelleryStore.BusinessService.Implementation
{
    public class GoldPriceEstimation : IGoldPriceEstimation
    {
        public decimal CalculateTotalPrice(PriceEstimationModel priceEstimationModel, bool eligibleForDiscount)
        {
            if (!eligibleForDiscount)
            {
                priceEstimationModel.Discount = null;
            }

            var totalPrice = priceEstimationModel.GoldPrice * priceEstimationModel.Weight;
            var netPrice = priceEstimationModel.Discount != null && priceEstimationModel.Discount.Value > 0 ? totalPrice - ((priceEstimationModel.Discount / 100) * totalPrice) : totalPrice;
            return netPrice.Value;
        }
    }
}
