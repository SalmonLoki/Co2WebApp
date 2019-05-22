using System;
using HidSharp;

namespace Co2WebApp {
	public class Co2DeviceHandler : ICo2DeviceHandler {
		public HidDevice connectDevice(int vendorId, int productId) {
			DeviceList devices = DeviceList.Local;
			HidDevice hidDevice = devices.GetHidDeviceOrNull(vendorID: vendorId, productID: productId);
			if (hidDevice == null) {
				throw new Exception(message: "Device " + vendorId + ":" + productId + " not found");
			}
			return hidDevice;
		}

		public HidStream openStream(HidDevice hidDevice) {
			if (!hidDevice.TryOpen(out HidStream stream)) {
				throw new Exception(message: "Stream could not be created");
			}
			stream.ReadTimeout = 3000; //The maximum amount of time, in milliseconds, to wait for the device to send some data
			stream.WriteTimeout = 3000; //The maximum amount of time, in milliseconds, to wait for the device to receive the data.
			return stream;
		} 

		public void closeStream(HidStream stream) {
			stream.Close();
		}

		public void sendSetFeatureSetupRequest(HidStream stream, byte[] buffer) {
			stream.SetFeature(buffer);
		}

		public byte[] readData(HidStream stream) {
			return stream.Read();
		}
	}
}