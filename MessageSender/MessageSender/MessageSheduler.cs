using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace MessageSender
{
    class MessageSheduler
    {
        private delegate Task<HttpResponseMessage> msgTsk();
        private readonly MessageSender sender;
        private Queue<msgTsk> MessageTasks = new Queue<msgTsk>();
        public event Action<string> OnRetry;
        public IList<Message> Messages { get; private set; }
        private Timer t;
        public MessageSheduler(Uri u, IEnumerable<Message> messages)
        {
            sender = new MessageSender(u);
            t = new Timer();
            t.Elapsed += OnTimerElapsed;
            t.AutoReset = false;
            Messages = messages.ToList();
            foreach (var el in Messages)
            {
                MessageTasks.Enqueue(() => sender.SendMessage(el.Text));
            }
            EnqueueCallback(TimeSpan.FromMilliseconds(100).TotalMilliseconds);
        }

        public void AddMessageToQueue(string msg)
        {
            Messages.Add(new Message(msg));
            MessageTasks.Enqueue(() => sender.SendMessage(msg));
            t.Stop();
            EnqueueCallback(TimeSpan.FromMilliseconds(100).TotalMilliseconds);
        }
        private void EnqueueCallback(double ms)
        {
            if (MessageTasks.Count > 0)
            {
                t.Interval = ms;
                t.Start();
            }
        }

        private async void OnTimerElapsed(object sender, EventArgs e)
        {
            var tmp = MessageTasks.Peek();
            try
            {
                var rsp = await tmp();

                if (rsp.IsSuccessStatusCode)
                {
                    MessageTasks.Dequeue();
                    EnqueueCallback(TimeSpan.FromMilliseconds(100).TotalMilliseconds);
                }
                else
                {
                    EnqueueCallback(TimeSpan.FromSeconds(20).TotalMilliseconds);
                    OnRetry?.Invoke($"Не удалось отправить сообщение({rsp.StatusCode.ToString()}). Следующая попыдка через 20 сек");
                }
            }
            catch (HttpRequestException ex)
            {
                EnqueueCallback(TimeSpan.FromSeconds(20).TotalMilliseconds);
                OnRetry?.Invoke($"Произошла ошибка соединения({ex.InnerException?.Message}) Следующая попытка через 20 сек");
            }
        }

    }
}
