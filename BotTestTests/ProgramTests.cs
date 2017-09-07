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

            Thread oThread = new Thread(new ThreadStart(Program.initBotConversation));


        [TestInitialize()]
        public void initBotInteractionProgram()
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

                    //Now send another message to the BOT and see if the BOT will echo the meesage
                    Program.setBotMessage("Hello Bot, What is my GPA");

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

            Console.WriteLine("TODO Tommorow");


        }

        [TestCleanup()]
        public void stopBotInteractionProgram()
        {
            oThread.Abort();

        }


    }
}