using System;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public interface ICommandInitializerMap
    {
        ICommandInitializer GetMappedInitializer(Type commandAttributeType);
        void MapInitializer(Type commandAttributeType, ICommandInitializer initializer);
    }
}
