using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public sealed class InterfaceTarget<TInterface> where TInterface : class
    {
        [SerializeField] private Object target;
        
        public Object TargetComponent => target;
        public TInterface Target => GetInterfaceTarget();

        public static implicit operator TInterface(InterfaceTarget<TInterface> field) => field.Target; 

        #region Accessors
        public bool TryGetTarget(out TInterface targetInterface) {
            targetInterface = GetInterfaceTarget();
            return targetInterface != null;
        }
        
        private TInterface GetInterfaceTarget()
        {
            if (target == null) return null;
            return (target as TInterface) ?? 
                   (target as Component)?.GetComponent<TInterface>();
        }
        #endregion
    }
}