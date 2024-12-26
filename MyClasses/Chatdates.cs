using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClasses
{
    public class Chatdates
    {
        public int chat_id { get; set; }
        public int match_id { get; set; }
        public int sender_id { get; set; }
        public string message { get; set; }
        public DateTime sent_at { get; set; } = DateTime.Now;

        // Constructor
        public Chatdates(int matchId, int senderId, string message)
        {
            match_id = matchId;
            sender_id = senderId;
            this.message = message;
            sent_at = DateTime.Now;
        }
    }
}