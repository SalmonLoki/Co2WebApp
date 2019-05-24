using System;
using System.Linq;
using HidSharp;

namespace co2Device {
    public class HidConnection {
        private HidDevice _hidDevice;
        private HidStream _stream;
        private byte[] _key = { 0xc4, 0xc6, 0xc0, 0x92, 0x40, 0x23, 0xdc, 0x96 };
        
        public void ConnectDevice(ICo2DeviceHandler co2DeviceHandler, IDataProcessor dataProcessor,
                                  int vendorId, int productId,
                                  ref JsonOutput output) {
            int co2 = int.MinValue;
            double temperature = double.NaN;
            
            _hidDevice = co2DeviceHandler.ConnectDevice(vendorId, productId);
            _stream = co2DeviceHandler.OpenStream(_hidDevice);
					
            //the device won't send anything before receiving this packet 
            byte[] reportId = { 0x00 };
            byte[] request = reportId.Concat(_key).ToArray();						
            co2DeviceHandler.SendSetFeatureSetupRequest(_stream, request);
            
            var attempts = 0;
            var exceptionMessage = "no attempts";
            
            while (true) {
                attempts++;
                if (attempts == 10) {
                    throw new Exception(exceptionMessage);
                }
                
                byte[] receivedData = co2DeviceHandler.ReadData(_stream);

                if (receivedData.Length == 0) {
                    exceptionMessage = "unable to read data";
                    continue;
                }
                
                if (receivedData.Length != 8 && receivedData.Length != 9) {
                    exceptionMessage = "transferred amount of bytes (" + receivedData.Length + ") != expected bytes amount (8 or 9)";
                    continue;
                }

                if (receivedData.Length == 9) {
                    var temp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (var i = 0; i < 8; i++) {
                        temp[i] = receivedData[i + 1];
                    }
                    receivedData = temp;
                }

                int[] data = dataProcessor.DecryptData(ref _key, ref receivedData);
                if (!dataProcessor.CheckEndOfMessage(ref data)) {
                    exceptionMessage = "unexpected data from device";
                    continue;
                }
	               
                if (!dataProcessor.CheckCheckSum(ref data)) {
                    exceptionMessage = "checksum error";
                    continue;
                }
	               
                dataProcessor.DataProcessing(ref data, ref co2);
                dataProcessor.DataProcessing(ref data, ref temperature);

                if (co2 != int.MinValue && !temperature.Equals(double.NaN)) {
                    output = new JsonOutput(co2, temperature, DateTime.Now.ToString("g"));
                    break;
                }                   
            }				
            co2DeviceHandler.CloseStream(_stream);
        }
    }
}