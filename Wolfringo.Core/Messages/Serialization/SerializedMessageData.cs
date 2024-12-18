﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Message JSON and binary messages. For messages and events, it will not contain the command.</summary>
    /// <remarks>This class bundles message and response JSON data and binary data for easy passing as a parameter.</remarks>
    public class SerializedMessageData
    {
        /// <summary>Message payload. For messages and events, it will not contain the command.</summary>
        public JToken Payload { get; }
        /// <summary>Message binary messages.</summary>
        public IEnumerable<byte[]> BinaryMessages { get; }

        /// <summary>Creates message JSON and binary data bundle.</summary>
        /// <param name="payload">JSON payload.</param>
        /// <param name="binaryMessages">All binary messages.</param>
        public SerializedMessageData(JToken payload, IEnumerable<byte[]> binaryMessages)
        {
            this.Payload = payload;
            this.BinaryMessages = binaryMessages;
        }

        /// <summary>Creates message JSON and binary data bundle.</summary>
        /// <param name="payload">JSON payload.</param>
        /// <param name="binaryData">Binary message.</param>
        public SerializedMessageData(JToken payload, byte[] binaryData)
            : this(payload, new byte[][] { binaryData }) { }

        /// <summary>Encapsulates JSON data only.</summary>
        /// <param name="payload">JSON payload.</param>
        public SerializedMessageData(JToken payload)
            : this(payload, Enumerable.Empty<byte[]>()) { }

        /// <summary>Is response an error?</summary>
        /// <returns>True if response is an error; otherwise false.</returns>
        // Baleringo does some really crappy way of returning the response codes
        public bool IsError()
        {
            if (this.Payload == null)
                return false;

            if (this.Payload is JArray elements && elements.Count == 1)
            {
                if (elements.First is JObject obj)
                {
                    if (!obj.ContainsKey("code"))
                        return false;

                    int code = obj["code"].ToObject<int>();
                    return code < 200 || code > 299;
                }
            }

            return false;
        }
    }
}
