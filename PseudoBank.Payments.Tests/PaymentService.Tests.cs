using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PseudoBank.Payments.DTOs;
using PseudoBank.Payments.Factories;
using PseudoBank.Payments.Helpers;
using PseudoBank.Payments.Runner.Context;
using PseudoBank.Payments.Services;
using PseudoBank.Payments.Validators;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PseudoBank.Payments.Tests
{
    public class PaymentServiceTests : IAsyncLifetime
    {
        private IPaymentService PaymentService { get; set; }

        private AccountContext AccountContext { get; set; }

        private DatabaseSeeder DatabaseSeeder { get; set; }

        private ServiceProvider ServiceProvider { get; set; }

        public PaymentServiceTests() 
        {
        }

        public async Task InitializeAsync()
        {
            // Create our provider and configure our services to mock our real application
            ServiceProvider = ServiceProviderHelper.ConfigureServices();

            // now we can seed our database and get our payment service to test
            PaymentService = ServiceProvider.GetRequiredService<IPaymentService>();
            DatabaseSeeder = ServiceProvider.GetRequiredService<DatabaseSeeder>();
            AccountContext = ServiceProvider.GetRequiredService<AccountContext>();

            // apply the database migration to update the database schema
            await AccountContext.Database.OpenConnectionAsync();

            await AccountContext.Database.MigrateAsync();

            // Seed the database so that we have initial data
            await DatabaseSeeder.SeedDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            await AccountContext.Database.CloseConnectionAsync();
        }

        [Fact]
        public async Task MakePayment_GivenBadAccountNumber_ReturnsFalseStatusAndErrorMessage()
        {
            // Arrange
            // Make a payment request with a bad account number
            MakeAutomatedPaymentRequest request = new()
            {
                Amount = 100.0m,
                CreditorAccountNumber = "123",
                DebtorAccountNumber = "001",
                PaymentDate = DateTime.Now,
                PaymentScheme = new()
                {
                    SchemeName = PaymentSchemeConstants.AutomatedPaymentSystem
                }
            };

            // Act
            MakePaymentResponse expectedResponse = new() { IsSuccess = false, ErrorMessage = "Account(s) not found" };
            MakePaymentResponse actualResponse = await PaymentService.MakePaymentAsync(request);

            // Assert
            Assert.False(actualResponse.IsSuccess);
            Assert.Equal(expectedResponse.ErrorMessage, actualResponse.ErrorMessage);
        }

        [Fact]
        public async Task MakePayment_WithoutAllowedPaymentScheme_ReturnsFalseStatusAndErrorMessage()
        {
            // Arrange
            // Make a bank-to-bank transfer from an account that doesn't allow the specified payment scheme
            MakeBankToBankPaymentRequest request = new()
            {
                Amount = 200.0m,
                CreditorAccountNumber = "123",
                DebtorAccountNumber = "456",
                PaymentDate = DateTime.Now,
                PaymentScheme = new()
                {
                    SchemeName = PaymentSchemeConstants.BankToBankTransfer
                }
            };

            // Act
            MakePaymentResponse expectedResponse = new() { IsSuccess = false, ErrorMessage = "Account is not authorized to make this type of payment" };
            MakePaymentResponse actualResponse = await PaymentService.MakePaymentAsync(request);

            // Assert
            Assert.False(actualResponse.IsSuccess);
            Assert.Equal(expectedResponse.ErrorMessage, actualResponse.ErrorMessage);
        }

        [Fact]
        public async Task MakePayment_GivenAccountWithInboundOnlyStatus_ReturnsFalseStatusAndErrorMessage()
        {
            // Arrange
            // Make a payment request to an account that can only accept inbound payments
            MakeAutomatedPaymentRequest request = new()
            {
                Amount = 50.0m,
                CreditorAccountNumber = "123",
                DebtorAccountNumber = "789",
                PaymentDate = DateTime.Now,
                PaymentScheme = new()
                {
                    SchemeName = PaymentSchemeConstants.AutomatedPaymentSystem
                }
            };

            // Act
            MakePaymentResponse expectedResponse = new() { IsSuccess = false, ErrorMessage = "Inbound only accounts cannot make payments" };
            MakePaymentResponse actualResponse = await PaymentService.MakePaymentAsync(request);

            // Assert
            Assert.False(actualResponse.IsSuccess);
            Assert.Equal(expectedResponse.ErrorMessage, actualResponse.ErrorMessage);
        }

        [Fact]
        public async Task MakePayment_GivenAccountWithDisabledStatus_ReturnsFalseStatusAndErrorMessage()
        {
            // Arrange
            // Make a payment request to a disabled account
            MakeExpeditedPaymentRequest request = new()
            {
                Amount = 50.0m,
                CreditorAccountNumber = "123",
                DebtorAccountNumber = "456",
                PaymentDate = DateTime.Now,
                PaymentScheme = new()
                {
                    SchemeName = PaymentSchemeConstants.ExpeditedPayments
                }
            };

            // Act
            MakePaymentResponse expectedResponse = new() { IsSuccess = false, ErrorMessage = "Account must be active to make payment" };
            MakePaymentResponse actualResponse = await PaymentService.MakePaymentAsync(request);

            // Assert
            Assert.False(actualResponse.IsSuccess);
            Assert.Equal(expectedResponse.ErrorMessage, actualResponse.ErrorMessage);
        }

        [Fact]
        public async Task MakePayment_GivenAccountWithBalanceLowerThanPaymentAmount_ReturnsFalseStatusAndErrorMessage()
        {
            // Arrange
            // Send a payment request that is higher than the debtor's balance
            MakeAutomatedPaymentRequest request = new()
            {
                Amount = 500.0m,
                CreditorAccountNumber = "123",
                DebtorAccountNumber = "987",
                PaymentDate = DateTime.Now,
                PaymentScheme = new()
                {
                    SchemeName = PaymentSchemeConstants.AutomatedPaymentSystem
                }
            };

            // Act
            MakePaymentResponse expectedResponse = new() { IsSuccess = false, ErrorMessage = "Account balance has insufficient funds to make payment" };
            MakePaymentResponse actualResponse = await PaymentService.MakePaymentAsync(request);

            // Assert
            Assert.False(actualResponse.IsSuccess);
            Assert.Equal(expectedResponse.ErrorMessage, actualResponse.ErrorMessage);
        }

        [Fact]
        public async Task MakePayment_GivenAPaymentAmountLessThanZero_ReturnsFalseStatusAndErrorMessage()
        {
            // Arrange
            // Send a request with a payment amout that is less than zero
            MakeBankToBankPaymentRequest request = new()
            {
                Amount = -1.0m,
                CreditorAccountNumber = "789",
                DebtorAccountNumber = "123",
                PaymentDate = DateTime.Now,
                PaymentScheme = new()
                {
                    SchemeName = PaymentSchemeConstants.BankToBankTransfer
                }
            };

            // Act
            MakePaymentResponse expectedResponse = new() { IsSuccess = false, ErrorMessage = "Invalid payment amount" };
            MakePaymentResponse actualResponse = await PaymentService.MakePaymentAsync(request);

            // Assert
            Assert.False(actualResponse.IsSuccess);
            Assert.Equal(expectedResponse.ErrorMessage, actualResponse.ErrorMessage);
        }

        [Fact]
        public async Task MakePayment_GivenValidPaymentRequest_ReturnsTrueStatusAndNoErrorMessage()
        {
            // Arrange
            // Send a valid payment request to see if it successfully passes
            MakeExpeditedPaymentRequest request = new()
            {
                Amount = 500.0m,
                CreditorAccountNumber = "789",
                DebtorAccountNumber = "123",
                PaymentDate = DateTime.Now,
                PaymentScheme = new()
                {
                    SchemeName = PaymentSchemeConstants.ExpeditedPayments
                }
            };

            // Act
            MakePaymentResponse expectedResponse = new() { IsSuccess = true, ErrorMessage = string.Empty };
            MakePaymentResponse actualResponse = await PaymentService.MakePaymentAsync(request);

            // Assert
            Assert.True(actualResponse.IsSuccess);
            Assert.Equal(expectedResponse.ErrorMessage, actualResponse.ErrorMessage);
        }


        [Fact]
        public void MakePayment_GivenNoPaymentSchemeName_ReturnsException()
        {
            // Arrange
            // Send a payment request without a payment scheme
            MakeExpeditedPaymentRequest request = new()
            {
                Amount = 500.0m,
                CreditorAccountNumber = "789",
                DebtorAccountNumber = "123",
                PaymentDate = DateTime.Now,
            };

            // Get the validator factory so we can request a validator
            IPaymentValidatorFactory validatorFactory = ServiceProvider.GetRequiredService<IPaymentValidatorFactory>();

            // Act
            MakePaymentResponse expectedResponse = new() { IsSuccess = true, ErrorMessage = string.Empty };
            Func<PaymentValidatorBase> actualResponse = () => validatorFactory.CreateValidator(request.PaymentScheme.SchemeName); ;

            // Assert
            Assert.Throws<ArgumentException>(actualResponse);
        }
    }
}
