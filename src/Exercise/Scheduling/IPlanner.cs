using System;
using System.Collections.Generic;

namespace Exercise.Scheduling
{
	/// <summary>
	/// Defines methods to schedule and manage event planning.
	/// </summary>
	public interface IPlanner
	{
		/// <summary>
		/// Gets the next scheduled event, not including any in progress.  Returns <c>null</c> if none are identified.
		/// </summary>
		/// <param name="start">The reference date from which to start the search.</param>
		/// <returns>Returns the next scheduled or <c>null</c> if none was found.</returns>
		Event GetNextEvent(DateTime start);

		/// <summary>
		/// Attempts to schedule an event.
		/// </summary>
		/// <param name="scheduledEvent">The event to add to the schedule.</param>
		/// <param name="conflicts">Any conflicts that were identified.</param>
		/// <returns>Returns <c>true</c> if the event was scheduled successfully, otherwise <c>false</c>.</returns>
		/// <example>
		///    IEnumerable&lt;Event&gt; conflicts = null;
		///    var item = new Event { Name = "Party", Start = new DateTime(2019, 5, 6, 8, 0, 0), Duration = TimeSpan.FromMinutes(30) };
		///    if(planner.TrySchedule(item, out conflicts))
		///    {
		///      Console.WriteLine("Event was scheduled successfully.");
		///    }
		///    else
		///    {
		///      Console.WriteLine("Event was not scheduled due to conflicts");
		///      Console.WriteLine("Conflicts:");
		///		 foreach(var e in conflicts)
		///      {
		///        Console.WriteLine("{0} ({1}-{2})", e.Name, e.Start, e.Start + e.Duration);
		///      }
		///    }
		/// </example>
		bool TrySchedule(Event scheduledEvent, out IEnumerable<Event> conflicts);
	}
}