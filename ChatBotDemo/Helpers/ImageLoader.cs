using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;

namespace ChatBotDemo.Helpers
{
    public class ImageLoader
    {
        public static BitmapImage LoadImage(byte[] imageData, bool decode, int size)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                try
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    if (decode)
                    {
                        image.DecodePixelWidth = size;
                    }
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: '{0}'", e);
                    //image = new BitmapImage();
                }
            }
            image.Freeze();
            return image;
        }
        public static byte[] LoadWikiImage(string language, string CurrentEntityName)
        {
            string WikiURL = "";

            if (language == "deDE")
            {
                // de.wikipedia donot have the image source urls in wikipedia website itself use enEN

                //WikiURL = "http://de.wikipedia.org/w/api.php?format=xml&action=query&prop=pageimages&titles=";

                WikiURL = "https://en.wikipedia.org/w/api.php?action=query&format=xml&formatversion=2&prop=pageimages|pageterms&piprop=thumbnail&pithumbsize=600&titles=";
            }
            if (language == "enEN")
            {
                WikiURL = "http://wikipedia.org/w/api.php?format=xml&action=query&prop=pageimages&titles=";
            }
            if (language == "frFR")
            {
                WikiURL = "http://fr.wikipedia.org/w/api.php?format=xml&action=query&prop=pageimages&titles=";
            }

            var subject = CurrentEntityName;
            // If subject was not converted to fit Wikipedia, it has to be done here (e.g. Donald Trump -> Donald_Trump)
            while (subject.EndsWith(" "))
                subject = subject.Substring(0, subject.Length - 1);
            while (subject.StartsWith(" "))
                subject = subject.Substring(1, subject.Length - 1);
            subject.Replace(' ', '_');

            XmlDocument doc = new XmlDocument();
            var webclient = new WebClient();
            webclient.Encoding = Encoding.UTF8;
            var pageSourceCode = webclient.DownloadString(WikiURL + subject + "&redirects=true");
            doc.LoadXml(pageSourceCode);
            var node = doc.GetElementsByTagName("page")[0];
            var thumbnail = doc.GetElementsByTagName("thumbnail")[0];
            string thumbnailvalue = null;
            string url = null;
            if (thumbnail != null)
            {
                thumbnailvalue = thumbnail.Attributes["source"].Value;
                var index = thumbnailvalue.LastIndexOf('/');
                var link = thumbnailvalue.Substring(0, index);
                var index2 = link.LastIndexOf('/');
                var imagelink = link.Substring(index2 + 1);
                if (imagelink.Contains(".svg"))
                {
                    url = link + "/" + "256px-" + imagelink + ".png";
                }
                else
                {
                    url = link + "/" + "256px-" + imagelink;
                }

                var imageBytes = webclient.DownloadData(new Uri(url));
                return imageBytes;
            }
            else
            {
                return null;
            }
        }


    }
}
