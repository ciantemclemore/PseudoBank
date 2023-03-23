using PseudoBank.Payments.DTOs;

namespace PseudoBank.Payments.Validators
{
    /// <summary>
    /// Validator for bank to bank transfer payments
    /// </summary>
    public class BankToBankTransferValidator : PaymentValidatorBase
    {
        public override string ValidatorName => "BankToBankTransfer";

        public BankToBankTransferValidator() : base()
        {
        }
        
        // Can override the base 'IsValidPayment' if any additional work needs to be done for this specific payment scheme
    }
}
