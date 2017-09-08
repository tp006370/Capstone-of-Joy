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

        //These are the responses from the Azure Bot Code
        String IntentGPA = "You have reached the myGPA intent. You said: ";
        String IntentInteract = "You have reached the Interact intent. You said: ";
        String IntentDates = "You have reached the DatesAndDeadlines intent. You said: ";
        String IntentSubjects = "You have reached the Subjects intent. You said: ";
        String IntentProf = "You have reached the Professor intent. You said: ";
        String IntentTeaches = "You have reached the teaches intent. You said: ";

        [ClassInitialize()]
        public static void initBotInteractionProgram(TestContext context)
        {

            oThread.Start();


        }


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
            while(tryCounter < 50000001)
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

        public void requestGPATest()
        {
            string intentQuestion = "Hello Bot, What is my GPA";
            getBotResponseTest(intentQuestion, IntentGPA + intentQuestion);


        }

        [TestMethod()]

        public void requestDatesTest()
        {
            String intentQuestion = "When can i register for SWENG500?";
            getBotResponseTest(intentQuestion, IntentDates + intentQuestion);

        }

        [TestMethod()]

        public void requestTeachesTest()
        {
            String intentQuestion = "who teaches SWENG500";
            getBotResponseTest(intentQuestion, IntentTeaches + intentQuestion);

        }

        [TestMethod()]

        public void requestSubjectsTest()
        {
            String intentQuestion = "what courses are available this fall?";
            getBotResponseTest(intentQuestion, IntentSubjects + intentQuestion);

        }

        //Same code as the 2nd part of the InitBot connection test
        //the input string and Assert String are now passed as variabales
        public void getBotResponseTest(string input, String AssertText)
        {

            //Create an activity to recieved the returned text
            Microsoft.Bot.Connector.DirectLine.Activity temp = new Microsoft.Bot.Connector.DirectLine.Activity();

            //establish a retry counter, to give the BOT time to respond, in the future this can correspond to a timeout requirement
            int tryCounter = 0;


            //Send the first message to the bot to establish the connection
            Program.setBotMessage(input);


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
                    //Doing the Assert to compare the exact value
                    Assert.AreEqual(temp.Text, AssertText);
                    break;

                }

                tryCounter++;

            }
        }

        [ClassCleanup()]
        public static void stopBotInteractionProgram()
        {
            oThread.Abort();

        }


    }
}