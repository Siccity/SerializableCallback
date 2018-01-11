### SerializableCallback
Lets you drag-and-drop methods with or without return values / parameters in the Unity inspector.
It uses expression trees and reflection to cache a delegate on first execution.

Usage is identical to UnityEvent

![unity_inspector](https://user-images.githubusercontent.com/6402525/34294989-46de127e-e70b-11e7-84f0-99bc4525a8f5.png)
```csharp
public class MyClass : MonoBehaviour {
    //These fields are shown in the inspector
    public SerializableCallback callback; // supports all non-void return types
    public Condition condition; // supports bool return types only
    public GetProduct getProduct; // supports MyProduct return types only

    void Start() {
        // Callbacks can be invoked with or without parameters, and with different types
        Debug.Log(callback.Invoke()); // returns object
        Debug.Log(condition.Invoke()); // returns bool
        Debug.Log(getProduct.Invoke(2)); // returns MyProduct
    }

    // As with UnityEvents, custom callbacks must have a non-generic wrapper class marked as [Serializable] in order to be serialized by Unity
    [Serializable]
    public class Condition : SerializableCallback<bool> {}

    // Last generic type parameter is the return type, staying consistent with System.Func
    [Serializable]
    public class GetProduct : SerializableCallback<int, MyProduct> {}
}
```

| Performance (100000 iterations)              | Time      |
| -------------------------------------------- | --------- |
| bool Method(float)                           | 00.00304s |
| SerializedCallback<float, bool> (Persistent) | 00.01026s |
| SerializedCallback<float, bool> (Dynamic)    | 00.00797s |

Join the [Discord](https://discord.gg/qgPrHv4 "Join Discord server") server to leave feedback or get support.
