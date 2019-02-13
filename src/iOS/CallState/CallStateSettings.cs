using System;
using CoreTelephony;

namespace SmartRoadSense.iOS
{
	public class CallStateSettings
	{
		public CallStateSettings ()
		{
		}

		public void CallEvent(CTCall call){

			if (call.CallState == call.StateConnected)
				;
				// TODO: disconnect if talking without headphones
			if (call.CallState == call.StateDialing);
				// TODO: disconnect if calling without headphones
			if (call.CallState == call.StateDisconnected);
				// TODO: reconnect if disconnected for call
			if (call.CallState == call.StateIncoming);
				// TODO: nothing?
		}
	}
}

