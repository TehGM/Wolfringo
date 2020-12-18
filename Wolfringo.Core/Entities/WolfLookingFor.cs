namespace TehGM.Wolfringo
{
    /// <summary>User's "looking for" field value.</summary>
    public enum WolfLookingFor
    {
        // values borrowed from https://github.com/dewwalters/Wolf.Net/blob/master/Wolf.Net/Enums/LookingFor.cs
        /// <summary>Field is not specified.</summary>
        NotSpecified = 0,
        /// <summary>Looking for friendship.</summary>
        Friendship = 1,
        /// <summary>Looking for dating.</summary>
        Dating = 2,
        /// <summary>Looking for relationship.</summary>
        Relationship = 4,
        /// <summary>Looking for networking..</summary>
        Networking = 8
    }
}
