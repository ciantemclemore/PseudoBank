using PseudoBank.Payments.DTOs;

namespace PseudoBank.Payments.Validators
{
    /// <summary>
    /// The base/shared data between all payment validators
    /// </summary>
    public abstract class PaymentValidatorBase
    {
        public string ErrorMessage { get; private set; } = string.Empty;

        public abstract string ValidatorName { get; }

        public PaymentValidatorBase()
        {
        }

        public virtual bool IsValidPayment(decimal requestAmount, decimal debtorBalance, string debtorAccountStatus, string creditorAccountStatus)
        {
            /* Based on the original code base it seems that checking the balance or account status only happened for 
               expedited or automated payments. However, I assume this needs to take place for all accounts? */

            if (debtorAccountStatus == AccountStatusConstants.InboundPaymentsOnly)
            {
                ErrorMessage = "Inbound only accounts cannot make payments";
                return false;
            }

            if (debtorAccountStatus == AccountStatusConstants.Disabled || creditorAccountStatus == AccountStatusConstants.Disabled)
            {
                ErrorMessage = "Account must be active to make payment";
                return false;
            }

            if (debtorBalance < requestAmount)
            {
                ErrorMessage = "Account balance has insufficient funds to make payment";
                return false;
            }

            if (requestAmount < 0)
            {
                ErrorMessage = "Invalid payment amount";
                return false;
            }

            return true;
        }
    }
}
