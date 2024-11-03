using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public class InterfaceTarget<TInterface> : ISerializationCallbackReceiver where TInterface : class
    {
        [SerializeField] private object _target;
        [NonSerialized] private TInterface _cachedInterface;
        
        public object TargetComponent => _target;

        public TInterface Target
        {
            get => GetCachedTarget();
            set {
                if (IsValidValueForField(value)) {
                    _target = value;
                    _cachedInterface = null;
                    NormalizeInput();
                }
            }
        }

        public static implicit operator TInterface(InterfaceTarget<TInterface> field) => field.Target; 

        #region Accessors
        public bool TryGetTarget(out TInterface targetInterface) {
            targetInterface = GetCachedTarget();
            return targetInterface != null;
        }
        
        private TInterface GetCachedTarget()
        {
            if (_target == null) return null;
            return _cachedInterface ??= _target as TInterface ?? (_target is Component cTarget ? cTarget.GetComponent<TInterface>() : null);
        }
        #endregion
        
        public bool IsValidValueForField(object value) {
            if (value == null) return true;
            if (value is TInterface) return true;
            if (value is Component component) return component.TryGetComponent<TInterface>(out _);
            
            return false;
        }
        
        public void NormalizeInput() {
            if (_target == null) return;
            if (_target is TInterface) return;

            if (_target is Component tComponent && tComponent.TryGetComponent<TInterface>(out TInterface component)) {
                _target = component;
                _cachedInterface = component;
            }
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