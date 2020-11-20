namespace TehGM.Wolfringo.Commands
{
    public interface ICommandsOptions
    {
        string Prefix { get; }
        bool CaseInsensitive { get; }
        PrefixRequirement RequirePrefix { get; }
    }
}
