using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class LinkChecker
    {
        internal static IEnumerable<string> GetLinks(string body)
        {
            var document = new HtmlDocument();
            document.LoadHtml(body);
            var links = document.DocumentNode.SelectNodes("//a[@href]")
                    .Select(n => n.GetAttributeValue("href", string.Empty))
                    .Where(l => !String.IsNullOrEmpty(l))
                    .Where(l => l.StartsWith("http"));

            return links.ToList<string>();
        }
    }
}