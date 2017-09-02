using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;

namespace BotTest
{
    class Program
    {

        private static string directLineSectret = "q7RPfg38hkE.cwA.pns.oxVS_JH5i-Kh9AWKooPsUtLh8q6fKxi-QHDZx2Tk6xU";
        private static string botId = "TheVirtualAdvisor";
        private static string fromUser = "DirectLineClientUser";


        static void Main(string[] args)
        {
            StartBotConversation().Wait();

        }

        private static async Task StartBotConversation()
        {
            DirectLineClient client = new DirectLineClient(directLineSectret);

            var conversation = await client.Conversations.StartConversationAsync();

            new System.Threading.Thread(async () => await ReadBotMessagesAsync(client, conversation.ConversationId)).Start();

            Console.Write("Command> ");



            //This is a change
            Activity NewMessage1 = new Activity
            {
                From = new ChannelAccount(fromUser),
                Text = "Hello",
                Type = ActivityTypes.Message
            };

            await client.Conversations.PostActivityAsync(conversation.ConversationId, NewMessage1);


            while (true)
            {
                string input = Console.ReadLine().Trim();

                if (input.ToLower() == "exit")
                {
                    break;
                }
                else
                {
                    if (input.Length > 0)
                    {
                        Activity userMessage = new Activity
                        {
                            From = new ChannelAccount(fromUser),
                            Text = input,
                            Type = ActivityTypes.Message
                        };

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
                    Console.WriteLine(activity.Text);


                    if (activity.Text.Contains("Hello"))
                    {
                        Console.Write("Test Pass \n");
                    }
                

                    Console.Write("Command> ");
                }

                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
        }

      
    }
}