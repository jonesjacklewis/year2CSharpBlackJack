using OOPA1.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Models
{
    // models a message
    public class Message
    {
        public DateTime DateTime { get; set; }
        public String Username { get; set; }
        public int CurrentBalancePence { get; set; }
        public MessageStates MessageState { get; set; }

        // polymorphic constructors, one takes a datetime, whereas the other uses the current datetime instead

        public Message(DateTime dateTime, string username, int currentBalancePence, MessageStates messageState)
        {
            DateTime = dateTime;
            Username = username;
            CurrentBalancePence = currentBalancePence;
            MessageState = messageState;
        }

        public Message(string username, int currentBalancePence, MessageStates messageState)
        {
            DateTime = DateTime.Now;
            Username = username;
            CurrentBalancePence = currentBalancePence;
            MessageState = messageState;
        }
    }
}
