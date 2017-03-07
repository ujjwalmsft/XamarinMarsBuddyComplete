using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MarsBuddy
{
    class BotConnection
    {
        public DirectLineClient Client = new DirectLineClient("Z1manpIsazM.cwA.6e4.w3TbL5dPeJKMTivS4iEl_ByEwr760KESaTQ7ftSX2T8");
        public Conversation MainConversation;
        public ChannelAccount Account;

        public BotConnection(string accountName)
        {
            MainConversation = Client.Conversations.StartConversation();
            Account = new ChannelAccount() { Id = accountName, Name = accountName };
        }

        public void SendMessage(string message)
        {
            Activity activity = new Activity
            {
                From = Account,
                Text = message,
                Type = ActivityTypes.Message
            };
            Client.Conversations.PostActivity(MainConversation.ConversationId, activity);
        }

        public async Task GetMessagesAsync(ObservableCollection<MessageListItem> collection)
        {
            string watermark = null;

            while (true)
            {
                Debug.WriteLine("Reading message every 3 seconds");

                var activitySet = await Client.Conversations.GetActivitiesAsync(MainConversation.ConversationId, watermark);
                watermark = activitySet?.Watermark;

                foreach (Activity activity in activitySet.Activities)
                {
                    if (activity.From.Name == "MarsBot")
                    {
                        collection.Add(new MessageListItem(activity.Text, activity.From.Name));
                    }
                }

                await Task.Delay(3000);
            }
         }


    }
}
