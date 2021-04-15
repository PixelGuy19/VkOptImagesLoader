using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Net;

namespace HtmlImagesLoader
{
    class Program
    {
        static List<string> LoadLinks = new List<string>();
        static Stopwatch Watch = new Stopwatch();

        static void Main(string[] args)
        {
            if (args.Length == 0) { return; }

            if (!Directory.Exists("Result")) { Directory.CreateDirectory("Result"); }

            Regex FindHref = new Regex("href=\"(.*?)\"");
            MatchCollection Matches = FindHref.Matches(File.ReadAllText(args[0]));
            foreach (string Link in Matches.Select(M => M.Groups[1].Value))
            {
                if (Link.Contains("userapi.com"))
                {
                    LoadLinks.Add(Link);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Files downloading");
            Watch.Start();
            Parallel.ForEach<string>(LoadLinks, Link => LoadLink(Link));
            Console.ReadLine();
        }

        static int DownloadedFilesCount = 0;
        static void LoadLink(string Link)
        {
            using (WebClient Client = new WebClient())
            {
                string OutFilePath = $"Result\\{DateTime.Now.Ticks}.jpg";
                Client.DownloadFileCompleted += Client_DownloadFileCompleted;
                try
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Client.DownloadFileAsync(new Uri(Link), OutFilePath);
                }
                catch (Exception e) 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                }

                void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                {
                    DownloadedFilesCount++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{OutFilePath} Downloaded (files count {DownloadedFilesCount}/{LoadLinks.Count})");

                    if (DownloadedFilesCount == LoadLinks.Count)
                    {
                        Watch.Stop();
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"Work done (total time = {Watch.ElapsedMilliseconds}ms)");
                    }
                }
            }
        }
    }
}
