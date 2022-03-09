using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// This class contains all the JSON objects which are represented as classes in C#. This are used to get the data content from the wikipedia 

namespace ChatBotDemo.Helpers
{
    public class pageval
    {
        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public string extract { get; set; }
    }


    public class Query
    {
        public Dictionary<string, pageval> pages { get; set; }
    }

    public class Limits
    {
        public int extracts { get; set; }
    }

    public class RootObject
    {
        public string batchcomplete { get; set; }
        public Query query { get; set; }
        public Limits limits { get; set; }
    }
}
