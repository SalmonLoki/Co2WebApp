namespace Co2WebApp.Services {
	public class DataProcessorService {
		protected internal IDataProcessor DataProcessor { get; }

		public DataProcessorService(IDataProcessor dataProcessor) {
			DataProcessor = dataProcessor;
		}
	}
}