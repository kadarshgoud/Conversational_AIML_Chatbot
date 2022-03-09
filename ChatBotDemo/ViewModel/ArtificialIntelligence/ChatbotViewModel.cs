using AIMLBot;
using ChatBotDemo.Models;
using ChatBotDemo.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Windows.Input;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using ChatBotDemo.Helpers;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections;
using System.Windows.Documents;

namespace ChatBotDemo.ViewModel.ArtificialIntelligence
{
    public class ChatbotViewModel : BaseViewModel
    {
        public DelegateCommand SubmitChatMessageAsyncCommand { get; set; }
        public DelegateCommand InitAIMLAsyncCommand { get; set; }

        //public DelegateCommand Check_ToggleButton { get; set; }


        // Text to speech implementation

        SpeechSynthesizer TexttoSpeech = new SpeechSynthesizer();

        // List of default questions that user can ask to bot

        // public List<string> ListOfQuestionsToBot { get; set; }

        private Bot myBot;
        private User myUser;
        private Request lastRequest = null;
        private Result lastResult = null;
        // private bool Toggled = false;


        public ChatbotViewModel()
        {
            SubmitChatMessageAsyncCommand = new DelegateCommand(() =>
            {
                var canExecute = AsyncCommand.Create(token => SubmitChatMessageAsync(token));
                canExecute.Execute(null);
            });

            InitAIMLAsyncCommand = new DelegateCommand(() =>
            {
                var canExecute = AsyncCommand.Create(token => InitAIMLAsync(token));
                canExecute.Execute(null);
            });

            ListOfQuestionsToBot = new List<string>()  // default qns
            {
                "What is your name?",
                "Do you eat?",
                "What is coronavirus?",
                "What is COVID-19?",
                "What are symptoms of corona virus?",
                "what are handwashing techniques?",
                "Can soap help with coronavirus?",
                "What is social distancing?",
                "How can i get tested?",
                "Who should take the test?"

            };

            //Check_ToggleButton = new DelegateCommand(() =>
            //{
            //    var canExecute = AsyncCommand.Create(token => Check_Toggle(token));
            //    canExecute.Execute(null);
            //});



            myBot = new Bot();
            myUser = new User("Johannes", myBot);

            ChatMessageCollection = new ObservableCollection<ChatMessageModel>();
        }

        public async void PrepareViewAsync()
        {
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            await Task.Factory.StartNew(async () =>
            {
                AIMLBot.Utils.AIMLLoader loader = new AIMLBot.Utils.AIMLLoader(this.myBot);
                this.myBot.isAcceptingUserInput = false;
                loader.loadAIML(this.myBot.PathToAIML);
                this.myBot.isAcceptingUserInput = true;

            }, CancellationToken.None, TaskCreationOptions.None, uiContext);
        }

        private async Task SubmitChatMessageAsync(CancellationToken token)
        {
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            await Task.Factory.StartNew(async () =>
            {
                processInputFromUser();
            }, CancellationToken.None, TaskCreationOptions.None, uiContext);
        }

        //private async Task Check_Toggle(CancellationToken token)
        //{
        //    var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
        //    await Task.Factory.StartNew(async () =>
        //    {
        //        ToggleButton();
        //    }, CancellationToken.None, TaskCreationOptions.None, uiContext);
        //}

        //private void ToggleButton()
        //{
        //    if (!Toggled)
        //    {

        //        Toggled = true;




        //    }
        //    else
        //    {


        //        Toggled = false;


        //    }

        //}

        private async Task InitAIMLAsync(CancellationToken token)
        {
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            await Task.Factory.StartNew(async () =>
            {
                myBot.loadSettings(myBot.PathToConfigFiles);
                AIMLBot.Utils.AIMLLoader loader = new AIMLBot.Utils.AIMLLoader(this.myBot);
                this.myBot.isAcceptingUserInput = false;
                loader.loadAIML(this.myBot.PathToAIML);
                this.myBot.isAcceptingUserInput = true;

            }, CancellationToken.None, TaskCreationOptions.None, uiContext);
        }




