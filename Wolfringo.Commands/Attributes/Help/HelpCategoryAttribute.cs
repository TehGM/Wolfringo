using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Assigns command or all commands in the handler to a specific category in help list.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HelpCategoryAttribute : Attribute
    {
        /// <summary>Name of the category.</summary>
        public string Name { get; }
        /// <summary>Priority of the category.</summary>
        public int Priority { get; set; }

        /// <summary>Assigns command or all commands in the handler to a specific category in help list.</summary>
        /// <param name="name">Name of the category.</param>
        /// <param name="priority">Priority of the category.</param>
        public HelpCategoryAttribute(string name, int priority)
        {
            this.Name = name;
            this.Priority = priority;
        }

        /// <summary>Assigns command or all commands in the handler to a specific category in help list, with priority of 0.</summary>
        /// <param name="name">Name of the category.</param>
        public HelpCategoryAttribute(string name)
            : this(name, 0) { }
    }
}
