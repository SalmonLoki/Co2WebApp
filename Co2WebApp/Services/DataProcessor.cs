using System;
using Co2WebApp.Models;

namespace Co2WebApp {
    public class DataProcessor : IDataProcessor {
        public int[] decryptData(ref byte[] key, ref byte[] dataBuffer) {
            int[] shuffle = { 2, 4, 0, 7, 1, 6, 5, 3 };
			
            int[] phase1 = { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (var i = 0; i < shuffle.Length; i++) {
                phase1[shuffle[i]] = dataBuffer[i];
            }    
			
            int[] phase2 = { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (var i = 0; i < 8; i++) {
                phase2[i] = phase1[i] ^ key[i];
            }
			
            int[] phase3 = { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (var i = 0; i < 8; i++) {
                phase3[i] = ((phase2[i] >> 3) | (phase2[(i + 7) % 8] << 5)) & 0xff;
            }
			
            int[] cate = { 0x48, 0x74, 0x65, 0x6D, 0x70, 0x39, 0x39, 0x65 };
            int[] tmp = { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (var i = 0; i < 8; i++) {
                tmp[i] = ((cate[i] >> 4) | (cate[i] << 4)) & 0xff;
            }
			
            int[] result = { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (var i = 0; i < 8; i++) {
                result[i] = (0x100 + phase3[i] - tmp[i]) & 0xff;
            }
            return result;    
        }

        public bool checkEndOfMessage(ref int[] data) {
            return data[4] == 0x0d;
        }
		
        public bool checkCheckSum(ref int[] data) {
            return ((data[0] + data[1] + data[2]) & 0xff) == data[3];            
        }
		
        private double decodeTemperature(int t) {
            return Math.Round(t * 0.0625 - 273.15, 1);
        }
				
        private string getHeartbeat() {
            string curTimeLong = DateTime.Now.ToLongTimeString();
            return curTimeLong;
        }

        public Result dataProcessing(ref int[] data) {
            int val = (data[1] << 8) | data[2];
			
            switch (data[0]) {
                case 80: //0x50d		
                    return new Result(type: "Relative Concentration of CO2", val, getHeartbeat());
                    break;	
				
                case 66: //0x42d					
                    return new Result(type: "Ambient Temperature", decodeTemperature(val), getHeartbeat());
                    break;
                
                default:
                    return null;
                    break;
            }
        }    
    }
}