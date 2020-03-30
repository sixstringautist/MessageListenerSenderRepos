using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace MessageSender
{
    class Program
    {
        static MessageSheduler scheduler;
        static string connectionString = ConfigurationManager.ConnectionStrings["LocalStorage"].ConnectionString;
        static void Main(string[] args)
        {
            using (var context = new AppDbContext(connectionString))
            {
                scheduler = new MessageSheduler(new Uri(args[0]), context.Messages);
            }
            scheduler.OnRetry += OnRtry;
            while (true)
            {
                string msg = Console.ReadLine();
                try
                {
                    AddMsg(msg);
                    scheduler.AddMessageToQueue(msg);
                }
                catch (ValidationException ex)
                {
                    Console.WriteLine($"Произошла ошибка валидации:{ex.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }

        }

        static void AddMsg(string msg)
        {
            using (var cnt = new AppDbContext(connectionString))
            {
                cnt.Messages.Add(new Message(msg));
                cnt.SaveChanges();
            }
        }
        static void OnRtry(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
