using System;
using System.Collections.Generic;
using System.Reflection;

namespace TehGM.Wolfringo.Commands
{
    public class CommandsOptions
    {
        public string Prefix { get; set; } = "!";
        public bool CaseInsensitive { get; set; } = true;
        public PrefixRequirement RequirePrefix { get; set; } = PrefixRequirement.Always;

        // for loading
        public ICollection<Type> Classes { get; set; } = new List<Type>();  // classes that  Command System will look at when initializing
        public ICollection<Assembly> Assemblies { get; set; } = new List<Assembly>() { Assembly.GetEntryAssembly() };   // assemblies that  Command System will look at when initializing
    }
}
