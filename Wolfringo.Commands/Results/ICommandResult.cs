namespace TehGM.Wolfringo.Commands
{
    /// <summary>Represents result of any command execution or check.</summary>
    public interface ICommandResult
    {
        /// <summary>Whether execution or check was successful.</summary>
        bool IsSuccess { get; }
    }
}
