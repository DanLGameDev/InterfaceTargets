using System;
using System.Collections.Generic;
using UnityEngine;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public class InterfaceTargets<TInterface> where TInterface : class
    {
        [SerializeField] private List<Component> targetComponents = new List<Component>();

        private List<TInterface> _cachedComponents;
        
        public int Count => targetComponents.Count;
        public IReadOnlyList<Component> TargetComponents => targetComponents;
        public IEnumerable<TInterface> Targets
        {
            get 
            {
                foreach (var target in targetComponents)
                {
                    if (target == null)
                        continue;
                    
                    if (target is TInterface)
                        yield return target as TInterface;
                    else
                        yield return target.GetComponent<TInterface>();
                    
                }
            }
        }
        
        public List<TInterface> ToList()
        {
            if (_cachedComponents == null)
            {
                _cachedComponents = new List<TInterface>();
                foreach (var target in targetComponents)
                {
                    if (target == null)
                        continue;
                    
                    if (target is TInterface)
                        _cachedComponents.Add(target as TInterface);
                    else
                        _cachedComponents.Add(target.GetComponent<TInterface>());
                }
            }

            return _cachedComponents;
        }

        public bool ValidateInput()
        {
            NormalizeInput();
            
            foreach (var target in targetComponents)
            {
                if (target == null || !target.TryGetComponent(out TInterface _))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private void NormalizeInput()
        {
            for (int i = 0; i < targetComponents.Count; i++)
            {
                if (targetComponents[i] == null)
                    continue;
                
                if (targetComponents[i] is TInterface)
                    continue;
                
                var component = targetComponents[i].GetComponent<TInterface>();
                if (component != null)
                    targetComponents[i] = component as Component;
            }
        }

    }
}