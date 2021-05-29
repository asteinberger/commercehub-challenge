using System;

namespace Exercise.Shipping
{
	/// <summary>Represents the result of a shipment notification.</summary>
	public sealed class ShipmentDispatchResult
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ShipmentDispatchResult"/> class.
		/// </summary>
		/// <param name="trackingNumber">The tracking number.</param>
		/// <param name="success">The submission was successful if set to <c>true</c>, otherwise <c>false</c>.</param>
		public ShipmentDispatchResult(string trackingNumber, bool success)
		{
			TrackingNumber = trackingNumber;
			Success = success;
		}

		/// <summary>Gets or sets the tracking number.</summary>
		/// <value>The tracking number.</value>
		public string TrackingNumber { get; }
		
		/// <summary>Gets or sets a value indicating whether this <see cref="ShipmentDispatchResult"/> is success.</summary>
		/// <value>
		///   <c>true</c> if operation was successful; otherwise, <c>false</c>.</value>
		public bool Success { get; }

		/// <summary>Gets or sets the exception, if one was caught.</summary>
		/// <value>The exception, or <c>null</c> if the operation was successful.</value>
		public Exception Exception { get; set; }
	}
}