using PseudoBank.Payments.Models;
using System;

namespace PseudoBank.Payments.DTOs
{
    /// <summary>
    /// The base/shared data between all payment request
    /// </summary>
    public abstract class MakePaymentRequestBase
    {
        public string CreditorAccountNumber { get; set; } = string.Empty;

        public string DebtorAccountNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentSchemeBase PaymentScheme { get; set; } = new();
    }
}
