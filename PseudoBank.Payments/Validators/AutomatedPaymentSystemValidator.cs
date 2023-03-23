using PseudoBank.Payments.DTOs;

namespace PseudoBank.Payments.Validators
{
    /// <summary>
    /// Validator for automated payments
    /// </summary>
    public class AutomatedPaymentSystemValidator : PaymentValidatorBase
    {
        public override string ValidatorName => "AutomatedPaymentSystem";

        public AutomatedPaymentSystemValidator() : base()
        {
        }

        // Can override the base 'IsValidPayment' if any additional work needs to be done for this specific payment scheme
    }
}
