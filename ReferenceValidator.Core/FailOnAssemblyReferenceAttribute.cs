using System;

namespace ReferenceValidator
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class FailOnAssemblyReferenceAttribute : Attribute
    {
        public FailOnAssemblyReferenceAttribute(string assemblyName)
        {
            
        }
    }
}