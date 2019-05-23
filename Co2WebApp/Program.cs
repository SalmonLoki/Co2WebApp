﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Co2WebApp {
    public class Program {
        public static void Main(string[] args) {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseUrls("https://*:5001;http://*:5000")
                   .UseStartup<Startup>();
    }
}