using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for logging in.</summary>
    /// <remarks>Uses <see cref="LoginResponse"/> as response type.</remarks>
    [ResponseType(typeof(LoginResponse))]
    public class LoginMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.SecurityLogin;

        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };

        /// <summary>Login email.</summary>
        [JsonProperty("username", Required = Required.Always)]
        public string Login { get; private set; }
        /// <summary>Login password.</summary>
        /// <remarks>The password might be hashed. Check <see cref="UseMD5"/> to see if password is hashed.</remarks>
        [JsonProperty("password", Required = Required.Always)]
        public string Password { get; private set; }
        /// <summary>Whether the password is hashed.</summary>
        /// <remarks>Current implementation will hash passwords only when <see cref="LoginType"/> is <see cref="WolfLoginType.Email"/>.</remarks>
        [JsonProperty("md5Password")]
        public bool UseMD5 => this.LoginType == WolfLoginType.Email;
        [JsonProperty("type")]
        private string _loginType => LoginTypeToString(this.LoginType);

        /// <summary>Login type to use.</summary>
        [JsonIgnore]
        public WolfLoginType LoginType { get; }

        [JsonConstructor]
        protected LoginMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="login">Email to login with.</param>
        /// <param name="password">Password to use when logging in.</param>
        /// <param name="loginType">Type of login to perform.</param>
        public LoginMessage(string login, string password, WolfLoginType loginType) : this()
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            this.Login = login;
            this.LoginType = loginType;
            if (this.UseMD5)
            {
                using (MD5 crypto = MD5.Create())
                {
                    byte[] pwdBytes = Encoding.UTF8.GetBytes(password);
                    byte[] hash = crypto.ComputeHash(pwdBytes);

                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                        builder.Append(hash[i].ToString("x2"));
                    this.Password = builder.ToString();
                }
            }
            else
                this.Password = password;
        }

        public static string LoginTypeToString(WolfLoginType loginType)
        {
            switch (loginType)
            {
                case WolfLoginType.Email:
                    return "email";
                case WolfLoginType.Google:
                    return "google";
                case WolfLoginType.Facebook:
                    return "facebook";
                case WolfLoginType.Apple:
                    return "apple";
                case WolfLoginType.Twitter:
                    return "twitter";
                case WolfLoginType.Snapchat:
                    return "snapchat";
                default:
                    return loginType.ToString().ToLowerInvariant();
            }
        }
    }
}
