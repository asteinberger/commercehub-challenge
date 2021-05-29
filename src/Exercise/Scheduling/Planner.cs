using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Exercise.Scheduling
{
	public sealed class Planner : IPlanner
	{
		private static List<Event> Events;

		public Planner()
        {
			Events = new List<Event>();
		}

		private static readonly object Lock = new object();
		private static Planner _instance;
		
		public static Planner Instance
		{
			get
			{
				lock (Lock)
				{
					_instance = _instance ?? new Planner();
					return _instance;
				}
			}
		}

		public Event GetNextEvent(DateTime start)
		{
			_ = Instance;
			return Events.OrderBy(x => x.Start).FirstOrDefault(x => x.Start > start);
		}

		public bool TrySchedule(Event scheduledEvent, out IEnumerable<Event> conflicts)
		{
			_ = Instance;
			var eventConflicts = new List<Event>();
			foreach (Event eventItem in Events.OrderBy(x => x.Start))
            {
				var eventItemStart = eventItem.Start;
				var eventItemEnd = eventItem.Start.Add(eventItem.Duration);
				var scheduledStart = scheduledEvent.Start;
				var scheduledEnd = scheduledEvent.Start.Add(scheduledEvent.Duration);

				if (!(eventItemStart <= scheduledStart && eventItemEnd <= scheduledStart)
					&& !(eventItemStart >= scheduledEnd && eventItemEnd >= scheduledEnd))
                {
					eventConflicts.Add(eventItem);
                }
			}
			conflicts = eventConflicts;
			if (eventConflicts.Any())
            {
				return false;
            }
			Events.Add(scheduledEvent);
			return true;
		}

		/// <summary>
		/// Constructs an instance of a type that implements <seealso cref="IPlanner"/>
		/// </summary>
		/// <param name="stream">A <see cref="Stream" /> containing a JSON string representing one or more <see cref="Event"/> objects </param>
		public static IPlanner FromJson(Stream stream)
		{
			_ = Instance;
			var stringBuilder = new StringBuilder();
			using (StreamReader streamReader = new StreamReader(stream))
			{
				string line;
				while ((line = streamReader.ReadLine()) != null)
                {
					stringBuilder.AppendLine(line); 
                }
			}
			var json = stringBuilder.ToString();
			Events = JsonConvert.DeserializeObject<List<Event>>(json);
			return Instance;
		}
	}
}
