using System;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace AIMLBot.AIMLTagHandlers
{
    /// <summary>
    /// The lowercase element tells the AIML interpreter to render the contents of the element 
    /// in lowercase, as defined (if defined) by the locale indicated by the specified language
    /// (if specified). 
    /// 
    /// If no character in this string has a different lowercase version, based on the Unicode 
    /// standard, then the original string is returned. 
    /// </summary>
    public class first : Utils.AIMLTagHandler
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
        public first(Bot bot,
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
            if (this.templateNode.Name.ToLower() == "first")
            {
                return DisplayFirstWordSentence(this.templateNode.InnerText);
            }
            return string.Empty;
        }

        private string DisplayFirstWordSentence(string word)
        {
            // Removing the spaces first

            var inputToFirst = word.Trim();

            // logic to print only the first element of the sentence

            string[] words = inputToFirst.Split();
            var firstword = words.First();

            return firstword;
        }
    }
}
