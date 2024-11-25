namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Represents position of group link in a text.</summary>
    public struct GroupLinkPosition
    {
        /// <summary>Start of a group link.</summary>
        public int Start { get; }
        /// <summary>End of a group link.</summary>
        public int End { get; }
        /// <summary>Contents of group link within brackets (group name).</summary>
        public string Text { get; }

        /// <summary>Create a new instance of group link position.</summary>
        /// <param name="start">Start of a group link.</param>
        /// <param name="end">End of a group link.</param>
        /// <param name="text">Contents of group link within brackets (group name).</param>
        public GroupLinkPosition(int start, int end, string text)
        {
            this.Start = start;
            this.End = end;
            this.Text = text;
        }
    }
}
