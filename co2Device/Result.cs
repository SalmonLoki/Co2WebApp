namespace co2Device {
	public class Result {
		public string Type { get; }
		public double Value { get; }
		public string Heartbeat { get; }

		public Result(string type, double value, string heartbeat) {
			Type = type;
			Value = value;
			Heartbeat = heartbeat;
		}
	}
}