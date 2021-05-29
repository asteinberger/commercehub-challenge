using System;
using System.Collections.Generic;
using Partner.Integrations.Shipments;

namespace Exercise.Shipping
{
	public sealed class ShipmentApiGateway : IShipmentApiGateway
	{
		private readonly IShipmentDispatcher dispatcher;
		private const int MaxBatchSize = 100;

		public ShipmentApiGateway(IShipmentDispatcher dispatcher)
		{
			this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
		}

		public IEnumerable<ShipmentDispatchResult> Notify(IList<string> trackingNumbers)
		{
            if (trackingNumbers.Count > MaxBatchSize)
            {
				throw new Exception($"The maximum batch size is {MaxBatchSize}.");
			}

			var shipmentDispatchResults = new List<ShipmentDispatchResult>();
            foreach (string trackingNumber in trackingNumbers)
            {
                ShipmentDispatchResult shipmentDispatchResult;
                try
                {
					this.dispatcher.Notify(new[] { trackingNumber });
					shipmentDispatchResult = new ShipmentDispatchResult(trackingNumber, true);	
				}
				catch (ShipmentDispatchException exception)
                {
                    shipmentDispatchResult = new ShipmentDispatchResult(trackingNumber, false)
                    {
                        Exception = exception
                    };
				}
				shipmentDispatchResults.Add(shipmentDispatchResult);
			}

			return shipmentDispatchResults;
		}
	}
}
