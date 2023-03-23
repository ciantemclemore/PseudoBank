using PseudoBank.Payments.DTOs;

namespace PseudoBank.Payments.Validators
{
    /// <summary>
    /// Validator for expedited payments
    /// </summary>
    public class ExpeditedPaymentsValidator : PaymentValidatorBase
    {
        public override string ValidatorName => "ExpeditedPayments";

        public ExpeditedPaymentsValidator() : base()
        {
        }

        // Can override the base 'IsValidPayment' if any additional work needs to be done for this specific payment scheme
    }
}
