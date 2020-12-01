﻿using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="GroupStatisticsMessage"/>.</summary>
    public class GroupStatisticsResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Most recent group statistics.</summary>
        [JsonProperty("body")]
        public WolfGroupStatistics GroupStatistics { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected GroupStatisticsResponse() : base() { }
    }
}
