using UnityEngine;

namespace DGP.InterfaceTargets.Editor
{
    public interface MyInterface
    {
        void MyMethod();
    }
    
    public class TestComponent : MonoBehaviour, MyInterface
    {
        public void MyMethod() {
            
        }
    }
}