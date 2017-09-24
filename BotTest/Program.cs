using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;

namespace BotTest
{
   public class Program
    {
        //Test
        private static string directLineSectret = "vQTXyG-I4nU.cwA.Mq0.3nnFdaS4OFYaMlVGQW678nzLKklUFPnjHLwl_N3HOKE";
        private static string botId = "VirtualAdvisorv2";
        private static string fromUser = "DirectLineClientUser";

        private static ConcurrentQueue<string> messagesToBot = new ConcurrentQueue<string>();
        private static  ConcurrentQueue<Activity> messagesFromBot = new ConcurrentQueue<Activity>();


        static void Main(string[] args)
        {
           
            StartBotConversation().Wait();

        }





        private static async Task StartBotConversation()
        {
            DirectLineClient client = new DirectLineClient(directLineSectret);

            var conversation = await client.Conversations.StartConversationAsync();

            new System.Threading.Thread(async () => await ReadBotMessagesAsync(client, conversation.ConversationId)).Start();

 //                   Console.Write("Command> ");

            Activity userMessage = new Activity
            {
                From = new ChannelAccount(fromUser),
                Type = ActivityTypes.Message
            };

            //This is a change
 /*                     Activity NewMessage1 = new Activity
                      {
                          From = new ChannelAccount(fromUser),
                          Text = "Hello",
                          Type = ActivityTypes.Message
                      }; 

                     await client.Conversations.PostActivityAsync(conversation.ConversationId, NewMessage1);
                     */

            while (true)
            {
 //               string input = Console.ReadLine().Trim();

 /*               if (input.ToLower() == "exit")
                {
                    break;
                }
                */

    //            messagesToBot.Enqueue(input);

                if (!messagesToBot.IsEmpty)
                {
                   
                    string message;
                    if (messagesToBot.TryDequeue(out message))
                    {
                        userMessage.Text = message;


                        await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);
                    }
                }
            }
        }

        private static async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId)
        {
            string watermark = null;

            while (true)
            {
                var activitySet = await client.Conversations.GetActivitiesAsync(conversationId, watermark);
                watermark = activitySet?.Watermark;

                var activities = from x in activitySet.Activities
                                 where x.From.Id == botId
                                 select x;

                foreach (Activity activity in activities)
                {
                    messagesFromBot.Enqueue(activity);
 //                   Console.WriteLine(activity.Text);
                }

                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
        }

      public static void initBotConversation()
        {
            Console.WriteLine("Starting Thread");
            StartBotConversation().Wait();
          

        }


        public static void setBotMessage(string message)
        {
            messagesToBot.Enqueue(message);
                        
        }

        public static Microsoft.Bot.Connector.DirectLine.Activity getBotMessage()
        {
            Activity tempActivity = new Activity();

            
            if (!messagesFromBot.TryDequeue(out tempActivity))
            {
                Activity ErrorActivity = new Activity
                {
                    From = new ChannelAccount(fromUser),
                    Text = "ERROR, No Message in Queue",
                    Type = ActivityTypes.Message
                };

                return ErrorActivity;
              //  tempActivity.Text = "ERROR, No Message in Queue";
            }

            return tempActivity;
        }



    }
}