namespace LineNotifySDK.Model
{
    /// <summary>
    /// <see ref="https://notify-bot.line.me/doc/"/>
    /// </summary>
    public class LineNotifyOptions
    {
        /// <summary>
        /// Assigns the client ID of the generated OAuth
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Assigns secret to issued OAuth
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Assigns the generated redirect URI.
        /// We recommend using HTTPS on redirect URI to prevent code parameter leaks.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// By assigning "form_post", sends POST request to redirect_uri by form post instead of redirecting
        /// Extended specifications: <see ref="https://openid.net/specs/oauth-v2-form-post-response-mode-1_0.html"/>
        /// We recommend assigning this to prevent code parameter leaks in certain environments
        /// Reference：<see ref="http://arstechnica.com/security/2016/07/new-attack-that-cripples-https-crypto-works-on-macs-windows-and-linux/"/>
        /// </summary>
        public string ResponseMode { get; set; } = "form_post";

        /// <summary>
        /// Assigns "notify"
        /// </summary>
        internal string Scope { get; } = "notify";

        /// <summary>
        /// Assigns "code"
        /// </summary>
        internal string ResponseType { get; } = "code";

        internal string GrantType { get; } = "authorization_code";
    }
}