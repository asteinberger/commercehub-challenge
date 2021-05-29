using System;
using System.Diagnostics;

namespace Exercise.Scheduling
{
	/// <summary>
	/// Represents an event that can be scheduled.
	/// </summary>
	[DebuggerDisplay("{Name,nq} ({Start.ToString(\"G\"),nq} {Duration.ToString(\"c\"),nq})")]
	public sealed class Event
	{
		/// <summary>
		/// Gets or sets the start date and time of the event
		/// </summary>
		public DateTime Start { get; set; }

		/// <summary>
		/// Gets or sets the duration of the event
		/// </summary>
		public TimeSpan Duration { get; set; }

		/// <summary>
		/// Gets or sets the name of the event
		/// </summary>
		public string Name { get; set; }
	}
}