using System;

namespace PWApplication.MobileShared.Validations
{
    public class IsDecimalGreaterThenNull : IValidationRule<decimal>
    {
        public string ValidationMessage { get; set; }

        public bool Check(decimal value)
        {
            if (value < 0 )
            {
                return false;
            }

            return true;
        }
    }
}
