namespace PseudoBank.Payments.DTOs
{
    /// <summary>
    /// Describes the status after a payment request
    /// </summary>
    public class MakePaymentResponse
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
