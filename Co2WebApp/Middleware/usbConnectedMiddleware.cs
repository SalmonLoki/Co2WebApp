using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using co2Device;

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
				if (!_cache.TryGetValue(CacheKeys.Co2Result, out Result co2Result) | !_cache.TryGetValue(CacheKeys.TemperatureResult, out Result temperatureResult)) {
					var hidConnection = new HidConnection();
					hidConnection.ConnectDevice(_co2DeviceHandler, _dataProcessor, VendorId, ProductId, 
					                            ref co2Result, ref temperatureResult);

					//co2Result = new Result("Relative Concentration of CO2", 1000, DateTime.Now.ToLongTimeString());
					//temperatureResult = new Result("Ambient Temperature", 25, DateTime.Now.ToLongTimeString());
					
					MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
						.SetSlidingExpiration(TimeSpan.FromSeconds(1));
					_cache.Set(CacheKeys.Co2Result, co2Result, cacheEntryOptions);
					_cache.Set(CacheKeys.TemperatureResult, temperatureResult, cacheEntryOptions);
				}
				
				httpContext.Response.ContentType = "text/html; charset=utf-8";
				await httpContext.Response.WriteAsync($"Relative Concentration of CO2: {co2Result.Value} ({co2Result.Heartbeat}), Ambient Temperature: {temperatureResult.Value} ({temperatureResult.Heartbeat})");
			} else {
				await _next.Invoke(httpContext);
			}
		}
	}
}