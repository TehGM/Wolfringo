using System;

namespace TehGM.Wolfringo.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CaseInsensitiveAttribute : Attribute
    {
        public bool CaseInsensitive { get; }

        public CaseInsensitiveAttribute(bool caseInsensitive)
        {
            this.CaseInsensitive = caseInsensitive;
        }
    }
}
