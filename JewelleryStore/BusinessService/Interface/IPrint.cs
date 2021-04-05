using JewelleryStore.Models;

namespace JewelleryStore.BusinessService.Interface
{
    public interface IPrint
    {
        void Print(CalculatedPriceModel priceModel);
    }
}
