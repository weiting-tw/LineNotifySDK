namespace LineNotifySDK.Model
{
    /// <summary>
    /// <see ref="https://notify-bot.line.me/doc/"/>
    /// </summary>
    public class LineNotifyStatus
    {
        /// <summary>
        /// Value according to HTTP status code
        /// 200: Success・Access token valid
        /// 401: Invalid access token
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Message visible to end-user
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// If the notification target is a user: "USER"
        /// If the notification target is a group: "GROUP"
        /// </summary>
        public string TargetTye { get; set; }

        /// <summary>
        /// If the notification target is a user, displays user name.If acquisition fails, displays "null."
        /// If the notification target is a group, displays group name.If the target user has already left the group, displays "null."
        /// </summary>
        public string Target { get; set; }

    }
}