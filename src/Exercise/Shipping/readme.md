## Overview
A new business partner has an API that we must integrate with.

The vendor has provided us a C# interface and library implementation to communicate with the API, which simply dispatches a message to a recipient to notify them that a shipment is en route. 

Messages consist only of a shipment tracking number, in the form of a `string`. The issue we're facing is that this is an older system and isn't reliable; we need to add a layer of resilience to handle errors and accurately determine the outcome of the API operations.


## Problem
Consider the following interface, provided by the partner's engineering team:
```csharp
public interface IShipmentDispatcher
{
    void Notify(string[] trackingNumbers);
}
```
***NOTE:*** This interface is provided in an included NuGet package `Partner.Integrations`.  The build script will automatically source that package from `./src/.packages/` and restore the package reference for you.  
If you rely solely on Visual Studio to perform the restore, you will need to explicitly configure your machine package source(s).


### Requirements

1. For a given batch of tracking numbers, our implementation needs to return an exact accounting of which tracking numbers were processed successfully, and which were not. 

### Limitations and constraints

The current system has the following limitations:

1. The maximum number of tracking numbers allowed to be submitted in a batch is 100.  Any more than that will cause a failure on the remote endpoint and should be guarded against.
2. The dispatch operation is idempotent.  Duplicate submissions, having previously been successful, will be silently discarded.
3. The endpoint is effectively `void` and will either throw `ShipmentDispatchException` or return cleanly.
4. Your implementation should make as many calls as necessary to return a precise result, but no more than that.