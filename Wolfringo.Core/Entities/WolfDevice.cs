namespace TehGM.Wolfringo
{
    /// <summary>Wolf device types.</summary>
    public enum WolfDevice
    {
        // values borrowed from https://github.com/dewwalters/Wolf.Net/blob/master/Wolf.Net/Enums/DeviceType.cs
        /// <summary>Unrecognized device typ.</summary>
        Unknown = 0,
        /// <summary>Bot.</summary>
        Bot = 1,
        /// <summary>Windows or Linux desktop client.</summary>
        PC = 2,
        /// <summary>Legacy mobile client.</summary>
        Mobile = 3,
        /// <summary>Macintosh desktop client.</summary>
        Mac = 4,
        /// <summary>iPhone client.</summary>
        iPhone = 5,
        /// <summary>iPad client.</summary>
        iPad = 6,
        /// <summary>Android client.</summary>
        Android = 7,
        /// <summary>Web browser client.</summary>
        Web = 8,
        /// <summary>Windows phone client.</summary>
        WindowsPhone = 9
    }
}
