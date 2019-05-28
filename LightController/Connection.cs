using System.Net.Http;

namespace LightController {
	public class Connection {
		private const string UrlLightON = "http://192.168.43.67/light/on";
		private const string UrlLightOFF = "http://192.168.43.67/light/off";

		private async void put(string Url) {
			using (var client = new HttpClient()) {
				using (var request = new HttpRequestMessage(HttpMethod.Put, Url)) {
					try {
						HttpResponseMessage unused = await client.SendAsync(request);
					} catch (HttpRequestException ex) { }
				}				
			}	
		}		

		public async void sendSignalToLightConroller(int co2, double temperature) {
			if (co2 >= 900) {
				put(UrlLightON);
			}	
			if (co2 < 900) {
				put(UrlLightOFF);
			}
		}
	}
}