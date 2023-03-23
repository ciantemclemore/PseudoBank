using Microsoft.EntityFrameworkCore;
using PseudoBank.Payments.DTOs;
using PseudoBank.Payments.Factories;
using PseudoBank.Payments.Runner.Context;
using PseudoBank.Payments.Runner.Entities;
using PseudoBank.Payments.Validators;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PseudoBank.Payments.Services
{
    public class PaymentService : IPaymentService
    {
        private AccountContext AccountContext { get; }
        
        public IPaymentValidatorFactory PaymentValidatorFactory { get; }

        public PaymentService(AccountContext context, IPaymentValidatorFactory paymentValidatorFactory) 
        {
            AccountContext = context;    
            PaymentValidatorFactory = paymentValidatorFactory;
        }

        public async Task<MakePaymentResponse> MakePaymentAsync(MakePaymentRequestBase request, CancellationToken cancellationToken = default)
        {
            // The response that will inform the user if the payment is successful or not
            MakePaymentResponse makePaymentResponse = new();

            try 
            {
                // Find the account of the debtor and the creditor
                Account? debtorAccount = await AccountContext.Accounts
                    .Include(a => a.AccountStatus)
                    .Include(a => a.AllowedPaymentSchemes)
                    .FirstOrDefaultAsync(da => da.AccountNumber == request.DebtorAccountNumber, cancellationToken);
                
                Account? creditorAccount = await AccountContext.Accounts
                    .Include(a => a.AccountStatus)
                    .Include(a => a.AllowedPaymentSchemes)
                    .FirstOrDefaultAsync(ca => ca.AccountNumber == request.CreditorAccountNumber, cancellationToken);

                if (debtorAccount == null || creditorAccount == null)
                {
                    makePaymentResponse.ErrorMessage = "Account(s) not found";
                    return makePaymentResponse;
                }

                // Validate if the account has the current payment scheme available
                if (!debtorAccount.AllowedPaymentSchemes.Any(aps => aps.SchemeName == request.PaymentScheme.SchemeName))
                {
                    makePaymentResponse.ErrorMessage = "Account is not authorized to make this type of payment";
                    return makePaymentResponse;
                }

                // Now we can use our payment validator to validate the specific payment
                PaymentValidatorBase validator = PaymentValidatorFactory.CreateValidator(request.PaymentScheme.SchemeName);

                if (!validator.IsValidPayment(request.Amount, debtorAccount.Balance, debtorAccount.AccountStatus.Status, creditorAccount.AccountStatus.Status))
                {
                    makePaymentResponse.ErrorMessage = validator.ErrorMessage;
                    return makePaymentResponse;
                }
                
                // update the balance of the debtor and the creditor and save the changes to the database
                debtorAccount.Balance -= request.Amount;
                creditorAccount.Balance += request.Amount;

                // update the payment response for successful payment
                makePaymentResponse.IsSuccess = true;
                makePaymentResponse.ErrorMessage = string.Empty;

                // save the changes to the tracked entities
                await AccountContext.SaveChangesAsync(cancellationToken);
            }
            catch(Exception ex) 
            {
                // in the future, we could return a specific type of error message/exception instead of wrapping a response with a bool (maybe create our own exception class)
                makePaymentResponse.ErrorMessage = ex.Message;
            }

            return makePaymentResponse;
        }
    }
}
