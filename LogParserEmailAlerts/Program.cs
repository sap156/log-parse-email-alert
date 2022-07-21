using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;

namespace SendEmailAlerts
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                string rootPath = @"C:\Users\pasa03\Desktop\Logs"; //Path to the Logs folder
                //var files = Directory.GetFiles(rootPath, "*.log*", SearchOption.AllDirectories); //Fetches the Path to each Log file
                //Fetches only the latest log files newer than yesterday
                var files = Directory.GetFiles(rootPath, "*.log*", SearchOption.AllDirectories).Where(x => new FileInfo(x).CreationTime.Date > DateTime.Today.AddDays(-1));
                List<string> errorList = new List<string>(); //stores the result of the condition loop in a List string
                                                             //string logPath = @"C:\Users\pasa03\Desktop\Logs\log.txt"; //Path to the log file if you want to write the output into a log

                foreach (string file in files)
                {

                    StreamReader myReader = new StreamReader(file); //goes through each log file path
                    string line = "";

                    while (line != null) //as long as the file does not have a null line
                    {

                        line = myReader.ReadLine(); //as long as the file does not have a null line read the line

                        if (line != null) //check if the line is not null
                        {

                            if (line.Contains("result=[false]") == true) //check for the TEXT you want to search in the log file
                            {

                                string datetime = line.Split(' ')[0] + " " + line.Split(' ')[1]; //getting the 1st and 2nd words in the line
                                string tablename = line.Split(' ')[3]; //getting the 3rd word in the line

                                line = String.Format("Date : {0} -- Table : {1} ", datetime, tablename); //gets the line with 1st,2nd and 3rd word
                                                                                                         //Console.WriteLine(line);
                                errorList.Add(line); //add the above line to the List String

                            }

                        }

                    }

                    myReader.Close(); //close the opened log File and goes on to the next log file

                }
                //errorList.ForEach(r => Console.WriteLine(r)); //prints out the list of errors
                StringBuilder emailAlertLogs = new StringBuilder(); //To store the contents of the list in a String 

                // using (StreamWriter writetext = new StreamWriter(logPath)) //To write the results into a .txt file
                //{
                foreach (string line in errorList) //for each line in the List String append the line to a String which will be sent as an email
                {
                    emailAlertLogs.AppendLine(line + "\n"); //appending each line in the List String to the String 
                    // Console.WriteLine(line);
                }

                // }
                // Console.WriteLine("Log writting complete");

                //Sending email

                string Username = "STRIIM";
                string Password = "SMSds123";
                string to = "sai.parvathaneni@sms-group.com";
                string from = "sai.parvathaneni@sms-group.com";
                MailMessage message = new MailMessage(from, to);
                message.To.Add("sai.parvathaneni@sms-group.com");
                message.Subject = "Checks Issues for BRSDB ETL";
                message.Body = emailAlertLogs.ToString();

                SmtpClient client = new SmtpClient("us-smtp.sms-group.com");
                client.Port = 25;
                client.EnableSsl = false;
                client.Credentials = new NetworkCredential(Username, Password);

                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
            // Environment.Exit(0);

        }
    }
}