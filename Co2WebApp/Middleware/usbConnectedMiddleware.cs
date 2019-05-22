using System;
using System.Linq;
using System.Threading.Tasks;
using Co2WebApp.Models;
using Co2WebApp.Services;
using HidSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using  co2Device;

namespace Co2WebApp.Middleware {
	public class usbConnectedMiddleware {
		private const int VendorId = 0x04d9;
		private const int ProductId = 0xa052;
		
		private readonly ICo2DeviceHandler _co2DeviceHandler;
		private readonly IDataProcessor _dataProcessor;
		private readonly RequestDelegate _next;
		private readonly IMemoryCache _cache;

		public usbConnectedMiddleware(RequestDelegate next, IDataProcessor dataProcessor, ICo2DeviceHandler co2DeviceHandler, IMemoryCache memoryCache) {
			_next = next;
			_cache = memoryCache;
			_dataProcessor = dataProcessor;
			_co2DeviceHandler = co2DeviceHandler;
		}
 
		public async Task Invoke(HttpContext httpContext) {
			if (httpContext.Request.Path.Value.ToLower() == "/co2")
			{
				if (!_cache.TryGetValue(CacheKeys.co2Result, out Result co2Result) | !_cache.TryGetValue(CacheKeys.temperatureResult, out Result temperatureResult)) {
					// Key not in cache, so get data.
					UsbConnection usbConnection = new UsbConnection();
					usbConnection.ConnectDevice((Co2DeviceHandler) _co2DeviceHandler, VendorId, ProductId);
					usbConnection.GetResults((Co2DeviceHandler) _co2DeviceHandler, (DataProcessor) _dataProcessor,
						ref co2Result, ref temperatureResult);
					
					//co2Result = new Result("Relative Concentration of CO2", 1000, DateTime.Now.ToLongTimeString());
					//temperatureResult = new Result("Ambient Temperature", 25, DateTime.Now.ToLongTimeString());
														
					// Set cache options.
					MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
						// Keep in cache for this time, reset time if accessed.
						.SetSlidingExpiration(TimeSpan.FromSeconds(1));
					// Save data in cache.
					_cache.Set(CacheKeys.co2Result, co2Result, cacheEntryOptions);
					_cache.Set(CacheKeys.temperatureResult, temperatureResult, cacheEntryOptions);
				}
				
				httpContext.Response.ContentType = "text/html; charset=utf-8";
				await httpContext.Response.WriteAsync($"Relative Concentration of CO2: {co2Result.value} ({co2Result.heartbeat}), Ambient Temperature: {temperatureResult.value} ({temperatureResult.heartbeat})");
			} else {
				//Иначе обращаемся к следующему делегату в конвейере обработки запроса
				await _next.Invoke(httpContext);
			}
		}
	}
}