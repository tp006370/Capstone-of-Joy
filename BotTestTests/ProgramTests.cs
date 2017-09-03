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

        [TestMethod()]
        public void getBotMessageTest()
        {

    

            Thread oThread = new Thread(new ThreadStart(Program.initBotConversation));

            oThread.Start();
           
            Program.setBotMessage("Hello");

            Microsoft.Bot.Connector.DirectLine.Activity temp = new Microsoft.Bot.Connector.DirectLine.Activity();

            temp = Program.getBotMessage();

            int tryCounter = 0;

            while(tryCounter < 50000001)
            {
                temp = Program.getBotMessage();

                if (!temp.Text.Contains("ERROR"))
                {
                    Console.WriteLine("Text does not contain Error, Text Contains: " + temp.Text);
                    StringAssert.Contains(temp.Text, "Welcome");
                    break;

                }

                tryCounter++;

            }

            Console.WriteLine("Text Contains: " + temp.Text);
            //   StringAssert.Contains(temp.Text, "Hello");

            oThread.Abort();
 
        }
    }
}