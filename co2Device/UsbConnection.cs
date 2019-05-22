using System;
using System.Linq;
using HidSharp;

namespace co2Device {
    public class UsbConnection {
        private HidDevice _hidDevice;
        private HidStream _stream;
        private byte[] _key = { 0xc4, 0xc6, 0xc0, 0x92, 0x40, 0x23, 0xdc, 0x96 };
        
        public void ConnectDevice(ICo2DeviceHandler co2DeviceHandler, IDataProcessor dataProcessor,
            int vendorId, int productId,
            ref Result co2Result, ref Result temperatureResult) {
            _hidDevice = co2DeviceHandler.ConnectDevice(vendorId, productId);
            _stream = co2DeviceHandler.OpenStream(_hidDevice);
					
            //the device won't send anything before receiving this packet 
            byte[] reportId = { 0x00 };
            byte[] request = reportId.Concat(_key).ToArray();
						
            co2DeviceHandler.SendSetFeatureSetupRequest(_stream, request);
            
            while (true) {
                byte[] receivedData = co2DeviceHandler.ReadData(_stream);

                if (receivedData.Length == 0) {
                    Console.WriteLine("Unable to read data");
                }
                
                if (receivedData.Length != 9 && receivedData.Length != 8) {
                    Console.WriteLine("transferred amount of bytes != expected bytes amount: " + receivedData.Length);
                }

                if (receivedData.Length == 9)
                {
                    var temp = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
                    for (var i = 0; i < 8; i++)
                    {
                        temp[i] = receivedData[i + 1];
                    }

                    receivedData = temp;
                }

                int[] data = dataProcessor.DecryptData(ref _key, ref receivedData);
                if (!dataProcessor.CheckEndOfMessage(ref data)) {
                    Console.WriteLine("Unexpected data from device");
                }
	               
                if (!dataProcessor.CheckCheckSum(ref data)) {
                    Console.WriteLine("checksum error");
                }
	               
                Result result = dataProcessor.DataProcessing(ref data);
                if (result != null) {
                    if (result.Type.Equals("Relative Concentration of CO2"))
                        co2Result = result;
                    if (result.Type.Equals("Ambient Temperature"))
                        temperatureResult = result;
                }

                if (co2Result != null & temperatureResult != null)
                    break;
            }				
            co2DeviceHandler.CloseStream(_stream);
        }
    }
}