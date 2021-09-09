using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterFollowingStalker
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Author
    {
        public string name { get; set; }
        public string url { get; set; }
        public string icon_url { get; set; }
    }

    public class Field
    {
        public string name { get; set; }
        public string value { get; set; }
        public bool inline { get; set; }
    }

    public class Embed
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime timestamp { get; set; }
        public int color { get; set; }
        public Author author { get; set; }
        public List<Field> fields { get; set; }
    }

    public class WebHook
    {
        public bool tts { get; set; }
        public string username { get; set; }
        public string avatar_url { get; set; }
        public List<Embed> embeds { get; set; }
    }
}
