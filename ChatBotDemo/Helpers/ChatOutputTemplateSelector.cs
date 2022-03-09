using ChatBotDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChatBotDemo.Helpers
{
    public class ChatOutputTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserInputTemplate { get; set; }
        public DataTemplate ChatStringOutputTemplate { get; set; }
        public DataTemplate ChatRichtTextOutputTemplate { get; set; }
        public DataTemplate ChatEmbeddedMediaTemplate { get; set; }
        public DataTemplate ChatEmbeddedMediaTextOutput { get; set; }
        public DataTemplate ChatEmbeddedVideoTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var selectedTemplate = this.UserInputTemplate;

            var message = item as ChatMessageModel;
            if (message == null)
            {
                return selectedTemplate;
            }
            if (message.Type == "userinput")
            {
                selectedTemplate = this.UserInputTemplate;
            }
            if (message.Type == "chatoutput")
            {
                selectedTemplate = this.ChatStringOutputTemplate;
            }
            if (message.Type == "chatrichtextoutput")
            {
                selectedTemplate = this.ChatRichtTextOutputTemplate;
            }
            if (message.Type == "chatmediaoutput")
            {
                selectedTemplate = this.ChatEmbeddedMediaTemplate;
            }
            if (message.Type == "chatmediatextoutput")
            {
                selectedTemplate = this.ChatEmbeddedMediaTextOutput;
            }
            if (message.Type == "chatvideooutput")
            {
                selectedTemplate = this.ChatEmbeddedVideoTemplate;
            }

            return selectedTemplate;
        }
    }
}
