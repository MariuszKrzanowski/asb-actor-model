using System;

namespace MrMatrix.Net.ActorOnServiceBus.Conventions
{

    [AttributeUsage(AttributeTargets.Class)]
    public class ActorAttribute : Attribute
    {
        public string HierarchyUri { get; }

        public ActorAttribute(string hierarchyUri)
        {
            HierarchyUri = hierarchyUri;
        }
    }
}