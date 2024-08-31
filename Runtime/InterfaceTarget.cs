using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public class InterfaceTarget<TInterface>
    {
        [ValidateInput(nameof(ValidateInput))][InlineEditor]
        [SerializeField] private Component target;
        public Component TargetComponent => target;
        public TInterface Target => GetTargetOrNull();
        
        private bool isCached;
        private TInterface cachedComponent;
        
        public static implicit operator TInterface(InterfaceTarget<TInterface> target) => target.Target; 
        
        public bool TryGetTarget(out TInterface targetInterface) {
            targetInterface = GetTargetOrNull();
            return targetInterface != null;
        }
        
        public bool TryGetComponent(out Component component) {
            component = target;
            return component != null;
        }
        
        private TInterface GetTargetOrNull() {
            if (target == null)
                return default;
            
            if (isCached)
                return cachedComponent;
            
            cachedComponent = target.GetComponent<TInterface>();
            isCached = true;
            
            return cachedComponent;
        }
        
        private bool ValidateInput(Component value, ref string errorMessage) {
            NormalizeInput();
            
            if (value == null) {
                errorMessage = "All targets must be assigned";
                return false;
            }

            if (value != null && !target.TryGetComponent(out TInterface _)) {
                errorMessage = "All targets must have a component of type " + typeof(TInterface).Name;
                return false;
            }
            
            return true;
        }
        
        private void NormalizeInput() {
            if (target == null)
                return;

            if (target is TInterface)
                return;
            
            if (target.TryGetComponent<TInterface>(out var component))
                target = component as Component;
        }
    }
}