using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public class InterfaceTargets<TInterface> where TInterface : class
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
            if (_cachedInterfaces == null)
            {
                _cachedInterfaces = new List<TInterface>();
                foreach (var target in targets)
                {
                    if (target == null)
                        continue;
                    
                    if (target is TInterface)
                        _cachedInterfaces.Add(target as TInterface);
                    else
                        _cachedInterfaces.Add(target.GetComponent<TInterface>());
                }
            }

            return _cachedInterfaces;
        }

        public bool ValidateInput()
        {
            NormalizeInput();
            
            foreach (var target in targets)
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
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                    continue;
                
                if (targets[i] is TInterface)
                    continue;
                
                var component = targets[i].GetComponent<TInterface>();
                if (component != null)
                    targets[i] = component as Component;
            }
        }

    }
}