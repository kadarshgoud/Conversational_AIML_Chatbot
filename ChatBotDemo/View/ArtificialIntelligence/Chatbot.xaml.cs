using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;


namespace ChatBotDemo.View.ArtificialIntelligence
{
    /// <summary>
    /// Interaktionslogik für Chatbot.xaml
    /// </summary>
    public partial class Chatbot : UserControl
    {

        //Thickness LeftSide = new Thickness(-39, 0, 0, 0);
        //Thickness RightSide = new Thickness(0, 0, -39, 0);
        //SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        //SolidColorBrush On = new SolidColorBrush(Color.FromRgb(130, 190, 125));
        //private bool Toggled = false;

        public Chatbot()
        {
            InitializeComponent();

            // togged button initial condition

            // Back.Fill = Off;
            // Toggled = false;
            // Dot.Margin = LeftSide;

            // for automatic scrolling of list till end

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Tick += ((sender, e) =>
            {
                _contentCtrl.Height += 10;

                if (_scrollViewer.VerticalOffset == _scrollViewer.ScrollableHeight)
                {
                    _scrollViewer.ScrollToEnd();
                }
            });
            timer.Start();


            // for rich text box
            //  var htmlText = "Google's website is http://www.google.com";
            //  MyRichTextBox.Document = ConvertToFlowDocument(htmlText);
        }




        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //found at https://stackoverflow.com/questions/34817673/wpf-dynamic-hyperlinks-richtextbox

        private FlowDocument ConvertToFlowDocument(string text)
        {
            var flowDocument = new FlowDocument();

            var regex = new Regex(@"(http:\/\/[^\s]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = regex.Matches(text).Cast<Match>().Select(m => m.Value).ToList();

            var paragraph = new Paragraph();
            flowDocument.Blocks.Add(paragraph);

            foreach (var segment in regex.Split(text))
            {
                if (matches.Contains(segment))
                {
                    var hyperlink = new Hyperlink(new Run(segment))
                    {
                        NavigateUri = new Uri(segment),
                    };
                    hyperlink.RequestNavigate += (sender, args) => Process.Start(segment);

                    paragraph.Inlines.Add(hyperlink);
                }
                else
                {
                    paragraph.Inlines.Add(new Run(segment));
                }
            }

            return flowDocument;
        }






        //private void Selected_question_from_list(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    string input_Question = listofQuestions.Text;


        //}

        // togged button code 

        //  public bool Toggled1 { get => Toggled; set => Toggled = value; }

        //private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (!Toggled)
        //    {
        //        Back.Fill = On;
        //        Toggled = true;
        //        Dot.Margin = RightSide;


        //    }
        //    else
        //    {

        //        Back.Fill = Off;
        //        Toggled = false;
        //        Dot.Margin = LeftSide;

        //    }
        //}

    }
}

