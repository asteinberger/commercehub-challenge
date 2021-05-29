## Overview
As part of a new application suite, a client has requested that we expose a lightweight day-planner feature, which you've been asked to implement.

## Problem

Consider the following interface:
```csharp
public interface IPlanner
{
    Event GetNextEvent(DateTime start);

    bool TrySchedule(Event scheduledEvent, out IEnumerable<Event> conflicts);
}
```
We need to implement the interface members as well as a factory-style method to initialize the scheduler from a JSON representation.  A stub has been provided.

### Requirements
Using the stubbed `Planner` class, implement all methods as indicated.

1. The `GetNextEvent` should only take into account those scheduled events with a `Start` value in the future.  Events already in progress should not be considered.
2. The `GetNextEvent` should return `null` if no event matching the given criteria can be identified.
3. The `TrySchedule` method should return `true` if the method was scheduled successfully; otherwise `false` should be returned.  In the case of a `false` return value, a list of *all* conflicting `Event` objects should be returned in the output variable.
4. The `FromJson` event should expect a JSON array of `Event` objects.  Example to follow.

```
[
    {
        "Start":"2019-05-05T13:00:00",
        "Duration":"00:30:00",
        "Name":"Birthday Party"
    },
    {
        "Start":"2019-05-05T15:30:00",
        "Duration":"00:30:00",
        "Name":"Appointment"
    }
]
```

### Limitations and constraints
None identified.