using JewelleryStore.BusinessService.Interface;
using JewelleryStore.Enum;

namespace JewelleryStore.Delegates
{
    public class Delegates
    {
        public delegate IPrint ServiceResolver(PrintOptions printOption);
    }
}
