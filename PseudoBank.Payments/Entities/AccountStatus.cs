using PseudoBank.Payments.Models;
using System;
using System.Collections.Generic;

namespace PseudoBank.Payments.Runner.Entities
{
    public class AccountStatus : AccountStatusBase
    {
        public Guid Id { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
