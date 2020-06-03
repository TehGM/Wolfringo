using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo
{
    public class UserUpdateMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberProfileUpdate;

        // main props
        [JsonProperty("nickname")]
        public string Nickname { get; private set; }
        [JsonProperty("status")]
        public string Status { get; private set; }
        // extended props
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("about")]
        public string About { get; private set; }
        [JsonProperty("gender")]
        public WolfGender Gender { get; private set; }
        [JsonProperty("language")]
        public WolfLanguage Language { get; private set; }
        [JsonProperty("relationship")]
        public WolfRelationship Relationship { get; private set; }
        [JsonProperty("lookingFor")]
        public WolfLookingFor LookingFor { get; private set; }
        [JsonProperty("urls")]
        public IEnumerable<string> Links { get; private set; }

        [JsonConstructor]
        private UserUpdateMessage() { }

        public class Builder
        {
            // main props
            public string Nickname { get; set; }
            public string Status { get; set; }
            // extended props
            public string Name { get; set; }
            public string About { get; set; }
            public WolfGender Gender { get; set; }
            public WolfLanguage Language { get; set; }
            public WolfRelationship Relationship { get; set; }
            public WolfLookingFor LookingFor { get; set; }
            public ICollection<string> Links { get; set; }

            public Builder(WolfUser user)
            {
                this.Nickname = user.Nickname;
                this.Status = user.Status;
                this.Name = user.Name;
                this.About = user.About;
                this.Gender = user.Gender ?? WolfGender.NotSpecified;
                this.Language = user.Language ?? WolfLanguage.NotSpecified;
                this.Relationship = user.Relationship ?? WolfRelationship.NotSpecified;
                this.LookingFor = user.LookingFor ?? WolfLookingFor.NotSpecified;
                // create new collection to not modify the underlying user
                this.Links = user.Links == null ? null : new List<string>(user.Links);
            }

            public UserUpdateMessage Build()
            {
                return new UserUpdateMessage()
                {
                    Nickname = this.Nickname,
                    Status = this.Status,
                    Name = this.Name,
                    About = this.About,
                    Gender = this.Gender,
                    Language = this.Language,
                    Relationship = this.Relationship,
                    Links = this.Links
                };
            }
        }
    }
}
