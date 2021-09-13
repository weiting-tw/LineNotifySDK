using System;
using System.Threading;
using System.Threading.Tasks;
using LineNotifySDK.Model;

namespace LineNotifySDK
{
    public interface ILineNotifyServices
    {
        Uri GetAuthorizeUri(string state = null);

        Task<string> GetTokenAsync(string code, CancellationToken cancellationToken = default);

        Task SentAsync(string token, LineNotifyMessage message, CancellationToken cancellationToken = default);

        Task<LineNotifyStatus> StatusAsync(string token, CancellationToken cancellationToken = default);

        Task RevokeAsync(string token, CancellationToken cancellationToken = default);
    }
}