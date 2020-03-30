using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MessageSender
{
    class MessageSender : HttpClient
    {
        public MessageSender(Uri u)
        {
            base.BaseAddress = u;
        }
        public Task<HttpResponseMessage> SendMessage(string msg)
        {
            return PostAsync(BaseAddress.AbsoluteUri, new StringContent(msg));
        }

    }
}
