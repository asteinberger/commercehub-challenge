using Xunit;
using Exercise.Shipping;
using Exercise.Scheduling;
using Partner.Integrations.Shipments;
using System;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Exercise.Tests
{
	public sealed class Tests
	{
        #region Scheduling Tests and Artifacts

        private const string SampleEventsJson =
			"[\n" +
			"\t{\n" +
			"\t\t\"Start\":\"2019-05-05T13:00:00\",\n" +
			"\t\t\"Duration\":\"00:30:00\",\n" +
			"\t\t\"Name\":\"Birthday Party\"\n" +
			"\t},\n" +
			"\t{\n" +
			"\t\t\"Start\":\"2019-05-05T15:30:00\",\n" +
			"\t\t\"Duration\":\"00:30:00\",\n" +
			"\t\t\"Name\":\"Appointment\"\n" +
			"\t}\n" +
			"]";

		[Fact]
		public void Planner_GetNextEvent_IsImplemented()
        {
			var stream = MemoryStreamConverter.ConvertStringToMemoryStream(SampleEventsJson);
			Planner.FromJson(stream);
			Planner.Instance.GetNextEvent(new DateTime(2020, 1, 1));
		}

		[Fact]
		public void Planner_GetNextEvent_ReturnsAFutureResult()
		{
			var startDate = DateTime.Parse("2019-05-05T15:00:00");
			var futureEvent = new Event()
			{
				Start = DateTime.Parse("2019-05-05T15:30:00"),
				Duration = TimeSpan.Parse("00:30:00"),
				Name = "Appointment"
			};
			var stream = MemoryStreamConverter.ConvertStringToMemoryStream(SampleEventsJson);
			Planner.FromJson(stream);
			var nextEvent = Planner.Instance.GetNextEvent(startDate);
			nextEvent.Should().BeEquivalentTo(futureEvent);
		}

		[Fact]
		public void Planner_GetNextEvent_ReturnsNoFutureResults()
		{
			var startDate = DateTime.Parse("2019-05-05T15:30:00");
			var stream = MemoryStreamConverter.ConvertStringToMemoryStream(SampleEventsJson);
			Planner.FromJson(stream);
			var nextEvent = Planner.Instance.GetNextEvent(startDate);
			nextEvent.Should().BeNull();
		}

		[Fact]
		public void Planner_TrySchedule_IsImplemented()
		{
			var stream = MemoryStreamConverter.ConvertStringToMemoryStream(SampleEventsJson);
			Planner.FromJson(stream);
			var testEvent = new Event();
			IEnumerable<Event> conflicts;
			Planner.Instance.TrySchedule(testEvent, out conflicts);
		}

		[Fact]
		public void Planner_TrySchedule_SchedulesAnEvent()
		{
			var stream = MemoryStreamConverter.ConvertStringToMemoryStream(SampleEventsJson);
			Planner.FromJson(stream);
			var testEvent = new Event()
			{
				Start = DateTime.Parse("2019-05-05T14:00:00"),
				Duration = TimeSpan.Parse("00:30:00"),
				Name = "Business Meeting"
			};
            var wasTheEventScheduled = Planner.Instance.TrySchedule(testEvent, out IEnumerable<Event> conflicts);
			wasTheEventScheduled.Should().BeTrue();
		}

		[Fact]
		public void Planner_TrySchedule_FindsConflicts()
		{
			var stream = MemoryStreamConverter.ConvertStringToMemoryStream(SampleEventsJson);
			Planner.FromJson(stream);
			var testEvent = new Event()
			{
				Start = DateTime.Parse("2019-05-05T13:15:00"),
				Duration = TimeSpan.Parse("00:30:00"),
				Name = "Business Meeting"
			};
			var wasTheEventScheduled = Planner.Instance.TrySchedule(testEvent, out IEnumerable<Event> conflicts);
			wasTheEventScheduled.Should().BeFalse();
			conflicts.ToList().Count().Should().Be(1);
		}

		[Fact]
		public void Planner_FromJson_ConvertsToJsonFromMemoryStream()
        {
			var stream = MemoryStreamConverter.ConvertStringToMemoryStream(SampleEventsJson);
			Planner.FromJson(stream);
        }

		[Fact]
		public void MemoryStreamConverter_ConvertStringToMemoryStream_IsImplemented()
        {
			MemoryStreamConverter.ConvertStringToMemoryStream("12345");
		}

		public static class MemoryStreamConverter
		{
			public static MemoryStream ConvertStringToMemoryStream(string text)
			{
				var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
				return memoryStream;
			}
		}

        #endregion

        #region Shipping Tests and Artifacts

        private const string InvalidTrackingNumber = "12345";

		[Fact]
		public void ShipmentApiGateway_Constructor_CanThrowAnException()
        {
			ShipmentApiGateway shipmentApiGateway;
			Action action = () => shipmentApiGateway = new ShipmentApiGateway(null);
			action.Should().Throw<ArgumentNullException>();
        }

		[Fact]
		public void ShipmentApiGateway_Notify_IsImplemented()
		{
			var shipmentDispatcher = new TestShipmentDispatcher();
			var shipmentApiGateway = new ShipmentApiGateway(shipmentDispatcher);
			var trackingNumbers = new string[] { "12345" };
			shipmentApiGateway.Notify(trackingNumbers);
		}

		[Fact]
		public void ShipmentApiGateway_Notify_ThrowsAnErrorWhenBatchSizeIsGreaterThan100()
		{
			var shipmentDispatcher = new TestShipmentDispatcher();
			var shipmentApiGateway = new ShipmentApiGateway(shipmentDispatcher);
			var trackingNumbers = RandomTrackingNumbersGenerator.GenerateTrackingNumbers(101);
			Action action = () => shipmentApiGateway.Notify(trackingNumbers);
			action.Should().Throw<Exception>().WithMessage("The maximum batch size is 100.");
		}

		[Fact]
		public void ShipmentApiGateway_Notify_DoesNotThrowAnErrorWhenBatchSizeIs100()
		{
			var shipmentDispatcher = new TestShipmentDispatcher();
			var shipmentApiGateway = new ShipmentApiGateway(shipmentDispatcher);
			var trackingNumbers = RandomTrackingNumbersGenerator.GenerateTrackingNumbers(100);
			shipmentApiGateway.Notify(trackingNumbers);
		}

		[Fact]
		public void ShipmentApiGateway_Notify_AcceptsOnlyValidTrackingNumbers()
		{
			var shipmentDispatcher = new TestShipmentDispatcher();
			var shipmentApiGateway = new ShipmentApiGateway(shipmentDispatcher);
			var firstSuccessfulTrackingNumber = "22222";
			var trackingNumbers = new string[] { InvalidTrackingNumber, firstSuccessfulTrackingNumber, "33333", "44444", "55555" };
			List<ShipmentDispatchResult> notifyResults = (List<ShipmentDispatchResult>)shipmentApiGateway.Notify(trackingNumbers);
			var countOfSuccesses = notifyResults.Count(x => x.Success);
			countOfSuccesses.Should().Be(4);
			var firstSuccess = notifyResults.Where(x => x.Success).ElementAt(0);
			firstSuccess.TrackingNumber.Should().Be(firstSuccessfulTrackingNumber);
		}

		[Fact]
		public void RandomTrackingNumbersGenerator_GeneratesTheCorrectSizeArray()
		{
			var targetArraySize = 20;
			var arrayOfTrackingNumbers = RandomTrackingNumbersGenerator.GenerateTrackingNumbers(targetArraySize);
			arrayOfTrackingNumbers.Should().HaveCount(targetArraySize);
		}

		[Fact]
		public void RandomTrackingNumbersGenerator_GeneratesNumbersOnly()
		{
			var targetArraySize = 20;
			var arrayOfTrackingNumbers = RandomTrackingNumbersGenerator.GenerateTrackingNumbers(targetArraySize);
			int.Parse(arrayOfTrackingNumbers[5]);
		}

		public class TestShipmentDispatcher : IShipmentDispatcher
		{
			void IShipmentDispatcher.Notify(string[] trackingNumbers)
			{
				foreach (string trackingNumber in trackingNumbers)
				{
					if (trackingNumber == InvalidTrackingNumber)
					{
						throw new ShipmentDispatchException();
					}
				}
			}
		}

		public static class RandomTrackingNumbersGenerator
		{
			public static string[] GenerateTrackingNumbers(int size)
			{
				var random = new Random();
				var outputArray = new string[size];
				for (int arrayIndex = 0; arrayIndex < size; arrayIndex++)
				{
					outputArray[arrayIndex] = random.Next(0, 100000).ToString("D5");
				}
				return outputArray;
			}
		}

		#endregion
	}
}
