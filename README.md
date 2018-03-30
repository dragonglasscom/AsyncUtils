# AsyncUtils
A set of utilities for Asynchronous programming in Unity.


## Motivation
Use clean, async style web requests with better exception handling instead of coroutine hell.
Provide easier task composition and cancellation.

## Thread safety
Most of the entities in Unity API are not thread safe.
Methods from this library are thread aware and will be pushed to Unity's Main thread's synchronization context for execution.
Cancellation of requests is also handled in the safe manner.

## Installation
Navigate to Player Settings and make sure your Scripting Runtime Version is set to .NET 4.6 Equivalent.
Copy  Assets/Plugins folder into your Project's Assets folder.

## References
The library is based on [this](https://assetstore.unity.com/packages/tools/async-await-support-101056) asset.
It was enhanced with support for true async style Http requests.
