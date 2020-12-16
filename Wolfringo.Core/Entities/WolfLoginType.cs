namespace TehGM.Wolfringo
{
    /// <summary>Represents WOLF login type.</summary>
    public enum WolfLoginType
    {
        /// <summary>Login using Palringo account (email and password).</summary>
        Email,
        /// <summary>Login using Google SSO token.</summary>
        Google,
        /// <summary>Login using Facebook SSO token.</summary>
        Facebook,
        /// <summary>Login using Apple SSO token.</summary>
        Apple,
        /// <summary>Login using Twitter SSO token.</summary>
        Twitter,
        /// <summary>Login using Snapchat SSO token.</summary>
        Snapchat
    }
}
