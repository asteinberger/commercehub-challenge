using System.Collections.Generic;

namespace Exercise.Shipping
{
	public interface IShipmentApiGateway
	{
		IEnumerable<ShipmentDispatchResult> Notify(IList<string> trackingNumbers);
	}
}