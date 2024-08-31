using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public class InterfaceTargets<TInterface>
    {
        [ValidateInput(nameof(ValidateInput))]
        [SerializeField] private List<Component> targetEmitters = new();
        
        private List<TInterface> cachedComponents;
        
        public int Count => targetEmitters.Count;
        
        public IReadOnlyList<Component> TargetComponents => targetEmitters;
        public List<TInterface> Targets
        {
            get {
                if (cachedComponents == null)
                    cachedComponents = new List<TInterface>();
                
                if (cachedComponents.Count != targetEmitters.Count) {
                    cachedComponents.Clear();
                    foreach (var target in targetEmitters) {
                        cachedComponents.Add(target.GetComponent<TInterface>());
                    }
                }

                return cachedComponents;
            }
        }

        #region Validation and Normalization
        private bool ValidateInput(List<Component> values, ref string errorMessage) {
            NormalizeInput();
            
            foreach (var target in values) {
                if (target == null) {
                    errorMessage = "All targets must be assigned";
                    return false;
                }

                if (target != null && !target.TryGetComponent(out TInterface _)) {
                    errorMessage = "All targets must have a component of type " + typeof(TInterface).Name;
                    return false;
                }
            }
            
            return true;
        }
        
        private void NormalizeInput() {
            for (int i = 0; i < targetEmitters.Count; i++) {
                if (targetEmitters[i] == null)
                    continue;
                
                var component = targetEmitters[i].GetComponent<TInterface>();
                if (component != null)
                    targetEmitters[i] = component as Component;
            }
        }
        #endregion
    }
}