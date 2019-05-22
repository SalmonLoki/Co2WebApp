namespace co2Device {
	public interface IDataProcessor {
		int[] DecryptData(ref byte[] key, ref byte[] dataBuffer);
		
		bool CheckEndOfMessage(ref int[] data);

		bool CheckCheckSum(ref int[] data);

		Result DataProcessing(ref int[] data);
	}
}