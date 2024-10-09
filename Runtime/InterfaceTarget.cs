using System;
using UnityEngine;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public class InterfaceTarget<TInterface> where TInterface : class
    {
        [SerializeField] private Component target;
        
        public Component TargetComponent => target;
        public TInterface Target => GetCachedTarget();
        
        private TInterface _cachedInterface;
        
        public static implicit operator TInterface(InterfaceTarget<TInterface> target) => target.Target; 
        
        public bool TryGetTarget(out TInterface targetInterface) {
            targetInterface = GetCachedTarget();
            return targetInterface != null;
        }
        
        public bool TryGetComponent(out Component component) {
            component = target;
            return component != null;
        }
        
        private TInterface GetCachedTarget()
        {
            if (target == null) return null;
            return _cachedInterface ??= target as TInterface ?? target.GetComponent<TInterface>();
        }
        
        public bool ValidateInput()
        {
            if (target == null)
            {
                return false;
            }
            
            NormalizeInput();
            return target is TInterface || target.TryGetComponent(out TInterface _);
        }
        
        private void NormalizeInput() {
            if (target == null) return;
            if (target is TInterface) return;
            
            if (target.TryGetComponent<TInterface>(out var component))
                target = component as Component;
        }

        
    }
}