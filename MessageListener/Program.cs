using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MessageListener
{
    class Program
    {
        static SimpleListener listener = new SimpleListener();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                listener.Dispose();
            };

            listener.OnMessage += (msg) =>
            {
                using (var repos = new MessageRepository(ConfigurationManager.ConnectionStrings["LocalStorage"].ConnectionString))
                {
                    repos.Add(msg);
                    repos.SaveChanges();
                }
            }; ;
            StartListen(args[0]);
            while (true)
            {
                var line = Console.ReadLine();
                if (line == "print")
                {
                    Print();
                }
                else { Console.WriteLine("Такой команды нет."); }
            }
        }

        static void StartListen(string prefix)
        {
            listener.AddPrefix(prefix);
            listener.StartListen();
        }


        public static void Print()
        {
            List<Message> msgs;
            using (var repos = new MessageRepository(ConfigurationManager.ConnectionStrings["LocalStorage"].ConnectionString))
            {
                msgs = repos.GetAll<Message>().ToList();
            }
            var builder = new StringBuilder();
            builder.AppendFormat("{0, -2} {1,-15} {2, -30} {3, -16}\n", "id", "ip", "Text", "Date");
            msgs.ForEach(msg =>
            {
                builder.Append($"{msg.Id,-2} {msg.IpAdress,-15} {msg.Text,-30} {msg.MessageCreationTime:dd.MM.yyyy HH:mm}\n");
            });
            Console.WriteLine(builder.ToString());
        }
    }
}
