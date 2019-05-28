using System.Net.Http;

namespace LightController {
	public class Connection {
		private const string Url = "http://192.168.43.67/light/on";

		public async void sendSignalToLightConroller(int co2, double temperature) {
			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Get, Url);

			try {
				HttpResponseMessage unused = await client.SendAsync(request);
			} catch (HttpRequestException ex) { }
		}
	}
}