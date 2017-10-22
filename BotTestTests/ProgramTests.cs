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
using System.Data.SqlClient;

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
        static SqlConnection theConnection;

        [ClassInitialize()]
        public static void initBotInteractionProgram(TestContext context)
        {
            oThread.Start();                  
            theConnection = new SqlConnection("Data Source = sweng500.database.windows.net,1433; Initial Catalog = VirtualAdvisor2; Integrated Security = False; User ID = sweng500; Password = Capstone@123; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
            InitBotConnection();
        }

        [ClassCleanup()]
        public static void stopBotInteractionProgram()
        {
            // restore the grades (changed during the tests)
    		SqlCommand UpdateGPA_1 = new SqlCommand();
            UpdateGPA_1 = theConnection.CreateCommand();
            UpdateGPA_1.CommandText = @"UPDATE dbo.StudentCourseHistory SET Grade='A'" + " WHERE StudentId=2 and CourseName='Advanced Software Engineering Studio'";

            SqlCommand UpdateGPA_2 = new SqlCommand();
            UpdateGPA_2 = theConnection.CreateCommand();
            UpdateGPA_2.CommandText = @"UPDATE dbo.StudentCourseHistory SET Grade='A'" + " WHERE StudentId=2 and CourseName='Intro Software Testing'";

            theConnection.Open();
            UpdateGPA_1.ExecuteNonQuery();
            UpdateGPA_2.ExecuteNonQuery();

            theConnection.Close();

            oThread.Abort();
        }

        #region Test cases

        public static void InitBotConnection()
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
            while (tryCounter < 1000000001)
            {
                //The BOTInterction program sets text of ERROR if there are no responses from the BOT
                //      if (!temp.Text.Contains("ERROR, ") ||
                //          !temp.Text.Contains("Welcome to The Virtual Advisor") ||
                //          !temp.Text.Contains("I know lots of things about your school and your records") ||
                //          !temp.Text.Contains("Try typing questions about educational subjects, some are supplied below.."))
                if (temp.Text.Contains("Hello"))
                {
                    //Burp out the text to the console.
                    Console.WriteLine("Recieved Text Contains: " + temp.Text + "Counter Value: " + tryCounter.ToString());

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
            StringAssert.Contains(response, "Your GPA is: 4");
        }

        [TestMethod()]
        public void repeatRequestCummulativeGPATest()
        {
            String input = "What is my GPA";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "Your GPA is: 4");
        }

        [TestMethod()]
        public void TestGPACaluation4_0()
        {
            SqlCommand UpdateGPA = new SqlCommand();
            UpdateGPA = theConnection.CreateCommand();
            UpdateGPA.CommandText = @"UPDATE dbo.StudentCourseHistory SET Grade='A-'" + " WHERE StudentId=2 and CourseName='Advanced Software Engineering Studio'";

            theConnection.Open();
            UpdateGPA.ExecuteNonQuery();
            theConnection.Close();

            Thread.Sleep(3000);

            String input = "Bot what is my GPA";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "Your GPA is: 3.92");
        }

        [TestMethod()]
        public void TestGPACaluation3_0()
        {
            SqlCommand UpdateGPA_1 = new SqlCommand();
            UpdateGPA_1 = theConnection.CreateCommand();
            UpdateGPA_1.CommandText = @"UPDATE dbo.StudentCourseHistory SET Grade='B'" + " WHERE StudentId=2 and CourseName='Advanced Software Engineering Studio'";

            SqlCommand UpdateGPA_2 = new SqlCommand();
            UpdateGPA_2 = theConnection.CreateCommand();
            UpdateGPA_2.CommandText = @"UPDATE dbo.StudentCourseHistory SET Grade='B'" + " WHERE StudentId=2 and CourseName='Intro Software Testing'";

            theConnection.Open();
            UpdateGPA_1.ExecuteNonQuery();
            UpdateGPA_2.ExecuteNonQuery();

            theConnection.Close();

            Thread.Sleep(3000);

            String input = "Bot what is my GPA";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "Your GPA is: 3.25");
        }

        [TestMethod()]
        public void TestGPACaluation2_0()
        {
            SqlCommand UpdateGPA_1 = new SqlCommand();
            UpdateGPA_1 = theConnection.CreateCommand();
            UpdateGPA_1.CommandText = @"UPDATE dbo.StudentCourseHistory SET Grade='C'" + " WHERE StudentId=2 and CourseName='Advanced Software Engineering Studio'";

            SqlCommand UpdateGPA_2 = new SqlCommand();
            UpdateGPA_2 = theConnection.CreateCommand();
            UpdateGPA_2.CommandText = @"UPDATE dbo.StudentCourseHistory SET Grade='C'" + " WHERE StudentId=2 and CourseName='Intro Software Testing'";

            theConnection.Open();
            UpdateGPA_1.ExecuteNonQuery();
            UpdateGPA_2.ExecuteNonQuery();

            theConnection.Close();

            Thread.Sleep(3000);

            String input = "Bot what is my GPA";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "Your GPA is: 2");
        }

        [TestMethod()]
        public void requestCourseTeacher()
        {
            String input = "Hello Bot, who teaches SWENG500";
            String response = GetBotResponseTest(input);
            if (response.Contains("sweng500"))
            {
                String inputResponse = "yes";
                String response2 = GetBotResponseTest(inputResponse);
                StringAssert.Contains(response2, "Professor Curry Fee");
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void requestTotalCredits()
        {
            String input = "Hello Bot, how many credits do I have";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "12");
        }

        [TestMethod()]
        public void requestClassGPATest()
        {
            String input = "Hello Bot, What is my GPA for class SWENG500";
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
        public void requestSpringSemester2017GPA()
        {
            String input = "Hello Bot, What was my GPA for the Spring 2017 semester";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "2.0");
        }

        [TestMethod()]
        public void requestSummerSemester2017GPA()
        {
            String input = "Hello Bot, What was my GPA for the Summer 2017 semester";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "2.0");
        }

        [TestMethod()]
        public void requestFallSemester2017GPA()
        {
            String input = "Hello Bot, What was my GPA for the Fall 2017 semester";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "3.0");
        }

        [TestMethod()]
        public void requestSpringSemester2017Credits()
        {
            String input = "Hello Bot, How many credits for spring 2017";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "3");
        }

        [TestMethod()]
        public void requestSummerSemester2017Credits()
        {
            String input = "Hello Bot, How many credits for summer 2017";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "3");
        }

        [TestMethod()]
        public void requestFallSemester2017Credits()
        {
            String input = "Hello Bot, How many credits for fall 2017";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "6");
        }

        [TestMethod()]
        public void requestDepartmentStaff()
        {
            String input = "Hello Bot, what teachers are in the computer science department";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "Professor Ed, Professor Twain");
        }

        [TestMethod()]
        public void requestCourseDate()
        {
            String input = "Hello Bot, how when does SWENG500 end";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "12/3/2017");
        }

        [TestMethod()]
        public void requestProfeseorRating()
        {
            String input = "What is Professor Stephens rating";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "9");
        }

        [TestMethod()]
        public void requestClassBasedOnSubject()
        {
            String input = "Bot is there a class on Software System Design";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "SWENG537");
        }

        [TestMethod()]
        public void requestProfessorEmailAddress()
        {
            String input = "Bot what is Professor's Stephens email address";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "rstevens@psu.edu");
        }

        [TestMethod()]
        public void requestCourseSinglePrerequisitesTest()
        {
            String input = "What are the prerequisites for course ABS7110";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "ABS7100");
        }

        [TestMethod()]
        public void requestCourseMultiplePrerequisitesTest()
        {
            String input = "What are the prerequisites for course SWENG500";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "ABS7120, ABS7220, ABS7230, ABS7600, ABS7601");
        }

        [TestMethod]
        public void ProfessorTeachesTest()
        {
            String input = "What does Professor Curry teach";
            String response = GetBotResponseTest(input);
            StringAssert.Contains(response, "There are 1 professor(s) who match this name.\n\n Curry Fee: CEG2170L, SWENG500");
        }

        public static string GetBotResponseTest(String input)
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
