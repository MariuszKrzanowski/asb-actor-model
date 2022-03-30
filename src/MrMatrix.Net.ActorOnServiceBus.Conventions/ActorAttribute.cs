using System;

namespace MrMatrix.Net.ActorOnServiceBus.Conventions
{
    /// <summary>
    /// Marks class with actor attribute. Instance of the class can be used in the actor system.
    /// **NOTE** remember to register class in bootstrapper.
    /// 
    /// Assigns actor to the hierarchy. In current projects hierarchy represents topic hierarchy. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ActorAttribute : Attribute
    {
        /// <summary>
        /// Actor hierarchy representing topic hierarchy.
        /// </summary>
        public string HierarchyUri { get; }

        /// <summary>
        /// </summary>
        /// <param name="hierarchyUri">Actor hierarchy representing topic hierarchy.</param>
        public ActorAttribute(string hierarchyUri)
        {
            HierarchyUri = hierarchyUri;
        }
    }
}