using HidSharp;

namespace co2Device {
	public interface ICo2DeviceHandler {
		HidDevice ConnectDevice(int vendorId, int productId);

		HidStream OpenStream(HidDevice hidDevice);

		void CloseStream(HidStream stream);

		void SendSetFeatureSetupRequest(HidStream stream, byte[] buffer);

		byte[] ReadData(HidStream stream);
	}
}