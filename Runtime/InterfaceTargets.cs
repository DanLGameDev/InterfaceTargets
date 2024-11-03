using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DGP.InterfaceTargets
{
    [Serializable]
    public sealed class InterfaceTargets<TInterface> where TInterface : class
    {
        [SerializeField] private List<Object> targets = new();
        public IReadOnlyList<Object> TargetComponents => targets;

        public IEnumerable<TInterface> Targets
        {
            get 
            {
                foreach (var target in targets)
                {
                    if (IsValidValue(target, out var interfaceTarget))
                        yield return interfaceTarget;
                }
            }
        }

        public bool IsValidValue(Object value, out TInterface target) 
        {
            if (value == null) {
                target = null;
                return true;
            }
            
            if (value is TInterface interfaceTarget) {
                target = interfaceTarget;
                return true;
            }

            if (value is GameObject gameObject) {
                target = gameObject.GetComponent<TInterface>();
                return target != null;
            }
            
            target = null;
            return false;
        }
    }
}