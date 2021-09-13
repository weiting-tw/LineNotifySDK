using System.ComponentModel.DataAnnotations;
using System.IO;

namespace LineNotifySDK.Model
{
    /// <summary>
    /// <see ref="https://notify-bot.line.me/doc/"/>
    /// </summary>
    public class LineNotifyMessage
    {
        /// <summary>
        /// 1000 characters max
        /// </summary>
        [StringLength(1000, ErrorMessage = "1000 characters max")]
        public string Message { get; set; }

        /// <summary>
        /// Maximum size of 2048×2048px JPEG
        /// </summary>
        public string ImageThumbnail { get; set; }

        ///// <summary>
        ///// Maximum size of 2048×2048px JPEG
        ///// </summary>
        public string ImageFullSize { get; set; }

        public FileInfo ImageFile { get; set; }

        ///// <summary>
        ///// Package ID. Sticker List.<see ref="https://developers.line.biz/en/docs/messaging-api/sticker-list/"/>
        ///// </summary>
        public int? StickerPackageId { get; set; }

        ///// <summary>
        ///// Sticker ID.
        ///// </summary>
        public int? StickerId { get; set; }

        ///// <summary>
        ///// true: The user doesn't receive a push notification when the message is sent.
        ///// false: The user receives a push notification when the message is sent(unless they have disabled push notification in LINE and/or their device).
        ///// If omitted, the value defaults to false.
        ///// </summary>
        public bool NotificationDisabled { get; set; } = false;
    }
}