namespace Co2WebApp.Models {
	public class Result {
		public string type { get; }
		public double value { get; }
		
		public string heartbeat { get; set; }

		public Result(string type, double value, string heartbeat) {
			this.type = type;
			this.value = value;
			this.heartbeat = heartbeat;
		}
	}
}