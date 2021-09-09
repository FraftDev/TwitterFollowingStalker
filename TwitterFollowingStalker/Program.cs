using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitterFollowingStalker.Models;

namespace TwitterFollowingStalker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = $"Twitter Following Stalker [v0.1.1] | Runthroughs: {Database.Runthroughs} | Total Checks {Database.Total}";
            TwitterAPI twitterAPI = new TwitterAPI("", "");

            twitterAPI.Initialize();

            Database.LoadFollowings();
            Database.LoadUsers();

            for(; ; )
            {
                foreach (DB_User_Model user in Database.TwitterUsers)
                {
                    twitterAPI.GetFollowings(user);
                    Database.BufferFollowings.Clear();

                    Console.WriteLine("Successfully ran through @" + user.TwitterUsername);
                    Database.Total++;
                    Console.Title = $"Twitter Following Stalker [v0.1.1] | Runthroughs: {Database.Runthroughs} | Total Checks {Database.Total}";

                    Thread.Sleep(TimeSpan.FromSeconds(70));
                }

                WebHook webHook = new WebHook();
                webHook.username = "Twitter Stalker 📅";
                webHook.avatar_url = "https://turbologo.com/articles/wp-content/uploads/2019/07/twitter-bird-logo.png.webp";
                webHook.embeds = new List<Embed>();
                webHook.embeds.Add(new Embed()
                {
                    title = "Twitter Stalker Ranking",
                    description = $"Following Ranking (500)",
                    timestamp = DateTime.Now,
                    color = 255,
                    author = new Author()
                    {
                        name = "FuseFire#9721",
                        url = "https://proxies.gg/",
                        icon_url = "https://i.imgur.com/XlpPqvu.png"
                    }
                });

                //Get last 500 and top count
                var dbLast500 = Database.TwitterFollowings.Skip(Math.Max(0, Database.TwitterFollowings.Count() - 500));
                var dbLast500Grouped = dbLast500.GroupBy(x => $"{x.TwitterFollowingId}:{x.TwitterFollowingUsername}");

                var result = dbLast500Grouped.Where(x => x.Count() > 1).First();

                dbLast500Grouped = dbLast500Grouped.OrderByDescending(x => x.Count());

                var top5Groups = dbLast500Grouped.Take(5);

                webHook.embeds[0].fields = new List<Field>();
                int i = 0;
                foreach (var fModel in top5Groups)
                {
                    string[] keySplit = fModel.Key.Split(':');
                    webHook.embeds[0].fields.Add(new Field() { name = "Rank #" + (i + 1), value = $"[@{keySplit[1]} ({fModel.Count()})](https://twitter.com/{keySplit[1]})", inline = true });
                    i++;
                }

                using(HttpClient client = new HttpClient())
                {
                    client.PostAsync("", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(webHook), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                }

                Database.Runthroughs++;
                Console.Title = $"Twitter Following Stalker [v0.1.1] | Runthroughs: {Database.Runthroughs} | Total Checks {Database.Total}";
            }

            Console.ReadKey();
        }
    }
}
