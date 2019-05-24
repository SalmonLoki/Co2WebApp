namespace co2Device {
	public interface IDataProcessor {
		int[] DecryptData(ref byte[] key, ref byte[] dataBuffer);
		
		bool CheckEndOfMessage(ref int[] data);

		bool CheckCheckSum(ref int[] data);

		void DataProcessing(ref int[] data, ref int co2);
		void DataProcessing(ref int[] data, ref double temperature);
	}
}