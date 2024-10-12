This package for Unity let's you store references to interfaces in the inspector.

## Usage:
```c#
// Hold a reference to an object that implements MyInterface
[SerializeField] private InterfaceTarget<MyInterface> unrequiredRef;

// Hold a reference to an object that implements MyInterface,
//and require that it is set in the inspector (no null value allowed)
[SerializeField, RequireTarget] private InterfaceTarget<MyInterface> requiredRef;

// Hold a list of references to objects that implement MyInterface
[SerializeField] private InterfaceTargets<MyInterface> unrequiredRefs;

// Hold a list of references to objects that implement MyInterface,
// and require no empty values in the list
[SerializeField, RequireTarget] private InterfaceTargets<MyInterface> requiredRefs;

...
    
void AccessMethods() {
    MyInterface x = unrequiredRef.Target;
    
    foreach (var target in unrequiredRefs.Targets) {
        target.MethodOnInterface();
    }
} 
```

