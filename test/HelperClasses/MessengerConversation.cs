using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.HelperClasses
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    

    public class Message
    {
        public string sender_name { get; set; }
        public object timestamp_ms { get; set; }
        public string content { get; set; }
        public List<Reaction> reactions { get; set; }
        public Share share { get; set; }


        //public propertie to get datetime from timestamp_ms
        public DateTime Timestamp
        {
            get
            {
                // Convert the timestamp_ms to a DateTime object
                return DateTimeOffset.FromUnixTimeMilliseconds((long)timestamp_ms).DateTime;
            }
        }

    }

    public class Participant
    {
        public string name { get; set; }
    }

    

    public class Reaction
    {
        public string reaction { get; set; }
        public string actor { get; set; }
    }

    public class Root
    {
        public List<Participant> participants { get; set; }
        public List<Message> messages { get; set; }
        public string title { get; set; }

        public void SaveJson(string filePath)
        {
            //use Newtonsoft.Json to serialize the object to a json string
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            //write the json string to a file
            System.IO.File.WriteAllText(filePath, json);
        }

        public static Root LoadJson(string filePath)
        {
            //use Newtonsoft.Json to deserialize the json string to an object
            string json = System.IO.File.ReadAllText(filePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(json);
        }

        public void MergeMessages(Root other)
        {
            // Merge messages from the other object into this object
            if (other != null && other.messages != null)
            {
                this.messages.AddRange(other.messages);
            }
        }

    }

    public class Share
    {
        public string link { get; set; }
        public string share_text { get; set; }
    }


}
