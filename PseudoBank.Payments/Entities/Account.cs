using PseudoBank.Payments.Models;
using System;
using System.Collections.Generic;

namespace PseudoBank.Payments.Runner.Entities
{
    public class Account : AccountBase
    {
        public Guid Id { get; set; }

        public AccountStatus AccountStatus { get; set; } = null!;

        public Guid AccountStatusId { get; set; }

        public ICollection<PaymentScheme> AllowedPaymentSchemes { get; set; } = new List<PaymentScheme>();
    }
}
