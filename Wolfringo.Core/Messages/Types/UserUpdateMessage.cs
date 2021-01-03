﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for updating a group.</summary>
    /// <remarks>Uses <see cref="UserUpdateResponse"/> as response type.</remarks>
    [ResponseType(typeof(UserUpdateResponse))]
    public class UserUpdateMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.SubscriberProfileUpdate"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.SubscriberProfileUpdate;

        // main props
        /// <summary>User's display name.</summary>
        [JsonProperty("nickname")]
        public string Nickname { get; protected set; }
        /// <summary>User's status.</summary>
        [JsonProperty("status")]
        public string Status { get; protected set; }
        // extended props
        /// <summary>User's name, as specified in the profile.</summary>
        [JsonProperty("name")]
        public string ProfileName { get; protected set; }
        /// <summary>User's "About Me" description.</summary>
        [JsonProperty("about")]
        public string About { get; protected set; }
        /// <summary>User's gender.</summary>
        [JsonProperty("gender")]
        public WolfGender Gender { get; protected set; }
        /// <summary>User's language.</summary>
        [JsonProperty("language")]
        public WolfLanguage Language { get; protected set; }
        /// <summary>User's relationship status.</summary>
        [JsonProperty("relationship")]
        public WolfRelationship Relationship { get; protected set; }
        /// <summary>User's looking for.</summary>
        [JsonProperty("lookingFor")]
        public WolfLookingFor LookingFor { get; protected set; }
        /// <summary>User's links.</summary>
        [JsonProperty("urls")]
        public IEnumerable<string> Links { get; protected set; }
        /// <summary>User's date of birth.</summary>
        [JsonProperty("dateOfBirth")]
        public DateTime? DateOfBirth { get; protected set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected UserUpdateMessage() { }

        /// <summary>A builder class for <see cref="UserUpdateMessage"/>.</summary>
        public class Builder
        {
            // main props
            /// <summary>User's display name.</summary>
            public string Nickname { get; set; }
            /// <summary>User's status.</summary>
            public string Status { get; set; }
            // extended props
            /// <summary>User's name, as specified in the profile.</summary>
            public string ProfileName { get; set; }
            /// <summary>User's "About Me" description.</summary>
            public string About { get; set; }
            /// <summary>User's gender.</summary>
            public WolfGender Gender { get; set; }
            /// <summary>User's language.</summary>
            public WolfLanguage Language { get; set; }
            /// <summary>User's relationship status.</summary>
            public WolfRelationship Relationship { get; set; }
            /// <summary>User's looking for.</summary>
            public WolfLookingFor LookingFor { get; set; }
            /// <summary>User's links.</summary>
            public ICollection<string> Links { get; set; }
            /// <summary>User's date of birth.</summary>
            public DateTime? DateOfBirth { get; set; }

            /// <summary>Create a new builder for <see cref="UserUpdateMessage"/>.</summary>
            /// <remarks>Ensure that <paramref name="user"/> is always currently connected user,
            /// as the message is being sent without any user ID, so always updates the current user.</remarks>
            /// <param name="user">User to update.</param>
            public Builder(WolfUser user)
            {
                this.Nickname = user.Nickname;
                this.Status = user.Status;
                this.ProfileName = user.ProfileName;
                this.About = user.About;
                this.Gender = user.Gender ?? WolfGender.NotSpecified;
                this.Language = user.Language ?? WolfLanguage.NotSpecified;
                this.Relationship = user.Relationship ?? WolfRelationship.NotSpecified;
                this.LookingFor = user.LookingFor ?? WolfLookingFor.NotSpecified;
                this.DateOfBirth = user.DateOfBirth;
                // create new collection to not modify the underlying user
                this.Links = user.Links == null ? new List<string>() : new List<string>(user.Links);
            }

            /// <summary>Build the <see cref="GroupUpdateMessage"/>.</summary>
            /// <returns>A new <see cref="GroupUpdateMessage"/>.</returns>
            public UserUpdateMessage Build()
            {
                return new UserUpdateMessage()
                {
                    Nickname = this.Nickname,
                    Status = this.Status,
                    ProfileName = this.ProfileName,
                    About = this.About,
                    Gender = this.Gender,
                    Language = this.Language,
                    Relationship = this.Relationship,
                    DateOfBirth = this.DateOfBirth,
                    Links = new ReadOnlyCollection<string>(
                        (this.Links as IList<string>) 
                        ?? this.Links?.ToArray() 
                        ?? Enumerable.Empty<string>().ToArray())
                };
            }
        }
    }
}
