using System;

namespace MemBus.Support
{
    /// <summary>
    /// Used to mark a class as public even though it does not need to be. This is
    /// e.g. when the class is used to reflect upon
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MustBePublicAttribute : Attribute
    {
        
    }
}