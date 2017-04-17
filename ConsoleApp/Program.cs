using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using Serilog.Formatting.Json;
using Serilog.Formatting.Compact;

namespace ConsoleApp
{
    class Program
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Config"))
                            .AddEnvironmentVariables()
                            .AddJsonFile("config.json", true)
                            .AddCommandLine(args);

            var config = builder.Build();
            var link = config["link"] ?? "https://g0t4.github.io/pluralsight-dotnet-core-xplat-apps/";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Version", Assembly.GetEntryAssembly().GetName().Version.ToString())
                .WriteTo.RollingFile(new CompactJsonFormatter(), Path.Combine(Directory.GetCurrentDirectory(), "Logs", "siri-{Date}.json"))
                .CreateLogger();

            var factory = new LoggerFactory()
                            .AddConsole(LogLevel.Trace, true)
                            .AddDebug(LogLevel.Trace)
                            .AddSerilog()
                            .AddFile("Logs/factory-{Date}.json");


            var logger = factory.CreateLogger("main");

            var fruit = new[] { "Apple", "Pear", "Orange" };
            Log.Information("In my bowl I have {@Fruit}", fruit);

            Console.WriteLine($"The link to analyze: {link}");
            Log.Information(($"Serilog: The link to analyze: {link}"), link);

            //obtaining temp file path
            var tempFile = Path.GetTempFileName();
            //Console.WriteLine("Temp file path: {0}", tempFile);
            logger.LogInformation($"Temp file path: {tempFile}");
            Log.Information($"Temp file path: {tempFile}", tempFile);


            var client = new HttpClient();
            var body = client.GetStringAsync(link);

            //Console.WriteLine(body.Result);
            var waitTimeInSeconds = 3;
            Task<int> waitTask = Task.Factory.StartNew(() =>
            {
                //Console.WriteLine($"Waiting for next task to start: {waitTimeInSeconds}");
                logger.LogTrace($"Waiting for next task to start: {waitTimeInSeconds}");
                Log.Information($"Waiting for next task to start: {waitTimeInSeconds}", waitTimeInSeconds);

                Thread.Sleep(waitTimeInSeconds * 1000);
                return 3000;
            });

            
           var linkCheckerTask = waitTask.ContinueWith(task =>
            {
                Console.WriteLine("Wait task has been finished: ", task.Status, task.Result);
                Log.Information("Wait task has been finished: {1}, {2}", task.Status, task.Result);

                var links = LinkChecker.GetLinks(body.Result);
                File.WriteAllLines(tempFile, links);
                foreach (var item in links)
                {
                    //Console.WriteLine(item);
                    logger.LogInformation(item);
                    Log.Verbose(item);
                }
            });

            logger.LogInformation("Waiting for Link Checker task to finish...");
            linkCheckerTask.Wait();

            //logger.LogInformation("Please press q to quit the application");
            //while (Console.ReadLine() != "q") ;

        }
    }
}