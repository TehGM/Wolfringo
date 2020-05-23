using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(LoginResponse))]
    public class LoginMessage : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.Login;

        [JsonProperty("username", Required = Required.Always)]
        public string Login { get; private set; }
        [JsonProperty("password", Required = Required.Always)]
        public string Md5Password { get; private set; }
        [JsonProperty("md5Password")]
        private readonly bool _useMd5 = true;
        [JsonProperty("type")]
        private string _loginType = "email";

        public LoginMessage(string login, string password, bool isPasswordAlreadyHashed = false)
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
