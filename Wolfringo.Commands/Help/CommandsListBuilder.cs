﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TehGM.Wolfringo.Commands.Initialization;

namespace TehGM.Wolfringo.Commands.Help
{
    /// <summary>Utility class that will build default commands list.</summary>
    public class CommandsListBuilder
    {
        private readonly IEnumerable<ICommandInstanceDescriptor> _commands;
        private string _builtCommandsList;
#if NET9_0_OR_GREATER
        private readonly Lock _lock = new Lock();
#else
        private readonly object _lock = new object();
#endif

        /// <summary>Prefix that should be prepended to each command. Set to null to not prepend any.</summary>
        public string PrependedPrefix
        {
            get => this._prependPrefix;
            set
            {
                lock (this._lock)
                {
                    if (this._prependPrefix == value)
                        return;

                    this._prependPrefix = value;
                    this._builtCommandsList = null;
                }
            }
        }
        private string _prependPrefix;
        /// <summary>String to separate command's display name and summary.</summary>
        /// <remarks>Defaults to `    - ` (four spaces, dash, and one more space).</remarks>
        public string SummarySeparator
        {
            get => this._summarySeparator;
            set
            {
                lock (this._lock)
                {
                    if (this._summarySeparator == value)
                        return;

                    this._summarySeparator = value;
                    this._builtCommandsList = null;
                }
            }
        }
        private string _summarySeparator = "    - ";
        /// <summary>Whether categories should have extra space between themselves.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool SpaceCategories
        {
            get => this._spaceCategories;
            set
            {
                lock (this._lock)
                {
                    if (this._spaceCategories == value)
                        return;

                    this._spaceCategories = value;
                    this._builtCommandsList = null;
                }
            }
        }
        private bool _spaceCategories = true;
        /// <summary>Whether commands without summaries will be listed.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool ListCommandsWithoutSummaries
        {
            get => this._withoutSummaries;
            set
            {
                lock (this._lock)
                {
                    if (this._withoutSummaries == value)
                        return;

                    this._withoutSummaries = value;
                    this._builtCommandsList = null;
                }
            }
        }
        private bool _withoutSummaries = true;

        /// <summary>Creates a new Builder.</summary>
        /// <param name="commands">List of command descriptors.</param>
        public CommandsListBuilder(IEnumerable<ICommandInstanceDescriptor> commands)
        {
            this._commands = commands ?? Enumerable.Empty<ICommandInstanceDescriptor>();
        }

        /// <summary>Creates a new Builder.</summary>
        /// <param name="commandsService">Commands service with loaded commands.</param>
        public CommandsListBuilder(ICommandsService commandsService)
            : this(commandsService?.Commands) { }

        /// <summary>Builds a commands list.</summary>
        /// <returns>String with commands list. Empty string if there's no commands.</returns>
        public string GetCommandsList()
        {
            lock (this._lock)
            {
                if (this._builtCommandsList != null)
                    return this._builtCommandsList;

                IOrderedEnumerable<IGrouping<string, ICommandInstanceDescriptor>> commands = this.OrderCommandDescriptors();
                if (!commands.Any())
                {
                    this._builtCommandsList = string.Empty;
                    return this._builtCommandsList;
                }

                StringBuilder builder = new StringBuilder();
                bool addPrefix = !string.IsNullOrWhiteSpace(this.PrependedPrefix);
                bool firstGroup = true;
                foreach (IGrouping<string, ICommandInstanceDescriptor> group in commands)
                {
                    if (!firstGroup && this.SpaceCategories)
                        builder.Append('\n');
                    firstGroup = false;

                    if (!string.IsNullOrWhiteSpace(group.Key))
                        builder.Append(group.Key + ":\n");

                    foreach (ICommandInstanceDescriptor descriptor in group)
                    {
                        string summary = descriptor.GetSummary();
                        if (!this.ListCommandsWithoutSummaries && string.IsNullOrWhiteSpace(summary))
                            continue;

                        if (addPrefix)
                        {
                            // check prefix override first
                            string prefixOverride = descriptor.GetPrefixOverride();
                            if (!string.IsNullOrWhiteSpace(prefixOverride))
                                builder.Append(prefixOverride);
                            // if no override, use standard prefix
                            else
                                builder.Append(this.PrependedPrefix);
                        }
                        builder.Append(descriptor.GetDisplayName());
                        if (!string.IsNullOrWhiteSpace(summary))
                        {
                            builder.Append(this.SummarySeparator);
                            builder.Append(summary);
                        }
                        builder.Append('\n');
                    }
                }

                this._builtCommandsList = builder.ToString().Trim();
                return this._builtCommandsList;
            }
        }

        private IOrderedEnumerable<IGrouping<string, ICommandInstanceDescriptor>> OrderCommandDescriptors()
        {
            // exclude hidden commands, and ones with no display name
            IEnumerable<ICommandInstanceDescriptor> descriptors = this._commands.Where(cmd => 
                !cmd.IsHidden() &&
                !string.IsNullOrWhiteSpace(cmd.GetDisplayName())
            );

            // exclude commands without summaries
            if (!this.ListCommandsWithoutSummaries)
                descriptors = descriptors.Where(cmd => !string.IsNullOrWhiteSpace(cmd.GetSummary()));

            // order commands based on priority and name
            IOrderedEnumerable<ICommandInstanceDescriptor> orderedDescriptors = descriptors
                .OrderByDescending(cmd => cmd.GetHelpOrder() ?? cmd.GetPriority())
                .ThenBy(cmd => cmd.GetDisplayName());

            // group commands by category
            IEnumerable<IGrouping<string, ICommandInstanceDescriptor>> groups = orderedDescriptors.GroupBy(cmd => cmd.GetHelpCategory()?.Name);

            // exclude empty groups
            groups = groups.Where(grp => grp.Any());

            // order groups based on priority and name
            IOrderedEnumerable<IGrouping<string, ICommandInstanceDescriptor>> orderedGroups = groups
                .OrderByDescending(grp => grp.FirstOrDefault()?.GetHelpCategory()?.Priority ?? 0)
                .ThenBy(grp => grp.Key);

            return orderedGroups;
        }

        /// <summary>Builds a commands list.</summary>
        /// <returns>String with commands list. Empty string if there's no commands.</returns>
        public override string ToString()
            => this.GetCommandsList();
    }
}