        private void processInputFromUser()
        {
            if (this.myBot.isAcceptingUserInput)
            {
                //DropdownListSelection?
                if ((SelectedQuestionfromQuestionsToBot != "" && SelectedQuestionfromQuestionsToBot != null) || Input == null)
                {
                    if (SelectedQuestionfromQuestionsToBot == null && Input == null)
                    {
                        Input = "";
                    }
                    else
                    {

                        if (Input != null && Input != "")
                        {
                            SelectedQuestionfromQuestionsToBot = null;
                            Input = Input;

                        }
                        else
                        {

                            Input = SelectedQuestionfromQuestionsToBot;
                        }
                    }
                }

                string rawInput = Input;
                Input = string.Empty;
                var userinput = "You: " + rawInput;
                Request myRequest = new Request(rawInput, this.myUser, this.myBot);
                Result myResult = this.myBot.Chat(myRequest);
                this.lastRequest = myRequest;
                this.lastResult = myResult;
                string searchword = null;
                bool checkInputisNullorEmpty = String.IsNullOrEmpty(rawInput);
                bool checkInputhasWhitespace = String.IsNullOrWhiteSpace(rawInput);
                string SaveDataToAiml = null;


                // RichMediaTextBox Trail



                // If the bot doesnot have knowledge and could not reply it throws the default answer

                //if ((myResult.Output == ""))
                //{
                //    string nomatchfound = "I have no answer for that. Can you please teach me ?" + Environment.NewLine;

                //    string noMatchOutput = "Bot: " + nomatchfound; // + userinput;

                //    string userinput_textbox = userinput + Environment.NewLine;

                //    ChatOutput = noMatchOutput + ChatOutput;

                //    ChatInputtextbox = userinput_textbox + ChatInputtextbox;
                //}

                //else
                //     {

                // ToggleButton();

                string combinedDialogString = "Bot: " + myResult.Output + Environment.NewLine; // + userinput;

                if ((checkInputisNullorEmpty == false) & (checkInputhasWhitespace == false) & (rawInput != "null")) // "  " is pending
                {
                    if (myResult.Output == "" || myResult.Output == String.Empty)
                    {

                        //Regex r1 = new Regex(@"what|WHAT|What");
                        //bool whatmatch = r1.IsMatch(rawInput);

                        var string_what = new[] { "what" }; var string_who = new[] { "who" };
                        bool CheckforWhat = rawInput.ContainsAny(string_what, StringComparison.CurrentCultureIgnoreCase);
                        bool CheckforWho = rawInput.ContainsAny(string_who, StringComparison.CurrentCultureIgnoreCase);

                        if (CheckforWhat == true)
                        {
                            var removespecialcharacters = Regex.Replace(rawInput, @"[ ](?=[ ])|[^A-Za-z0-9 ]+", "");
                            var removespaces_at_end = removespecialcharacters.Trim();
                            string[] spiltwords = removespaces_at_end.Split(' ');
                            string lastword = spiltwords[spiltwords.Length - 1];

                            // Taking only the last word from the sentence and making it as a Formal text which is the input to wiki search

                            var withspace_at_end = FormalTextConverter(lastword);
                            var removespace_end = TrimtheText(withspace_at_end);
                            searchword = removespace_end;

                        }
                        else if (CheckforWho == true)
                        {
                            var removespecialcharacters = Regex.Replace(rawInput, @"[ ](?=[ ])|[^A-Za-z0-9 ]+", "");
                            var removespaces_at_end = removespecialcharacters.Trim();

                            string lastword = CodeforGettingWords(removespaces_at_end);
                            // Taking only the last word from the sentence and making it as a Formal text which is the input to wiki search

                            var withspace_at_end = FormalTextConverter(lastword);
                            var removespace_end = TrimtheText(withspace_at_end);
                            searchword = removespace_end;
                        }

                        else
                        {
                            var CountNoOfWordsinInput = CheckNoOfWordsinInput(rawInput);

                            bool containsSingleWord = false;
                            bool containsTwoWords = false;
                            bool contaisThreeWords = false;

                            if (CountNoOfWordsinInput == 1)
                            {
                                containsSingleWord = true;
                            }
                            else if (CountNoOfWordsinInput == 2)
                            {
                                containsTwoWords = true;
                            }
                            else if (CountNoOfWordsinInput == 3)
                            {
                                contaisThreeWords = true;
                            }

                            if (containsSingleWord == true || containsTwoWords == true || contaisThreeWords == true)  // This else checks whether the input is a single word or not 
                            {
                                //if yes then search the word in wikipedia and store the output

                                var withspace_at_end = FormalTextConverter(rawInput);
                                var removespace_end = TrimtheText(withspace_at_end);
                                while (removespace_end.EndsWith(" "))
                                    removespace_end = removespace_end.Substring(0, removespace_end.Length - 1);
                                while (removespace_end.StartsWith(" "))
                                    removespace_end = removespace_end.Substring(1, removespace_end.Length - 1);
                                var finalword = removespace_end.Replace(' ', '_');
                                searchword = finalword;
                            }


                        }

                        if ((searchword != "") & (searchword != null))
                        {
                            // 2 {100% Working}

                            var result = WikiSearchTextOutput.SendingRecieveingRequestsfromWiki(searchword);


                            bool value = String.IsNullOrEmpty(result);

                            if (value == false)
                            {
                                if (CheckforWhat == true)
                                {
                                    // Saving the data into a file 

                                    SaveDataToAiml = "<category>" + Environment.NewLine + "<pattern>" + searchword.ToUpper() + "</pattern>" + Environment.NewLine + "<template>" + result + "</template>" + Environment.NewLine + "</category>" + Environment.NewLine + "<category>" + Environment.NewLine + "<pattern>" + "WHAT IS A " + searchword.ToUpper() + "</pattern>" + Environment.NewLine + "<template>" + result + "</template>" + Environment.NewLine + "</category>" + Environment.NewLine + "<category>" + Environment.NewLine + "<pattern>" + "WHAT IS " + searchword.ToUpper() + "</pattern>" + Environment.NewLine + "<template>" + "<srai>" + searchword.ToUpper() + "</srai>" + "</template>" + Environment.NewLine + "</category>" + Environment.NewLine;
                                }

                                else if (CheckforWho == true)
                                {
                                    SaveDataToAiml = "<category>" + Environment.NewLine + "<pattern>" + searchword.ToUpper() + "</pattern>" + Environment.NewLine + "<template>" + result + "</template>" + Environment.NewLine + "</category>" + Environment.NewLine + "<category>" + Environment.NewLine + "<pattern>" + "WHO WAS " + searchword.ToUpper() + "</pattern>" + Environment.NewLine + "<template>" + result + "</template>" + Environment.NewLine + "</category>" + Environment.NewLine + "<category>" + Environment.NewLine + "<pattern>" + "WHO IS " + searchword.ToUpper() + "</pattern>" + Environment.NewLine + "<template>" + "<srai>" + searchword.ToUpper() + "</srai>" + "</template>" + Environment.NewLine + "</category>" + Environment.NewLine;
                                }

                                string filepath = @"G:\mkrAIMLbot\CustomAIMLStore\Wiki_Search_file.aiml";

                                // This text is added only once to the file.
                                if (!File.Exists(filepath))
                                {
                                    // Create a file to write to.
                                    string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + SaveDataToAiml + Environment.NewLine + "</aiml>";
                                    File.WriteAllText(filepath, createText, Encoding.UTF8);
                                }
                                else
                                {
                                    DeleteLastLine(filepath);
                                    // This text is appended.
                                    string appendText = SaveDataToAiml + Environment.NewLine + "</aiml>";
                                    File.AppendAllText(filepath, appendText, Encoding.UTF8);
                                }


                                string NewUrlString = "Bot: " + result + Environment.NewLine;


                                string userinput_textbox = userinput + Environment.NewLine;
                                ChatOutput = NewUrlString + ChatOutput;
                                ChatInputtextbox = userinput_textbox + ChatInputtextbox;


                                bool CheckIsImageSaved = false;
                                bool jk = false;

                                CheckIsImageSaved = CheckImageIsSavedinPC(searchword.ToUpper());

                                if (CheckIsImageSaved == true)
                                {
                                    var ImageIsAvailable = LoadtheImageSavedinPC(searchword.ToUpper());

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = result, Type = "chatoutput", IsAnswer = true });

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = ImageIsAvailable, Type = "chatmediaoutput" });
                                }
                                else
                                {
                                    // This is executed when the output sentence starts with A,The,AN followed by the word to be searched
                                    // Example: A Tree, The Tiger etc.

                                    var ImagefromWiki = ImageLoader.LoadImage(ImageLoader.LoadWikiImage("enEN", searchword), true, 256);

                                    if (ImagefromWiki != null)
                                    {
                                        // save image

                                        var filename1 = searchword.ToUpper() + ".jpg";

                                        var localFilePath1 = Path.Combine(@"G:\mkrAIMLbot\ImagesSavedWiki", filename1);

                                        SaveImage(ImagefromWiki, localFilePath1);

                                        ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                                        ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = result, Type = "chatoutput", IsAnswer = true });

                                        ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = ImagefromWiki, Type = "chatmediaoutput" });
                                    }
                                    else
                                    {

                                        ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                                        ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = result, Type = "chatoutput", IsAnswer = true });
                                    }
                                }



                                // Loading the data into the bot brain after printing the output on to the screen

                                this.myBot.loadAIMLFromFiles();
                            }
                            else  // This else is executed when output from wikipedia is null 
                            {
                                string nomatchfound = "I have no answer for that. Can you please teach me ?" + Environment.NewLine;

                                string noMatchOutput = "Bot: " + nomatchfound; // + userinput;

                                string userinput_textbox = userinput + Environment.NewLine;

                                ChatOutput = noMatchOutput + ChatOutput;

                                ChatInputtextbox = userinput_textbox + ChatInputtextbox;

                                ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });
                                ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = noMatchOutput, Type = "chatoutput", IsAnswer = true });
                            }

                        }
                        else  // This else is executed when there is no ans for the user input/ searchword is null or empty
                        {


                            string nomatchfound = "I have no answer for that. Can you please teach me ?" + Environment.NewLine;

                            var linetwo = " You can teach me using the following Commands ";
                            var linethree = " learnf -----> For person related information (dob, place)";
                            var linefour = "learng -----> Information regarding geography";
                            var linefive = "learnp -----> Information regarding professions";


                            string noMatchOutput = "Bot: " + nomatchfound + Environment.NewLine + linetwo + Environment.NewLine + linethree + Environment.NewLine + linefour + Environment.NewLine + linefive; // + userinput;

                            string userinput_textbox = userinput + Environment.NewLine;

                            ChatOutput = noMatchOutput + ChatOutput;



                            ChatInputtextbox = userinput_textbox + ChatInputtextbox;

                            ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });
                            ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = noMatchOutput, Type = "chatoutput", IsAnswer = true });



                        }
                    }

                    else    //This else is executed when the information is found in AIML files
                    {
                        // Speech to text Implementation ** 

                        //string OutputToSpeak = myResult.Output;

                        //if (OutputToSpeak != "")
                        //{
                        //    TexttoSpeech.Speak(OutputToSpeak);

                        //}



                        string userinput_textbox = userinput + Environment.NewLine;

                        ChatOutput = combinedDialogString + ChatOutput;

                        ChatInputtextbox = userinput_textbox + ChatInputtextbox;

                        var resultString = Regex.Replace(myResult.Output, "^[^_]*_", "", RegexOptions.IgnorePatternWhitespace);


                        var wikiwordcheck = CodeforGettingFirstWordofSentence(resultString);



                        var restofthewordsafterwikicheck = resultString.Replace(wikiwordcheck, "").TrimStart();

                        if (wikiwordcheck == "wikiimagesearch")
                        {

                            //check whether the image is already stored in a folder in PC

                            bool CheckIsImageSaved = false;
                            bool jk = false;

                            CheckIsImageSaved = CheckImageIsSavedinPC(restofthewordsafterwikicheck.ToUpper());

                            if (CheckIsImageSaved == true)
                            {
                                var ImageIsAvailable = LoadtheImageSavedinPC(restofthewordsafterwikicheck.ToUpper());

                                string text1 = "Searching an image of " + restofthewordsafterwikicheck + " for you !!";

                                string output1 = "Bot: " + text1 + Environment.NewLine; // + userinput;


                                ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                                ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = output1, Type = "chatoutput", IsAnswer = true });

                                ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = ImageIsAvailable, Type = "chatmediaoutput" });
                            }
                            else
                            {
                                // This is executed when the output sentence starts with A,The,AN followed by the word to be searched
                                // Example: A Tree, The Tiger etc.

                                var ImagefromWiki = ImageLoader.LoadImage(ImageLoader.LoadWikiImage("enEN", restofthewordsafterwikicheck), true, 256);

                                if (ImagefromWiki != null)
                                {
                                    // save image

                                    var filename1 = restofthewordsafterwikicheck.ToUpper() + ".jpg";

                                    var localFilePath1 = Path.Combine(@"G:\mkrAIMLbot\ImagesSavedWiki", filename1);

                                    SaveImage(ImagefromWiki, localFilePath1);


                                    string textforImage = "Searching an image of " + restofthewordsafterwikicheck + " for you !!";

                                    string OutputtextforImage = "Bot: " + textforImage + Environment.NewLine; // + userinput;


                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = OutputtextforImage, Type = "chatoutput", IsAnswer = true });

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = ImagefromWiki, Type = "chatmediaoutput" });
                                }
                                else
                                {
                                    string nomatchfound = "Could not found an image. Please give the valid image name " + Environment.NewLine;


                                    string noMatchOutput = "Bot: " + nomatchfound + Environment.NewLine; // + userinput;

                                    string userinput_textbox1 = userinput + Environment.NewLine;

                                    ChatOutput = noMatchOutput + ChatOutput;



                                    ChatInputtextbox = userinput_textbox1 + ChatInputtextbox;

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });
                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = noMatchOutput, Type = "chatoutput", IsAnswer = true });
                                }
                            }
                        }
                        else if (wikiwordcheck == "imagetagsearch")
                        {
                            bool CheckIsImageSaved = false;
                            bool jk = false;
                            string removingtag = null;

                            CheckIsImageSaved = CheckImageIsSavedinPC(restofthewordsafterwikicheck.ToUpper());

                            if (CheckIsImageSaved == true)
                            {
                                var ImageIsAvailable = LoadtheImageSavedinPC(restofthewordsafterwikicheck);

                                removingtag = myResult.Output.Substring(0, myResult.Output.LastIndexOf('_')).TrimEnd();

                                ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                                ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = removingtag, Type = "chatoutput", IsAnswer = true });

                                ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = ImageIsAvailable, Type = "chatmediaoutput" });

                            }
                            else
                            {
                                // This is executed when the output sentence starts with A,The,AN followed by the word to be searched
                                // Example: A Tree, The Tiger etc.

                                var ImagefromWiki = ImageLoader.LoadImage(ImageLoader.LoadWikiImage("enEN", restofthewordsafterwikicheck), true, 256);

                                if (ImagefromWiki != null)
                                {

                                    // save image

                                    var filename1 = restofthewordsafterwikicheck.ToUpper() + ".jpg";

                                    var localFilePath1 = Path.Combine(@"G:\mkrAIMLbot\ImagesSavedWiki", filename1);

                                    SaveImage(ImagefromWiki, localFilePath1);

                                    // used to remove the end tag 
                                    removingtag = myResult.Output.Substring(0, myResult.Output.LastIndexOf('_')).TrimEnd();

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = removingtag, Type = "chatoutput", IsAnswer = true });

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = ImagefromWiki, Type = "chatmediaoutput" });
                                }
                                else
                                {
                                    string nomatchfound = "Could not found an image. Please give the valid image name " + Environment.NewLine;


                                    string noMatchOutput = "Bot: " + nomatchfound + Environment.NewLine; // + userinput;

                                    string userinput_textbox1 = userinput + Environment.NewLine;

                                    ChatOutput = noMatchOutput + ChatOutput;



                                    ChatInputtextbox = userinput_textbox1 + ChatInputtextbox;

                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });
                                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = noMatchOutput, Type = "chatoutput", IsAnswer = true });
                                }
                            }
                        }
                        else
                        {

                            //bool imageoutput = false;
                            //bool TryforOtherTextPattern = false;

                            //if (ImagefromWiki != null)
                            //{
                            //    imageoutput = true;
                            //}
                            //else
                            //{
                            //    TryforOtherTextPattern = true;
                            //    imageoutput = false;
                            //}

                            //// save image

                            //if (imageoutput == true)
                            //{

                            //    var filename1 = firstword_formal + ".jpg";

                            //    var localFilePath1 = Path.Combine(@"G:\mkrAIMLbot\ImagesSavedWiki", filename1);

                            //    SaveImage(ImagefromWiki, localFilePath1);

                            //    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                            //    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = ImagefromWiki, Type = "chatmediaoutput" });

                            //    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = myResult.Output, Type = "chatoutput", IsAnswer = true });
                            //}

                            //// This is executed when the output sentence starts with A,The,AN or with the keyword for the image input 
                            //// example: The Albert Einstein, Donald Trumph, A Gandhi

                            //if (TryforOtherTextPattern == true)
                            //{
                            //    var tempout = CodeforGettingFirstTwoWordsSentence(myResult.Output);
                            //    var formout = FormalTextConverter(tempout);
                            //    var getsecondthirdword = formout.Trim();
                            //    bool ImageFound = false;

                            //    var Imageresult = CheckImageIsSavedinPC(getsecondthirdword);

                            //    if ((Imageresult != null))
                            //    {
                            //        ImageFound = true;

                            //        ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                            //        ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = Imageresult, Type = "chatmediaoutput" });

                            //        ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = myResult.Output, Type = "chatoutput", IsAnswer = true });


                            //    }

                            //    if (ImageFound == false)
                            //    {
                            //        bool ImageFoundFromWiki = false;

                            //        ImagefromWiki = ImageLoader.LoadImage(ImageLoader.LoadWikiImage("enEN", getsecondthirdword), true, 256);

                            //        if (ImagefromWiki != null)
                            //        {
                            //            ImageFoundFromWiki = true;

                            //            // save image

                            //            var filename = getsecondthirdword + ".jpg";

                            //            var localFilePath = Path.Combine(@"G:\mkrAIMLbot\ImagesSavedWiki", filename);

                            //            SaveImage(ImagefromWiki, localFilePath);

                            //            ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                            //            ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = ImagefromWiki, Type = "chatmediaoutput" });

                            //            ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = myResult.Output, Type = "chatoutput", IsAnswer = true });

                            //        }
                            //        else
                            //        {
                            //            ImageFoundFromWiki = false;
                            //            TryforOtherTextPattern = false;
                            //        }

                            //    }
                            //}

                            //if ((imageoutput == false) & (TryforOtherTextPattern == false))
                            //{


                            if (combinedDialogString.Contains("Ok I will store the information that"))
                            {
                                this.myBot.loadAIMLFromFiles();
                            }
                            else if (combinedDialogString.Contains("Ok. I will store the information that"))
                            {
                                this.myBot.loadAIMLFromFiles();
                            }
                            else if (combinedDialogString.Contains("Ok. I will store the information"))
                            {
                                this.myBot.loadAIMLFromFiles();
                            }
                            //}



                            ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });

                            // ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = ImagefromWiki, Type = "chatmediaoutput" });

                            ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = myResult.Output, Type = "chatoutput", IsAnswer = true });
                        }
                    }
                }
                else
                { // This else is executed if the input is empty "" 

                    string nomatchfound = "Please check the input and enter a valid question or a word. Thank You :-)" + Environment.NewLine;

                    string noMatchOutput = "Bot: " + nomatchfound; // + userinput;

                    string userinput_textbox = userinput + Environment.NewLine;

                    //This code is used  when we have an image source saved in PC

                    Image img = new Image();

                    BitmapImage bitImage = new BitmapImage();
                    bitImage.BeginInit();
                    bitImage.UriSource = new Uri(@"G:\mkrAIMLbot\CustomImages\merkel.jpg");
                    bitImage.EndInit();

                    img.Source = bitImage;



                    var Loadimg = LoadtheImageSavedinPC("Albert Einstein");

                    //BitmapImage ssss = new BitmapImage();
                    //ssss.BeginInit();
                    //ssss.UriSource = new Uri(@"G:\mkrAIMLbot\ImagesSavedWiki\Dog.jpg");
                    //ssss.EndInit();



                    //// This code searches the input fom the wikipedia and gives the image 

                    //var ImagefromWiki = ImageLoader.LoadImage(ImageLoader.LoadWikiImage("enEN", "Dog"), true, 256);

                    //if (ImagefromWiki == null)
                    //{
                    //    var text = "There is no Image found.";

                    //    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatEmbeddedMediaTextOutput = text, Type = "chatmediatextoutput", IsAnswer = true });
                    //}

                    //string folderPath = @"G:\mkrAIMLbot\ImagesSavedWiki";
                    //string fileName = searchword + ".jpg";
                    //string imagePath = folderPath + fileName;

                    //// save image

                    //var filename = "Dog.jpg";

                    //var localFilePath = Path.Combine(@"G:\mkrAIMLbot\ImagesSavedWiki", filename);

                    //SaveImage(ImagefromWiki, localFilePath);

                    // RotateAndSaveImage(folderPath, SecondImage, 0);

                    //System.Drawing.Image img = System.Drawing.Image.
                    //img.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);


                    ChatOutput = noMatchOutput + ChatOutput;

                    ChatInputtextbox = userinput_textbox + ChatInputtextbox;

                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, UserInputTextMessage = userinput, Type = "userinput", IsAnswer = false });
                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputTextMessage = noMatchOutput, Type = "chatoutput", IsAnswer = true });

                    // Trail 1  imagelink http://www.clipartkid.com/images/817/pic-of-german-flag-clipart-best-VkuN37-clipart.jpeg

                    ChatMessageCollection.Add(new ChatMessageModel { Time = DateTime.Now, ChatOutputEmbeddedMediaMessage = Loadimg, Type = "chatmediaoutput" });

                }
            }

            else
            {
                Input = string.Empty;
                Input = "Bot not accepting user input." + Environment.NewLine;
            }

        }


        public static void DeleteLastLine(string path)  // created for file saving after searching the data from wikipedia
        {
            List<string> lines = File.ReadAllLines(path).ToList();

            File.WriteAllLines(path, lines.GetRange(0, lines.Count - 1).ToArray(), Encoding.UTF8);

        }

        public static string FormalTextConverter(string text)
        {
            // Code to Captalise the first word of the sentence
            // Wikipedia will find a result if it is example Lion but if it is LION the search fails and gives null output

            StringBuilder formaloutput = new StringBuilder();
            if (text.Length > 0)
            {
                string[] words = text.ToLower().Split();
                foreach (string word in words)
                {
                    string newWord = word.Substring(0, 1);
                    newWord = newWord.ToUpper();
                    if (word.Length > 1)
                    {
                        newWord += word.Substring(1);
                    }
                    formaloutput.Append(newWord + " ");
                }
            }
            string output = formaloutput.ToString();
            return output;
        }

        public static string TrimtheText(string input)
        {
            // Code for trimming the whitespaces of the input 

            var removespace_end = input.Trim();
            return removespace_end;
        }

        //public static string SendingRecieveingRequestsfromWiki(string keyword)
        //{
        //    WebClient client = new WebClient();
        //    var response = client.DownloadString("https://en.wikipedia.org/w/api.php?action=query&prop=extracts&exsentences=2&exlimit=1&explaintext=1&format=json&exintro=1&titles=" + keyword);

        //    var responseJson = JsonConvert.DeserializeObject<RootObject>(response);
        //    var firstKey = responseJson.query.pages.First().Key;
        //    var extract = responseJson.query.pages[firstKey].extract;
        //    string result = extract;
        //    return result;
        //}

        private static int CheckNoOfWordsinInput(string inputword)
        {
            int i, outputword, l;
            l = 0;
            outputword = 1;

            /* loop till end of string */
            while (l <= inputword.Length - 1)
            {
                /* check whether the current character is white space or new line or tab character*/
                if (inputword[l] == ' ' || inputword[l] == '\n' || inputword[l] == '\t')
                {
                    outputword++;
                }

                l++;
            }

            return outputword;

            //if (outputword == 1)
            //{
            //    return inputword;
            //}
            //else if (outputword == 2)
            //{
            //    return inputword;
            //}
            //else if (outputword == 3)
            //{
            //    return inputword;
            //}
            //else
            //{
            //    return null;
            //}


        }

        private static string CodeforGettingWords(string inputforwho)
        {
            string[] spiltwords = inputforwho.Split(' ');

            string lastword = spiltwords[spiltwords.Length - 1];
            string lastwordsecond = spiltwords[spiltwords.Length - 2];

            string totallastwowords = String.Join(" ", lastwordsecond, lastword);

            //  string result = string.Join(" ", inputforwho.Split().Take(3));

            return totallastwowords;
        }

        private static string CodeforGettingFirstWordofSentence(string inputsentence)
        {
            string[] words = inputsentence.Split();
            var word = words.Take(2);


            if (words.Contains("The") || words.Contains("A") || words.Contains("the") || words.Contains("an") || words.Contains("An") || words.Contains("AN") || words.Contains("THE") || words.Contains("a"))
            {
                string secondword = word.Last();
                return secondword;
            }

            else
            {

                string firstWord = word.First();
                return firstWord;
            }
        }

        private static string CodeforGettingFirstTwoWordsSentence(string isentence) //edit
        {
            List<string> words = isentence.Split(' ').Take(3).ToList();

            if (words.Count >= 3)
            {
                var word_one = words[0];
                var word_two = words[1];
                var word_three = words[2];

                if (words.Contains("The") || words.Contains("A") || words.Contains("the") || words.Contains("an") || words.Contains("An") || words.Contains("AN") || words.Contains("THE") || words.Contains("a"))
                {
                    string secondthirdword = string.Join(" ", word_two, word_three);
                    return secondthirdword;
                }
                else
                {
                    string firsttwowords = string.Join(" ", isentence.Split().Take(2));
                    return firsttwowords;
                }
            }
            else
            {
                return isentence;
            }
        }

        public static void SaveImage(BitmapImage Image, string localFilePath)
        {
            var image = Image
                ;
            using (var fileStream = new FileStream(localFilePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }

        public static BitmapImage LoadtheImageSavedinPC(string context)
        {

            BitmapImage bitImage = new BitmapImage();
            bitImage.BeginInit();
            bitImage.UriSource = new Uri(@"G:\mkrAIMLbot\ImagesSavedWiki\" + context + ".jpg");
            bitImage.EndInit();

            return bitImage;
        }

        public static bool CheckImageIsSavedinPC(string context)
        {
            bool IsImageFound = false;

            //BitmapImage bitImage = new BitmapImage();
            //bitImage.BeginInit();
            //bitImage.UriSource = new Uri(@"G:\mkrAIMLbot\ImagesSavedWiki\" + context + ".jpg");
            //bitImage.EndInit();

            var folderPath = @"G:\mkrAIMLbot\ImagesSavedWiki\";

            string[] files = Directory.GetFiles(folderPath);
            foreach (string file in files)
            {
                var result = Path.GetFileNameWithoutExtension(file);

                if (result == (context))
                {
                    IsImageFound = true;
                    break;

                }
                else
                {
                    IsImageFound = false;
                }

            }


            if (IsImageFound == true)
            {
                return IsImageFound;
            }
            else
            {
                return IsImageFound;
            }

        }

        //string FindStringTakeX(string strValue, string findKey, int take, bool ignoreWhiteSpace = true)
        //{
        //    int index = strValue.IndexOf(findKey) + findKey.Length;

        //    if (index >= 0)
        //    {
        //        if (ignoreWhiteSpace)
        //        {
        //            while (strValue[index].ToString() == " ")
        //            {
        //                index++;
        //            }
        //        }

        //        if (strValue.Length >= index + take)
        //        {
        //            string result = strValue.Substring(index, take);

        //            return result;
        //        }


        //    }

        //    return string.Empty;
        //}


        private string input;
        public string Input
        {
            get { return input; }
            set
            {
                input = value;
                NotifyPropertyChanged("Input");
            }
        }

        private string chatOutput;
        public string ChatOutput
        {
            get { return chatOutput; }
            set
            {
                chatOutput = value;
                NotifyPropertyChanged("ChatOutput");
            }
        }
        private string chatInputtextbox;
        public string ChatInputtextbox
        {
            get { return chatInputtextbox; }
            set
            {
                chatInputtextbox = value;
                NotifyPropertyChanged("ChatInputtextbox");
            }
        }

        private List<string> listOfQuestionsToBot;
        public List<string> ListOfQuestionsToBot
        {
            get { return listOfQuestionsToBot; }
            set
            {
                listOfQuestionsToBot = value;
                NotifyPropertyChanged("ListOfQuestionsToBot");
            }
        }

        private string selectedQuestionfromQuestionsToBot;
        public string SelectedQuestionfromQuestionsToBot
        {
            get { return selectedQuestionfromQuestionsToBot; }
            set
            {
                selectedQuestionfromQuestionsToBot = value;
                NotifyPropertyChanged("SelectedQuestionfromQuestionsToBot");
            }
        }

        private ObservableCollection<ChatMessageModel> chatMessageCollection;


        public ObservableCollection<ChatMessageModel> ChatMessageCollection
        {
            get { return chatMessageCollection; }
            set
            {
                chatMessageCollection = value;
                NotifyPropertyChanged("ChatMessageCollection");
            }
        }
    }

}
