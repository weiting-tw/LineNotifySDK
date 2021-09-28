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

        /// <summary>
        /// Upload a image file to the LINE server.
        /// Supported image format is png and jpeg.
        /// If you specified imageThumbnail, imageFullsize and imageFile, imageFile takes precedence.
        /// There is a limit that you can upload to within one hour.
        /// For more information, please see the section of the API Rate Limit.
        /// </summary>
        public Stream ImageFile { get; set; }

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