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
        
        public bool IsValidValueForField(object value) {
            if (value == null) return true;
            if (value is TInterface) return true;
            if (value is Component component) return component.TryGetComponent<TInterface>(out _);
            return false;
        }
            
        
        public void NormalizeInput() {
            if (target == null) return;
            if (target is TInterface) return;
            
            if (target.TryGetComponent<TInterface>(out var component))
                target = component as Component;
        }

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