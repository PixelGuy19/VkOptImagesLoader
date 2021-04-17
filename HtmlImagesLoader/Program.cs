using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlImagesLoader
{
    class Program
    {
        static List<string> LoadLinks = new List<string>();
        static Stopwatch Watch = new Stopwatch();

        async static Task Main(string[] args)
        {
            //if (args.Length == 0) { return; }
            args = new string[1];
            
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
            ParallelLoopResult Result = Parallel.ForEach<string>(LoadLinks, Link => LoadLink(Link));
            Console.ReadLine();
        }

        //static List<WebClient> UnfinishedLoadings = new List<WebClient>();
        static int DownloadedFilesCount = 0;
        static void LoadLink(string Link)
        {
            using (WebClient Client = new WebClient())
            {
                {
                    string OutFilePath = $"Result\\{DateTime.Now.Ticks}.jpg";
                    Client.DownloadFileCompleted += Client_DownloadFileCompleted;

                    try
                    {
                        Client.DownloadFileTaskAsync(new Uri(Link), OutFilePath).Wait();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                    {
                        AddCounter();
                    }
                    void AddCounter()
                    {
                        Client.Dispose();
                        DownloadedFilesCount++;
                        Console.WriteLine($">>>{OutFilePath} Downloaded ({DownloadedFilesCount}/{LoadLinks.Count}");

                        if (DownloadedFilesCount == LoadLinks.Count)
                        {
                            Watch.Stop();
                            Console.WriteLine($"Work done (total time = {Watch.ElapsedMilliseconds}ms)");
                        }
                    }

                }
            }
        }
    }
}