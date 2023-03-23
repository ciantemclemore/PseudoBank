using PseudoBank.Payments.DTOs;
using PseudoBank.Payments.Validators;

namespace PseudoBank.Payments.Factories
{
    public interface IPaymentValidatorFactory
    {
        PaymentValidatorBase CreateValidator(string paymentScheme);
    }
}
