using System;

namespace MemBus.Support
{
    /// <summary>
    /// Used to mark methods or classes to be useful in the context of an available API. 
    /// This is useful for NDepend code analysis to mark items that could be private as they do not seem to be used
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property)]
    public class ApiAttribute : Attribute
    {
        
    }
}