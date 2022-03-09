using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatBotDemo.Models
{
    public class ChatMessageModel
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public bool IsAnswer { get; set; }
        public string UserInputTextMessage { get; set; }
        public string ChatOutputTextMessage { get; set; }
        public string ChatOutputRichtTextMessage { get; set; }
        public ImageSource ChatOutputEmbeddedMediaMessage { get; set; }

        public string ChatEmbeddedMediaTextOutput { get; set; }
        public string ChatOutputEmbeddedVideoMessage { get; set; }

        public Image img { get; set; }


    }
}
