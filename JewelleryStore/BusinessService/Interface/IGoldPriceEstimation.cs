using JewelleryStore.Models;

namespace JewelleryStore.BusinessService.Interface
{
    public interface IGoldPriceEstimation
    {
        decimal CalculateTotalPrice(PriceEstimationModel priceEstimationModel, bool eligibleForDiscount);
    }
}
