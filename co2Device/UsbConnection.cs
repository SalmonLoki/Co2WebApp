using System;
using System.Linq;
using Co2WebApp.Models;
using Co2WebApp.Services;
using HidSharp;

namespace co2Device
{
    public class UsbConnection
    {
        private HidDevice _hidDevice;
        private HidStream _stream;
        private byte[] _key = { 0xc4, 0xc6, 0xc0, 0x92, 0x40, 0x23, 0xdc, 0x96 };
        
        public void ConnectDevice(Co2DeviceHandler co2DeviceHandler, int vendorId, int productId)
        {
            _hidDevice = co2DeviceHandler.connectDevice(vendorId, productId);
            _stream = co2DeviceHandler.openStream(_hidDevice);
					
            //the device won't send anything before receiving this packet 
            byte[] reportId = { 0x00 };
            
            byte[] request = reportId.Concat(_key).ToArray();
						
            co2DeviceHandler.sendSetFeatureSetupRequest(_stream, request);
        }

        public void GetResults(Co2DeviceHandler co2DeviceHandler, DataProcessor dataProcessor,
            ref Result co2Result, ref Result temperatureResult)
        {
            while (true) {
                byte[] receivedData = co2DeviceHandler.readData(_stream);
                if (receivedData.Length == 0) {
                    Console.WriteLine("Unable to read data");
                }
                if (receivedData.Length != 9) {
                    Console.WriteLine("transferred amount of bytes != expected bytes amount");
                }
	
                var temp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                for (var i = 0; i < 8; i++) {
                    temp[i] = receivedData[i + 1];
                }                
                receivedData = temp;   
					
                var data = dataProcessor.decryptData(ref _key, ref receivedData);
                if (!dataProcessor.checkEndOfMessage(ref data)) {
                    Console.WriteLine("Unexpected data from device");
                }
	               
                if (!dataProcessor.checkCheckSum(ref data)) {
                    Console.WriteLine("checksum error");
                }
	               
                var result = dataProcessor.dataProcessing(ref data);
                if (result != null)
                {
                    if (result.type.Equals("Relative Concentration of CO2"))
                        co2Result = result;
                    if (result.type.Equals("Ambient Temperature"))
                        temperatureResult = result;
                }

                if (co2Result != null & temperatureResult != null)
                    break;
            }				
            co2DeviceHandler.closeStream(_stream);
        }
    }
}