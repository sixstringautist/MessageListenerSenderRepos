using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageListener
{
    class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required]
        public string Text { get; private set; }
        [RegularExpression(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$")]
        public string IpAdress { get; set; }
        public DateTimeOffset MessageCreationTime { get; private set; }

        public Message() { }
        public Message(string msg, string ip)
        {
            Text = msg;
            MessageCreationTime = DateTimeOffset.Now;
        }

    }
}
