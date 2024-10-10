using UnityEngine;

namespace DGP.InterfaceTargets
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class RequireTargetAttribute : PropertyAttribute
    {
        public bool IsRequired { get; }

        public RequireTargetAttribute(bool isRequired = true)
        {
            IsRequired = isRequired;
        }
    }
}