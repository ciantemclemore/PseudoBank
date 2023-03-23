using PseudoBank.Payments.DTOs;
using PseudoBank.Payments.Validators;
using System;
using System.Collections.Generic;

namespace PseudoBank.Payments.Factories
{
    /// <summary>
    /// The factory that produces payment validators
    /// </summary>
    public class PaymentValidatorFactory : IPaymentValidatorFactory
    {
        private IEnumerable<PaymentValidatorBase> Validators { get; set; }

        public PaymentValidatorFactory(IEnumerable<PaymentValidatorBase> validators)
        {
            Validators = validators;
        }

        public PaymentValidatorBase CreateValidator(string paymentScheme)
        {
            foreach (PaymentValidatorBase validator in Validators)
            {
                if (validator.ValidatorName == paymentScheme)
                {
                    return validator;
                }
            }

            throw new ArgumentException("Invalid payment scheme");
        }
    }
}
