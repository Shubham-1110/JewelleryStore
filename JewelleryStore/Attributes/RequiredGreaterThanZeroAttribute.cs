using System.ComponentModel.DataAnnotations;

namespace JewelleryStore.Attributes
{
    public class RequiredGreaterThanZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            decimal decimalValue;
            return value != null && decimal.TryParse(value.ToString(), out decimalValue) && decimalValue > 0;
        }
    }
}
