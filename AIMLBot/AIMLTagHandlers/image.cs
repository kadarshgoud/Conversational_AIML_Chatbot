using System;
using System.Xml;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace AIMLBot.AIMLTagHandlers
{
    /// <summary>
    /// The websearch element search data from wiki    
    ///     
    /// for now its only search data from wiki 
    /// </summary>
    public class image : Utils.AIMLTagHandler
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public image(Bot bot,
                        AIMLBot.User user,
                        Utils.SubQuery query,
                        AIMLBot.Request request,
                        AIMLBot.Result result,
                        XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
        }

        protected override string ProcessChange()
        {
            if (this.templateNode.Name.ToLower() == "image")
            {
                var output = GetSearchResult(this.templateNode.InnerText);
                return output;
            }
            return string.Empty;
        }




        private string GetSearchResult(string inputimage)
        {
            var check = " _imagetagsearch " + inputimage;
            return check;

        }


    }
}

