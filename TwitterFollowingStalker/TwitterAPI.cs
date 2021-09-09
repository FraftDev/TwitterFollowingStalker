using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitterFollowingStalker.Models;

namespace TwitterFollowingStalker
{
    class TwitterAPI
    {
        public string API_KEY { get; set; }
        public string API_SECRET { get; set; }
        public string BEARER_ACCESS { get; set; }

        public TwitterAPI(string api_key, string api_secret)
        {
            API_KEY = api_key;
            API_SECRET = api_secret;

            if (!File.Exists(Environment.CurrentDirectory + "\\Bearer.txt"))
                return;

            string line = File.ReadAllText(Environment.CurrentDirectory + "\\Bearer.txt");
            
            if (string.IsNullOrEmpty(line))
                return;

            BEARER_ACCESS = line;
        }

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(BEARER_ACCESS))
                return;

            Connect();
        }

        public APIReponseStatus Connect()
        {
            try
            {
                using(HttpClient httpClient = new HttpClient())
                {
                    string basic_access = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{API_KEY}:{API_SECRET}"));
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {basic_access}");
                    HttpResponseMessage httpResponse = httpClient.PostAsync("https://api.twitter.com/oauth2/token", new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded")).Result;
                    string httpResponseString = httpResponse.Content.ReadAsStringAsync().Result;

                    dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(httpResponseString);

                    BEARER_ACCESS = jsonResponse.access_token;
                    File.WriteAllText(Environment.CurrentDirectory + "\\Bearer.txt", BEARER_ACCESS);
                    return APIReponseStatus.Success;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return APIReponseStatus.Failure;
            }
        }

        public APIReponseStatus GetFollowings(DB_User_Model user)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {BEARER_ACCESS}");
                    HttpResponseMessage httpResponse = httpClient.GetAsync("https://api.twitter.com/1.1/friends/list.json?count=200&user_id=" + user.TwitterId).Result;
                    string httpResponseString = httpResponse.Content.ReadAsStringAsync().Result;

                    Root jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(httpResponseString);

                    foreach(User jsonUser in jsonResponse.users)
                    {
                        DB_Following_Model followingModel = new DB_Following_Model()
                        {
                            TwitterId = user.TwitterId,
                            TwitterFollowingId = (long)jsonUser.id,
                            TwitterFollowingUsername = (string)jsonUser.screen_name
                        };

                        Database.AddFollowing(followingModel);
                    }

                    if(Database.BufferFollowings.Count == 0)
                        return APIReponseStatus.Success;

                    WebHook webHook = new WebHook();
                    webHook.username = "Twitter Stalker 📅";
                    webHook.avatar_url = "https://turbologo.com/articles/wp-content/uploads/2019/07/twitter-bird-logo.png.webp";
                    webHook.embeds = new List<Embed>();
                    webHook.embeds.Add(new Embed()
                    {
                        title = "Twitter Stalker Alert",
                        description = $"@{user.TwitterUsername} started following new Accounts!",
                        timestamp = DateTime.Now,
                        color = 255,
                        author = new Author()
                        {
                            name = "FuseFire#9721",
                            url = "https://proxies.gg/",
                            icon_url = "https://i.imgur.com/XlpPqvu.png"
                        }
                    });

                    webHook.embeds[0].fields = new List<Field>();
                    foreach (DB_Following_Model followingModel in Database.BufferFollowings)
                    {
                        webHook.embeds[0].fields.Add(new Field() { name = "Followed", value = $"[@{followingModel.TwitterFollowingUsername}](https://twitter.com/{followingModel.TwitterFollowingUsername})", inline = true });
                    }

                    using (HttpClient client = new HttpClient())
                    {
                        client.PostAsync("", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(webHook), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                    }

                    return APIReponseStatus.Success;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message} | {ex.StackTrace}");
                return APIReponseStatus.Failure;
            }
        }

        public enum APIReponseStatus
        {
            Success,
            Failure
        };
    }
}
