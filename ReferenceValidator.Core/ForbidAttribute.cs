using System;

namespace ReferenceValidator
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ForbidAttribute : Attribute
    {
        public ForbidAttribute(string projectName)
        {
            
        }
    }
}