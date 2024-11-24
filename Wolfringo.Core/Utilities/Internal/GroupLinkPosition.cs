namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Represents position of group link in a text.</summary>
    public struct GroupLinkPosition
    {
        public int Start { get; }
        public int End { get; }
        public string Text { get; }

        public GroupLinkPosition(int start, int end, string text)
        {
            this.Start = start;
            this.End = end;
            this.Text = text;
        }
    }
}
