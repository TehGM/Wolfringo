using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for updating group audio settings, and an event when the settings changed.</summary>
    public class GroupAudioUpdateMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.GroupAudioUpdate;

        /// <summary>Is audio stage enabled?</summary>
        [JsonProperty("enabled")]
        public bool IsEnabled { get; private set; }
        /// <summary>Minimum reputation level to enter audio stage.</summary>
        [JsonProperty("minRepLevel")]
        public int? MinimumReputationLevel { get; private set; }
        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        /// <summary>Type of the stage.</summary>
        [JsonProperty("stageId")]
        public WolfStageType? StageType { get; private set; }
        /// <summary>ID of user that made the update.</summary>
        [JsonProperty("sourceSubscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint? UpdatingUserID { get; private set; }

        [JsonConstructor]
        protected GroupAudioUpdateMessage() { }

        public class Builder
        {
            /// <summary>ID of the group.</summary>
            public uint GroupID { get; }
            /// <summary>Is audio stage enabled?</summary>
            public bool IsEnabled { get; set; }
            /// <summary>Minimum reputation level to enter audio stage.</summary>
            public int? MinimumReputationLevel { get; set; }
            /// <summary>Type of the stage.</summary>
            public WolfStageType StageType { get; set; }

            /// <summary>Create a new builder for <see cref="GroupAudioUpdateMessage"/>.</summary>
            /// <param name="existingConfig">Existing audio config.</param>
            public Builder(WolfGroup.WolfGroupAudioConfig existingConfig)
            {
                if (existingConfig == null)
                    throw new ArgumentNullException(nameof(existingConfig));

                this.GroupID = existingConfig.GroupID;
                this.IsEnabled = existingConfig.IsEnabled;
                this.MinimumReputationLevel = existingConfig.MinimumReputationLevel;
                this.StageType = existingConfig.StageType ?? WolfStageType.Basic;
            }

            /// <summary>Build the <see cref="GroupAudioUpdateMessage"/>.</summary>
            /// <returns>A new <see cref="GroupAudioUpdateMessage"/>.</returns>
            public GroupAudioUpdateMessage Build()
            {
                return new GroupAudioUpdateMessage()
                {
                    GroupID = this.GroupID,
                    IsEnabled = this.IsEnabled,
                    MinimumReputationLevel = this.MinimumReputationLevel,
                    StageType = this.StageType
                };
            }
        }
    }
}
