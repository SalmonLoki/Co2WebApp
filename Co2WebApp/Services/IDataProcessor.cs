using Co2WebApp.Models;

namespace Co2WebApp {
	public interface IDataProcessor {
		int[] decryptData(ref byte[] key, ref byte[] dataBuffer);
		
		bool checkEndOfMessage(ref int[] data);

		bool checkCheckSum(ref int[] data);

		Result dataProcessing(ref int[] data);
	}
}