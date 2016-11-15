using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Threading.Tasks;

namespace Scraper
{
    class Scrape
    {
        /// <summary>
        /// Scrapes all links from a webpage using BFS
        /// Gets all links from original url only
        /// </summary>
        /// <param name="args"></param>
       static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a url.");
                return 1;
            }
            string domainOriginal = args[0];
            string domain;
            Queue<string> queue = new Queue<string>();
            List<string> seen = new List<string>();
            Scrape scrape = new Scrape();
            queue.Enqueue(domainOriginal);

            string htmlContent;
            string formattedDomain;
            List<string> urls = new List<string>();

            while (queue.Count > 0)
            {
                domain = queue.Dequeue();
                formattedDomain = scrape.FormatUrl(domain,domainOriginal);
                htmlContent = scrape.GetPageContent(formattedDomain);
               
                urls = scrape.ExtractUrls(htmlContent);
                foreach(string uri in urls )
                {
                    if (!scrape.IsValidUrl(uri, domainOriginal))
                    {
                        continue;
                    }
                    if (seen.Contains(uri))
                    {
                        continue;
                    }
                    if (queue.Contains(uri))
                    {
                        continue;
                    }
                    queue.Enqueue(uri);
                }
                if (!seen.Contains(domain))
                {
                    seen.Add(domain);
                }
            }
            seen.ForEach((e) => Console.WriteLine(e));
            return 0;
        }
        
        public string FormatUrl(string url, string domain)
        {
            if (!String.IsNullOrEmpty(url) && !url.StartsWith("http") && !url.StartsWith("https"))
            {
                return (url.ElementAt(0).Equals('/') ? domain + url : domain + "/" + url); 
            }
            return url;
        }
        public bool IsValidUrl(string url,string domain)
        {
            Regex reg = new Regex(@"(www.*)");
            Match m = reg.Match(domain);
            return (url.Contains(m.Value) || (!url.StartsWith("http")) && (!url.StartsWith("https")));
        }

        public List<string> ExtractUrls(string content)
        {
            List<string> urls = new List<string>();
            Regex regex = new Regex(@"href=""(.*?)""");
            MatchCollection m = regex.Matches(content);
            foreach (Match ma in m)
            {
                urls.Add(ma.Groups[1].Value);
            }
            return urls;
            
        }
        public string GetPageContent(string domain)
        {
            
            WebClient getData = new WebClient();
            try
            {
                Stream htmlStream = getData.OpenRead(domain);
                StreamReader htmlStreamReader = new StreamReader(htmlStream);
                string htmlContent = htmlStreamReader.ReadToEnd();
                return htmlContent;
            } catch(Exception e)
            {
                return "";
            }
        }
    }
}
