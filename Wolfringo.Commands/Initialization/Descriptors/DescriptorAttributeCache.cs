using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>A class caching descriptor attribute data.</summary>
    /// <remarks>This class is used internally by <see cref="CommandInstanceDescriptorExtensions"/> and <see cref="Help.DescriptorHelpExtensions"/>.</remarks>
    internal class DescriptorAttributeCache
    {
        private static readonly IDictionary<ICommandInstanceDescriptor, DescriptorAttributeCache> _cache = new Dictionary<ICommandInstanceDescriptor, DescriptorAttributeCache>();

        /// <summary>The descriptor this object caches data for.</summary>
        public ICommandInstanceDescriptor Descriptor { get; }

        /*** General attributes ***/
        /// <summary>Cached command priority.</summary>
        public int Priority => this._priority.Value;
        /// <summary>Cached command case sensitivity override.</summary>
        public bool? CaseSensitivityOverride => this._caseSensitivityOverride.Value;
        /// <summary>Cached command prefix override.</summary>
        public string PrefixOverride => this._prefixOverride.Value;
        /// <summary>Cached command prefix requirement override.</summary>
        public PrefixRequirement? PrefixRequirementOverride => this._prefixRequirementOverride.Value;
        /// <summary>Cached command requirement attributes override.</summary>
        public IEnumerable<CommandRequirementAttribute> Requirements => this._requirements.Value;

        private readonly Lazy<int> _priority;
        private readonly Lazy<bool?> _caseSensitivityOverride;
        private readonly Lazy<string> _prefixOverride;
        private readonly Lazy<PrefixRequirement?> _prefixRequirementOverride;
        private readonly Lazy<IEnumerable<CommandRequirementAttribute>> _requirements;

        /*** Help attributes ***/
        /// <summary>Cached command display name.</summary>
        public string DisplayName => this._displayName.Value;
        /// <summary>Cached command summary text.</summary>
        public string Summary => this._summary.Value;
        /// <summary>Cached info whether command or its handler is hidden.</summary>
        public bool IsHidden => this._hidden.Value;
        /// <summary>Cached help category of command or its descriptor.</summary>
        public HelpCategoryAttribute HelpCategory => this._helpCategory.Value;
        /// <summary>Cached command help order.</summary>
        public int? HelpOrder => this._helpOrder.Value;

        private readonly Lazy<string> _displayName;
        private readonly Lazy<string> _summary;
        private readonly Lazy<bool> _hidden;
        private readonly Lazy<HelpCategoryAttribute> _helpCategory;
        private readonly Lazy<int?> _helpOrder;

        /*** All attributes ***/
        private readonly Lazy<IEnumerable<Attribute>> _commandAttributes;
        private readonly Lazy<IEnumerable<Attribute>> _handlerAttributes;

        /// <summary>Creates a new descriptor help cache container.</summary>
        /// <param name="descriptor">Descriptor which to create cache for.</param>
        public DescriptorAttributeCache(ICommandInstanceDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            // store descriptor
            this.Descriptor = descriptor;

            // cache all attributes
            this._commandAttributes = new Lazy<IEnumerable<Attribute>>(() => descriptor.Method.GetCustomAttributes<Attribute>(true), true);
            this._handlerAttributes = new Lazy<IEnumerable<Attribute>>(() => descriptor.Method.DeclaringType.GetCustomAttributes<Attribute>(true), true);

            // cache general attributes
            this._priority = new Lazy<int>(() => this.GetAttribute<PriorityAttribute>(true)?.Priority ?? 0);
            this._caseSensitivityOverride = new Lazy<bool?>(() => this.GetAttribute<CaseSensitivityAttribute>(true)?.CaseSensitive);
            this._prefixOverride = new Lazy<string>(() => this.GetAllAttributes<PrefixAttribute>(true).LastOrDefault(attr => attr.PrefixOverride != null)?.PrefixOverride);
            this._prefixRequirementOverride = new Lazy<PrefixRequirement?>(() => this.GetAllAttributes<PrefixRequirementAttribute>(true).LastOrDefault(attr => attr.PrefixRequirementOverride != null)?.PrefixRequirementOverride);
            this._requirements = new Lazy<IEnumerable<CommandRequirementAttribute>>(() => this.GetAllAttributes<CommandRequirementAttribute>(true));

            // cache help attributes
            this._displayName = new Lazy<string>(this.GetDisplayName);
            this._summary = new Lazy<string>(() => this.GetAttribute<SummaryAttribute>(false)?.Text);
            this._hidden = new Lazy<bool>(() => this.GetAttribute<HiddenAttribute>(true) != null);
            this._helpCategory = new Lazy<HelpCategoryAttribute>(() => this.GetAttribute<HelpCategoryAttribute>(true));
            this._helpOrder = new Lazy<int?>(() => this.GetAttribute<HelpOrderAttribute>(true)?.Order);
        }

        private string GetDisplayName()
        {
            DisplayNameAttribute displayNameAttribute = GetAttribute<DisplayNameAttribute>(false);
            if (displayNameAttribute != null)
                return displayNameAttribute.Text;

            if (this.Descriptor.Attribute is CommandAttribute command)
                return command.Text;
            if (this.Descriptor.Attribute is RegexCommandAttribute regex)
                return regex.Pattern;
            return null;
        }

        /// <summary>Gets all custom attributes of specified type.</summary>
        /// <typeparam name="T">Type of attributes.</typeparam>
        /// <param name="includeHandlerAttributes">Whether handler attributes should also be checked.</param>
        /// <returns>Enumerable of found attributes.</returns>
        public IEnumerable<T> GetAllAttributes<T>(bool includeHandlerAttributes = false) where T : Attribute
        {
            IEnumerable<T> FilterAttributes(IEnumerable<Attribute> allAttributes)
                => allAttributes?.Where(attr => attr is T).Cast<T>() ?? Enumerable.Empty<T>();

            IEnumerable<T> methodAttributes = FilterAttributes(this._commandAttributes.Value);
            if (!includeHandlerAttributes)
                return methodAttributes;

            // union handler attributes BEFORE the method attributes
            // this will ensure that GetAttribute will prioritize method attributes (LastOrDefault())
            IEnumerable<T> handlerAttributes = FilterAttributes(this._handlerAttributes.Value);
            return handlerAttributes.Union(methodAttributes);
        }

        /// <summary>Gets single custom attribute of specified type.</summary>
        /// <typeparam name="T">Type of attribute.</typeparam>
        /// <param name="includeHandlerAttributes">Whether handler attributes should also be checked.</param>
        /// <returns>Found attribute; null if not found.</returns>
        public T GetAttribute<T>(bool includeHandlerAttributes = false) where T : Attribute
            => GetAllAttributes<T>(includeHandlerAttributes).LastOrDefault();

        public static DescriptorAttributeCache GetCache(ICommandInstanceDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (_cache.TryGetValue(descriptor, out DescriptorAttributeCache result))
                return result;
            result = new DescriptorAttributeCache(descriptor);
            _cache.Add(descriptor, result);
            return result;
        }
    }
}
