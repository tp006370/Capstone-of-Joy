using Microsoft.VisualStudio.TestTools.UnitTesting;
using BotTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.DirectLine;
using System.Threading;

namespace BotTest.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        static Thread oThread = new Thread(new ThreadStart(Program.initBotConversation));
        String IntentGPA = "You have reached the myGPA intent. You said: ";
        String IntentInteract = "You have reached the Interact intent. You said: ";
        String IntentDates = "You have reached the DatesAndDeadlines intent. You said: ";
        String IntentSubjects = "You have reached the Subjects intent. You said: ";
        String IntentProf = "You have reached the Professor intent. You said: ";

        [ClassInitialize()]
        public static void initBotInteractionProgram(TestContext context)
        {
            oThread.Start();
        }

        [ClassCleanup()]
        public static void stopBotInteractionProgram()
        {
            oThread.Abort();
        }

        #region Test cases
        [TestMethod()]

        //This test, tests the connection to the bot
        public void InitBotConnectionTest()
        {
            bool Init_Message_Interaction_Complete = false;

            //Send the first message to the bot to establish the connection
            Program.setBotMessage("Hello");

            //Create an activity to recieved the returned text
            Microsoft.Bot.Connector.DirectLine.Activity temp = new Microsoft.Bot.Connector.DirectLine.Activity();

            //See if the bot has responded
            temp = Program.getBotMessage();

            //establish a retry counter, to give the BOT time to respond, in the future this can correspond to a timeout requirement
            int tryCounter = 0;

            //retry for awhile initially
            while (tryCounter < 50000001)
            {
                //The BOTInterction program sets text of ERROR if there are no responses from the BOT
                if (!temp.Text.Contains("ERROR"))
                {
                    //Burp out the text to the console.
                    Console.WriteLine("Recieved Text Contains: " + temp.Text);

                    //Set a small logic flag
                    Init_Message_Interaction_Complete = true;
                    break;
                }

                //Look to see of the BOT has responded
                temp = Program.getBotMessage();

                //Increment the counter
                tryCounter++;
            }

            //Ok got initial response, now reset the counter to look for the message echo
            tryCounter = 0;
            if (Init_Message_Interaction_Complete)
            {
                while (tryCounter < 50000001)
                {
                    //Try to get the BOT response
                    temp = Program.getBotMessage();

                    //If there is a response
                    if (!temp.Text.Contains("ERROR"))
                    {
                        Console.WriteLine("Recieved Text Contains: " + temp.Text);

                        //Here is the actual test Pass/Fall Test, we sent "Hello", we expect the returned message to contain "Hello".
                        StringAssert.Contains(temp.Text, "Hello");
                        break;
                    }

                    tryCounter++;
                }
            }
        }

        [TestMethod()]
        public void requestCummulativeGPATest()
        {
            String input = "What is my GPA";
            String response = GetBotResponseTest(input);
            Assert.AreEqual(response, IntentGPA + input);
        }


        [TestMethod()]
        public void requestClassGPATest()
        {
            String input = "Hello Bot, What is my GPA for class XYZ";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "3.2");
        }


        [TestMethod()]
        public void requestAnothersGPA()
        {
            String input = "Hello Bot, What is Henry's GPA";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "Error, you can't have another persons GPA");
        }

        [TestMethod()]
        public void requestCredits()
        {
            String input = "Hello Bot, how many credits do I have";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "36");
        }

        [TestMethod()]
        public void requestCourseDate()
        {
            String input = "Hello Bot, how when does SWENG500 end";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "Never");
        }

        public string GetBotResponseTest(String input)
        {
            //Create an activity to recieved the returned text
            Microsoft.Bot.Connector.DirectLine.Activity temp = new Microsoft.Bot.Connector.DirectLine.Activity();

            //establish a retry counter, to give the BOT time to respond, in the future this can correspond to a timeout requirement
            int tryCounter = 0;

            //Send the first message to the bot to establish the connection
            Program.setBotMessage(input);
            //"Hello Bot, What is my total GPA"

            //retry for awhile initially
            while (tryCounter < 50000001)
            {
                //Look to see of the BOT has responded
                temp = Program.getBotMessage();

                //The BOTInterction program sets text of ERROR if there are no responses from the BOT
                if (!temp.Text.Contains("ERROR"))
                {
                    //Burp out the text to the console.
                    Console.WriteLine("Recieved Text Contains: " + temp.Text);
                    // StringAssert.Contains(temp.Text, "myGPA intent");
                    break;
                }

                tryCounter++;
            }
            return temp.Text;
        }
        #endregion
    }
}