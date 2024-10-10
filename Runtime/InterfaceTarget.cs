using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public class InterfaceTarget<TInterface> : ISerializationCallbackReceiver where TInterface : class
    {
        [SerializeField] private Component target;
        
        [NonSerialized] private TInterface _cachedInterface;
        
        public Component TargetComponent => target;
        public TInterface Target => GetCachedTarget();
        
        public static implicit operator TInterface(InterfaceTarget<TInterface> field) => field.Target; 

        #region Accessors
        public bool SetTarget(Component newTarget) {
            target = newTarget;
            return ValidateInput();
        }
        
        public bool TryGetTarget(out TInterface targetInterface) {
            targetInterface = GetCachedTarget();
            return targetInterface != null;
        }
        
        private TInterface GetCachedTarget()
        {
            if (target == null) return null;
            return _cachedInterface ??= target as TInterface ?? target.GetComponent<TInterface>();
        }
        #endregion
        
        #region Validation
        /// <summary>
        /// Returns true if the field references a valid object that implements the interface
        /// </summary>
        /// <returns>True if a valid reference is present, otherwise returns false</returns>
        public bool ValidateInput() {
            if (target == null)
                return false;
            
            NormalizeInput();
            return target is TInterface;
        }
        
        private void NormalizeInput() {
            if (target == null) return;
            if (target is TInterface) return;
            
            if (target.TryGetComponent<TInterface>(out var component))
                target = component as Component;
        }
        #endregion

        #region Serialization
        public void OnBeforeSerialize() {
            //noop
        }

        public void OnAfterDeserialize() {
            _cachedInterface = null;
        }
        #endregion
    }
}