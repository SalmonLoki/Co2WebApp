using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using co2Device;
using LightController;
using Newtonsoft.Json;

namespace Co2WebApp.Middleware {
	public class UsbConnectedMiddleware {
		private const int VendorId = 0x04d9;
		private const int ProductId = 0xa052;
		
		private readonly ICo2DeviceHandler _co2DeviceHandler;
		private readonly IDataProcessor _dataProcessor;
		private readonly RequestDelegate _next;
		private readonly IMemoryCache _cache;

		public UsbConnectedMiddleware(RequestDelegate next, IDataProcessor dataProcessor, ICo2DeviceHandler co2DeviceHandler, IMemoryCache memoryCache) {
			_next = next;
			_cache = memoryCache;
			_dataProcessor = dataProcessor;
			_co2DeviceHandler = co2DeviceHandler;
		}
 
		public async Task Invoke(HttpContext httpContext) {
			if (httpContext.Request.Path.Value.ToLower() == "/co2") {
				if (!_cache.TryGetValue(CacheKeys.JsonOutput, out JsonOutput jsonOutput)) {
					#region defaultValues

					jsonOutput = new JsonOutput(1000, 25.5, DateTime.Now.ToString("g"));

					#endregion
					
					//var hidConnection = new HidConnection();
					//hidConnection.ConnectDevice(_co2DeviceHandler, _dataProcessor, VendorId, ProductId, ref jsonOutput);
									
					MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
						.SetSlidingExpiration(TimeSpan.FromSeconds(1));
					_cache.Set(CacheKeys.JsonOutput, jsonOutput, cacheEntryOptions);
				}
								
				httpContext.Response.ContentType = "application/json";
				string jsonString = JsonConvert.SerializeObject(jsonOutput);
				
				var lightConnection = new Connection();
				lightConnection.sendSignalToLightConroller(jsonOutput.co2, jsonOutput.temperature);
				
				await httpContext.Response.WriteAsync(jsonString);
			} else {
				await _next.Invoke(httpContext);
			}
		}
	}
}