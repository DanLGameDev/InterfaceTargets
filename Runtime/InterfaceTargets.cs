using System;
using System.Collections.Generic;
using UnityEngine;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public class InterfaceTargets<TInterface> : ISerializationCallbackReceiver where TInterface : class
    {
        [SerializeField] private List<Component> targets = new List<Component>();

        [NonSerialized] private List<TInterface> _cachedInterfaces;
        
        public IReadOnlyList<Component> TargetComponents => targets;
        public int Count => targets.Count;

        public IEnumerable<TInterface> Targets
        {
            get 
            {
                foreach (var target in targets)
                {
                    if (target == null) continue;
                    yield return target as TInterface ?? target.GetComponent<TInterface>();
                }
            }
        }
        
        public List<TInterface> ToList()
        {
            if (_cachedInterfaces == null)
            {
                _cachedInterfaces = new List<TInterface>();
                foreach (var target in targets)
                {
                    if (target == null) continue;
                    _cachedInterfaces.Add(target as TInterface ?? target.GetComponent<TInterface>());
                }
            }

            return _cachedInterfaces;
        }

        public bool ValidateAndNormalizeInput()
        {
            bool isValid = true;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                {
                    isValid = false;
                    continue;
                }
                
                if (targets[i] is TInterface) continue;
                
                var component = targets[i].GetComponent<TInterface>();
                if (component != null)
                    targets[i] = component as Component;
                else
                    isValid = false;
            }
            
            return isValid;
        }

        public bool TryGetTarget(int index, out TInterface targetInterface)
        {
            targetInterface = default;
            if (index < 0 || index >= targets.Count) return false;
            
            var target = targets[index];
            if (target == null) return false;
            
            targetInterface = target as TInterface ?? target.GetComponent<TInterface>();
            return targetInterface != null;
        }

        public bool IsValidValueForField(object value)
        {
            if (value == null) return true;
            if (value is TInterface) return true;
            if (value is Component component) return component.TryGetComponent<TInterface>(out _);
            return false;
        }

        public void OnBeforeSerialize()
        {
            // No operation needed
        }

        public void OnAfterDeserialize()
        {
            _cachedInterfaces = null;
        }
    }
}