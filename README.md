# AsyncUtils
A set of utilities for Asynchronous programming in Unity.  


## Motivation
Use clean, async style web requests with better exception handling instead of coroutine hell.  
Provide easier task composition and cancellation.  
Provide easy to implement conversion and compatibility with older Coroutine style code.

## Thread safety
Most of the entities in Unity API are not thread safe.  
Methods from this library are thread aware and will be pushed to Unity's Main thread's synchronization context for execution.  
Cancellation of requests is also handled in the safe manner.  

## Installation
Navigate to Player Settings and make sure your Scripting Runtime Version is set to .NET 4.6 Equivalent.  
Copy  Assets/Plugins folder into your Project's Assets folder.  

## Usage

### Making Web Requests

Simply call WebrequestUtils.<HttpMethodName>();  
Theese methods are awaitable, provide cancellation, and are thread safe (posted onto main thread).  

```c#

public async void Start()
{
    var req = await WebRequestUtils.Get("https://ipinfo.io");

    Debug.Log(req.ReadToEnd());
}
```

### Awaiting coroutines
With .net 4.6 script runtime anything that returns IEnumerator is awaitable.  
Can be used like so:  
```c#

public async void Start()
{
    var req = await WaitForSomething();
}
    
public IEnumerator WaitForSomething()
{
     while(isSomethingCompleded)
         yield return null;
}

```

## References
The library is based on [this](https://assetstore.unity.com/packages/tools/async-await-support-101056) asset.  
It was enhanced with support for true async style Http requests.  
