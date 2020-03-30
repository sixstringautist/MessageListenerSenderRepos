using System;
using System.IO;
using System.Net;
using System.Text;

namespace MessageListener
{
    class SimpleListener : IDisposable
    {
        HttpListener listener = new HttpListener();

        public event Action<Message> OnMessage;

        private async void MainWorkLoop()
        {
            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;
                switch (request.RawUrl.ToLower())
                {
                    case "/postmessage":
                        if (request.HttpMethod == "POST")
                        {
                            OnMessage(GetMessage(request));
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.Close();
                        }
                        else { response.StatusCode = (int)HttpStatusCode.BadRequest; response.Close(); }
                        break;
                    default:
                        DefaultCase(response);
                        break;
                }
            }
        }
        public void StartListen()
        {
            listener.Start();
            MainWorkLoop();
        }
        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }
        public void AddPrefix(string prefix)
        {
            listener.Prefixes.Add(prefix);
        }
        private byte[] ConvertToUTF8(string s)
        {
            var utf8 = Encoding.UTF8;
            var utf16 = Encoding.Unicode;
            byte[] output = Encoding.Convert(utf16, utf8, utf16.GetBytes(s));
            return output;
        }

        private Message GetMessage(HttpListenerRequest request)
        {
            string msg;
            IPEndPoint ip;
            using (var Reader = new StreamReader(request.InputStream))
            {
                msg = Reader.ReadToEnd();
                ip = request.RemoteEndPoint;
            }
            return new Message(msg, ip.Address.ToString());
        }
        private void DefaultCase(HttpListenerResponse response)
        {
            string GenerateContent(string s)
            {
                return "<html><head><meta charset='utf8'></head><body>" + s + "</body></html>";
            };
            byte[] answer = ConvertToUTF8(GenerateContent("Такого метода нет"));
            response.ContentLength64 = answer.LongLength;
            response.ContentType = "text/html";
            using (Stream s = response.OutputStream)
            {
                s.Write(answer, 0, answer.Length);
                response.Close();
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                listener.Stop();
                listener.Close();
            }
        }
    }


}
