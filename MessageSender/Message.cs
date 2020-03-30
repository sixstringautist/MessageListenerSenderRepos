using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageSender
{
    class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        [Required]
        public string Text { get; private set; }
        public Message(string msg) => Text = msg;
        public Message() { }
    }
}
