using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LineNotifySDK.Model;
using Refit;

namespace LineNotifySDK
{
    public interface ILineNotifyClient
    {
        [Post("/api/notify")]
        Task<ApiResponse<string>> Sent(
            [Authorize] string token,
            [Body(BodySerializationMethod.UrlEncoded)] LineNotifyMessage message,
            CancellationToken cancellationToken);

        [Multipart]
        [Post("/api/notify")]
        Task<ApiResponse<LineMessageResponse>> Sent(
            [Authorize] string token,
            string message,
            string imageThumbnail,
            [AliasAs("imageFullsize")] string imageFullSize,
            Stream imageFile,
            int? stickerPackageId,
            int? stickerId,
            bool notificationDisabled,
            CancellationToken cancellationToken);

        [Get("/api/status")]
        Task<ApiResponse<LineNotifyStatus>> Status([Authorize] string token, CancellationToken cancellationToken);

        [Post("/api/revoke")]
        Task<ApiResponse<LineMessageResponse>> Revoke([Authorize] string token, CancellationToken cancellationToken);
    }
}