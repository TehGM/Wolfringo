using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class HandlerDescriptor
    {
        public Type Type => this.Constructor.DeclaringType;
        public ConstructorInfo Constructor { get; }
        public object[] ConstructorParams { get; }

        public HandlerDescriptor(ConstructorInfo ctor, IEnumerable<object> parameters)
        {
            this.Constructor = ctor;
            this.ConstructorParams = parameters.ToArray();
        }

        public object CreateInstance()
            => this.Constructor.Invoke(this.ConstructorParams);
    }
}
