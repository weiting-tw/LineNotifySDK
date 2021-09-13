namespace LineNotifySDK.Model
{
    /// <summary>
    /// <see ref="https://notify-bot.line.me/doc/"/>
    /// </summary>
    public class LineMessageResponse
    {
        /// <summary>
        /// 200: Success
        /// 400: Bad request
        /// 401: Invalid access token
        /// 500: Failure due to server error
        /// Other: Processed over time or stopped
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Message visible to end-user
        /// </summary>
        public string Message { get; set; }
    }
}