using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Mogera
{
    internal class Program
    {
        public static void DoDebug(List<Element> els, int nest)
        {
            foreach (var element in els)
            {
                if (element.Type == ElementType.Special)
                {
                    Console.WriteLine(
                        $"< Special ({element.ElementStartedAt}:{element.ElementEndedAt}) > ElType: {element.Type}, IsComment: {element.IsComment}, DocType: '{element.DocumentType}', Comment: '{element.Comment}'");
                    continue;
                }
                
                if (element.Type != ElementType.Content)
                {
                    Console.WriteLine(
                        $"{string.Concat(Enumerable.Repeat(" ", nest * 4))}< {element.TagName} ({element.ElementStartedAt}:{element.ElementEndedAt}) > Enclose: '{element.EnclosedText}'");
                }
                else
                {
                    Console.WriteLine($"{string.Concat(Enumerable.Repeat(" ", nest * 4))}< {element.TagName} ({element.ElementStartedAt}:{element.ElementEndedAt}) > Content: '{element.Content}'");
                }
                if (element.Attributes != null){
                    Console.WriteLine($"{string.Concat(Enumerable.Repeat(" ", nest * 4))}  |- Attributes");
                    foreach (var attr in element.Attributes.Keys)
                    {
                        Console.WriteLine(
                            $"{string.Concat(Enumerable.Repeat(" ", nest * 4))}     - '{attr}' : '{element.Attributes[attr]}'");
                    }
                }
                
                if (element.Children != null)
                {
                    Console.WriteLine($"{string.Concat(Enumerable.Repeat(" ", nest * 4))}  |- Children");
                    var l = element.Children;
                    DoDebug(l,nest+1);
                }
            }
        }
        public static void Main(string[] args)
        {
            string html;
            using (WebClient client = new WebClient ())
            {
                var url = "https://ja.wikipedia.org/wiki/ウィキペディア日本語版";
                Console.WriteLine($"Requesting... {url}");
                html = client.DownloadString(url);
            }
            // var web = new HtmlParser("");
            // var hassp = new HtmlParser("<!doctype html><!--H--><!--OOO--><h1 id='welcome-message' class='mes'>good</h1><div><a>link</a></div>");
            var web = new HtmlParser(System.IO.File.ReadAllText(@"/Users/x0y14/dev/csharp/Mogera/Mogera/examplecom.html"));
            // var web = new HtmlParser(html);
            var tags = web.Parse();
            DoDebug(tags, 0);
        }
    }
}