using UnityEngine;

namespace DGP.InterfaceTargets.Editor
{
    
    public class TestMono : MonoBehaviour
    {
        [SerializeField] private InterfaceTarget<MyInterface> target;
        [SerializeField] private InterfaceTargets<MyInterface> targets;
        
    }
}