using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for logging in.</summary>
    /// <remarks>Uses <see cref="LoginResponse"/> as response type.</remarks>
    [ResponseType(typeof(LoginResponse))]
    public class LoginMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.SecurityLogin;

        /// <summary>Login email.</summary>
        [JsonProperty("username", Required = Required.Always)]
        public string Login { get; private set; }
        /// <summary>Login password.</summary>
        [JsonProperty("password", Required = Required.Always)]
        public string Md5Password { get; private set; }
        [JsonProperty("md5Password")]
        private readonly bool _useMd5 = true;
        [JsonProperty("type")]
        private readonly string _loginType = "email";

        [JsonConstructor]
        protected LoginMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="login">Email to login with.</param>
        /// <param name="password">Password to use when logging in.</param>
        /// <param name="isPasswordAlreadyHashed">If false, constructor will hash the provided password; if false, password will be sent as provided.</param>
        public LoginMessage(string login, string password, bool isPasswordAlreadyHashed = false) : this()
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            this.Login = login;
            if (isPasswordAlreadyHashed)
                this.Md5Password = password;
            else
            {
                using (MD5 crypto = MD5.Create())
                {
                    byte[] pwdBytes = Encoding.UTF8.GetBytes(password);
                    byte[] hash = crypto.ComputeHash(pwdBytes);

                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                        builder.Append(hash[i].ToString("x2"));
                    this.Md5Password = builder.ToString();
                }
            }
        }
    }
}
