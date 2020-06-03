using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo.Messages
{
    public class GroupAudioUpdateMessage : IWolfMessage
    {
        public string Command => MessageCommands.GroupAudioUpdate;

        [JsonProperty("enabled")]
        public bool IsEnabled { get; private set; }
        [JsonProperty("minRepLevel")]
        public int? MinimumReputationLevel { get; private set; }
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        [JsonProperty("stageId")]
        public WolfStageType? StageType { get; private set; }
        [JsonProperty("sourceSubscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint? UpdatingUserID { get; private set; }

        [JsonConstructor]
        private GroupAudioUpdateMessage() { }

        public class Builder
        {
            public uint GroupID { get; }
            public bool IsEnabled { get; set; }
            public int? MinimumReputationLevel { get; set; }
            public WolfStageType StageType { get; set; }

            public Builder(WolfGroup.WolfGroupAudioConfig existingConfig)
            {
                if (existingConfig == null)
                    throw new ArgumentNullException(nameof(existingConfig));

                this.GroupID = existingConfig.GroupID;
                this.IsEnabled = existingConfig.IsEnabled;
                this.MinimumReputationLevel = existingConfig.MinimumReputationLevel;
                this.StageType = existingConfig.StageType ?? WolfStageType.Basic;
            }

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
