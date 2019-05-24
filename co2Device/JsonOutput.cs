namespace co2Device {
    public class JsonOutput {
        public JsonOutput(int co2, double temperature, string time) {
            this.co2 = co2;
            this.temperature = temperature;
            this.time = time;
        }

        public int co2 { get; }
        public double temperature { get; }
        public string time { get; }        
    }
}