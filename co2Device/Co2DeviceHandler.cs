using System;
using HidSharp;

namespace co2Device {
	public class Co2DeviceHandler : ICo2DeviceHandler {
		public HidDevice ConnectDevice(int vendorId, int productId) {
			DeviceList devices = DeviceList.Local;
			HidDevice hidDevice = devices.GetHidDeviceOrNull(vendorID: vendorId, productID: productId);
			if (hidDevice == null) {
				throw new Exception("Device " + vendorId + ":" + productId + " not found");
			}
			return hidDevice;
		}

		public HidStream OpenStream(HidDevice hidDevice) {
			if (!hidDevice.TryOpen(out HidStream stream)) {
				throw new Exception("Stream could not be created");
			}
			stream.ReadTimeout = 3000; //The maximum amount of time, in milliseconds, to wait for the device to send some data
			stream.WriteTimeout = 3000; //The maximum amount of time, in milliseconds, to wait for the device to receive the data.
			return stream;
		} 

		public void CloseStream(HidStream stream) {
			stream.Close();
		}

		public void SendSetFeatureSetupRequest(HidStream stream, byte[] buffer) {
			stream.SetFeature(buffer);
		}

		public byte[] ReadData(HidStream stream) {
			return stream.Read();
		}
	}
}