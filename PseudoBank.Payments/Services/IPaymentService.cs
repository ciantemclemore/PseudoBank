using PseudoBank.Payments.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace PseudoBank.Payments.Services
{
    public interface IPaymentService
    {
        Task<MakePaymentResponse> MakePaymentAsync(MakePaymentRequestBase request, CancellationToken cancellationToken = default);
    }
}
