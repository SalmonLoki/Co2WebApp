using System;
using System.Linq;
using System.Threading.Tasks;
using Co2WebApp.Models;
using Co2WebApp.Services;
using HidSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

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
			if (httpContext.Request.Path.Value.ToLower() == "/co2") {
				if (!_cache.TryGetValue(CacheKeys.co2Result, out Result co2Result) || !_cache.TryGetValue(CacheKeys.temperatureResult, out Result temperatureResult)) {
					// Key not in cache, so get data.
							
					/*
					HidDevice hidDevice = _co2DeviceHandler.connectDevice(VendorId, ProductId);
					HidStream stream = _co2DeviceHandler.openStream(hidDevice);
					
					//the device won't send anything before receiving this packet 
					byte[] reportId = { 0x00 };
					byte[] key = { 0xc4, 0xc6, 0xc0, 0x92, 0x40, 0x23, 0xdc, 0x96 };
					byte[] request = reportId.Concat(key).ToArray();
						
					_co2DeviceHandler.sendSetFeatureSetupRequest(stream, request);				
					while (true) {
						byte[] receivedData = _co2DeviceHandler.readData(stream);
						if (receivedData.Length == 0) {
							Console.WriteLine(value: "Unable to read data");
						}
						if (receivedData.Length != 9) {
							Console.WriteLine(value: "transferred amount of bytes != expected bytes amount");
						}
	
						var temp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
						for (var i = 0; i < 8; i++) {
							temp[i] = receivedData[i + 1];
						}                
						receivedData = temp;   
					
						int[] data = _dataProcessor.decryptData(ref key, ref receivedData);
						if (!_dataProcessor.checkEndOfMessage(ref data)) {
							Console.WriteLine(value: "Unexpected data from device");
						}
	               
						if (!_dataProcessor.checkCheckSum(ref data)) {
							Console.WriteLine(value: "checksum error");
						}
	               
						Result result = _dataProcessor.dataProcessing(ref data);
						if (result.type.Equals("Relative Concentration of CO2"))
							co2Result = result;
						if (result.type.Equals("Ambient Temperature"))
							temperatureResult = result;
						if (co2Result != null & temperatureResult != null)
							break;
					}				
					_co2DeviceHandler.closeStream(stream); 
					*/
					co2Result = new Result("Relative Concentration of CO2", 1000, DateTime.Now.ToLongTimeString());
					temperatureResult = new Result("Ambient Temperature", 25, DateTime.Now.ToLongTimeString());
														
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