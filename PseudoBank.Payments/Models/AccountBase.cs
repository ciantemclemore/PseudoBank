namespace PseudoBank.Payments.Models
{
    public class AccountBase
    {
        public string AccountNumber { get; set; } = string.Empty;

        public decimal Balance { get; set; }
    }
}
