using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;

namespace MarsBuddy
{
	public partial class MainPage : ContentPage
	{
        //Initialize a connection with ID and Name
        BotConnection connection = new BotConnection("Ujjwal");

        //ObservableCollection to store the messages to be displayed
        ObservableCollection<MessageListItem> messageList = new ObservableCollection<MessageListItem>();

        //Computer Vision client
        VisionServiceClient visionClient = new VisionServiceClient("775c63123c104445bbc227eb90496098");

        public MainPage()
		{
			InitializeComponent();

            //Bind the ListView to the ObservableCollection
            MessageListView.ItemsSource = messageList;

            //Start listening to messages
            var messageTask = connection.GetMessagesAsync(messageList);

            //Initialize camera plugin
            CrossMedia.Current.Initialize();
        }

        //Send method for message entry
        public void Send(object sender, EventArgs args)
        {
            //Get text in entry
            var message = ((Entry)sender).Text;

            if(message.Length > 0)
            {
                //Clear entry
                ((Entry)sender).Text = "";

                //Make object to be placed in ListView
                var messageListItem = new MessageListItem(message, connection.Account.Name);
                messageList.Add(messageListItem);

                //Send the message to the bot
                connection.SendMessage(message);
            }
        }

        //TakePic method for button
        public async void TakePic(object sender, EventArgs args)
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                //Supply media options for saving our photo after it's taken.
                var mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Receipts",
                    Name = $"{DateTime.UtcNow}.jpg"
                };

                //Get chosen file stream
                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);
                var fileStream = file.GetStream();

                //Display loading
                await DisplayAlert("Loading Result", "Please wait", "OK");
                
                //Send file to ComputerVision
                var result = await GetImageDescriptionAsync(fileStream);
                await DisplayAlert("Detection Result", "I think it's " + result.Description.Captions[0].Text, "OK");
            }
        }

        public async Task<AnalysisResult> GetImageDescriptionAsync(Stream imageStream)
        {
            VisualFeature[] features = { VisualFeature.Tags, VisualFeature.Categories, VisualFeature.Description };
            var result = await visionClient.AnalyzeImageAsync(imageStream, features.ToList(), null);
            return result;
        }

    }
}
