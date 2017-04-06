﻿using System;
using System.Net.Http;
using System.IO;

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
            var tempFile = Path.GetTempFileName();
            Console.WriteLine("Temp file path: {0}", tempFile);
            var client = new HttpClient();
            var body = client.GetStringAsync("https://g0t4.github.io/pluralsight-dotnet-core-xplat-apps/");
            
            //Console.WriteLine(body.Result);

            var links = LinkChecker.GetLinks(body.Result);
            File.WriteAllLines(tempFile, links);
            foreach (var item in links)
            {

                Console.WriteLine(item);
            }


        }
    }
}