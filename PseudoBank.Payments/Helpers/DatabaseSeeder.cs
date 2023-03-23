using PseudoBank.Payments.Runner.Context;
using PseudoBank.Payments.Runner.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PseudoBank.Payments.Helpers
{
    public class DatabaseSeeder
    {
        private AccountContext AccountContext { get; }

        public DatabaseSeeder(AccountContext context)
        {
            AccountContext = context;
        }

        public async Task SeedDatabaseAsync(CancellationToken cancellationToken = default)
        {
            // Seed the payment schemes
            List<PaymentScheme> paymentSchemes = new()
            {
                new PaymentScheme() { Id = Guid.NewGuid(), SchemeName = PaymentSchemeConstants.ExpeditedPayments },
                new PaymentScheme() { Id = Guid.NewGuid(), SchemeName = PaymentSchemeConstants.BankToBankTransfer },
                new PaymentScheme() { Id = Guid.NewGuid(), SchemeName = PaymentSchemeConstants.AutomatedPaymentSystem }
            };

            await AccountContext.PaymentSchemes.AddRangeAsync(paymentSchemes, cancellationToken);

            // Seed the account statuses
            List<AccountStatus> accountStatuses = new()
            {
                new AccountStatus() { Id = Guid.NewGuid(), Status = AccountStatusConstants.Live },
                new AccountStatus() { Id = Guid.NewGuid(), Status = AccountStatusConstants.Disabled },
                new AccountStatus() { Id = Guid.NewGuid(), Status = AccountStatusConstants.InboundPaymentsOnly }
            };

            await AccountContext.AccountStatuses.AddRangeAsync(accountStatuses, cancellationToken);


            // Seed the accounts
            await AccountContext.Accounts.AddRangeAsync(new List<Account>()
            {
                new Account()
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "123",
                    AllowedPaymentSchemes = new List<PaymentScheme>()
                    {
                        paymentSchemes[0],
                        paymentSchemes[1],
                        paymentSchemes[2]
                    },
                    Balance = 1000.0m,
                    AccountStatus = accountStatuses[0]
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "456",
                    AllowedPaymentSchemes = new List<PaymentScheme>()
                    {
                        paymentSchemes[0],
                        paymentSchemes[2]
                    },
                    Balance = 500.0m,
                    AccountStatus = accountStatuses[1]
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "789",
                    AllowedPaymentSchemes = new List<PaymentScheme>()
                    {
                        paymentSchemes[2]
                    },
                    Balance = 100.0m,
                    AccountStatus = accountStatuses[2]
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "987",
                    AllowedPaymentSchemes = new List<PaymentScheme>()
                    {
                        paymentSchemes[2]
                    },
                    Balance = 100.0m,
                    AccountStatus = accountStatuses[0]
                }
            }, cancellationToken);

            // Save all of the seeded data to the database
            await AccountContext.SaveChangesAsync(cancellationToken);
        }
    }
}
