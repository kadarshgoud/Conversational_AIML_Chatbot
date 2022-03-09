using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMLBot.Misc
{
    public class Json_Libraries
    {

        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public string extract { get; set; }
    }


    public class Query
    {
        public Dictionary<string, Json_Libraries> Pages { get; set; }
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
