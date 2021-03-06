﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Wtalk.Desktop.CustomDataTemplate;
using WTalk.Mvvm;

namespace Wtalk.Desktop.DesignData
{
    internal class DesignConversationViewModel : ObservableObject
    {
        public dynamic Conversation { get; set; }
        public string ConversationName { get { return "TEST"; } }
        public string LastMessage { get { return "Last message"; } }
        public bool HasUnreadMessages { get; set; }
        public DesignConversationViewModel()
        {
            Conversation = new { MessagesHistory = new List<dynamic>(), HistoryEnabled  = true};            
            
            Conversation.MessagesHistory.Add(
                new
                {
                    SenderId = "2",
                    Content = "Message 1 ",
                    MessageDate = DateTime.Now,
                    IncomingMessage = true
                    
                }
                );

            Conversation.MessagesHistory.Add(
                new
                {
                    SenderId = "1",
                    Content = "Message 2 ",
                    MessageDate = DateTime.Now,
                    IncomingMessage = false
                }
                );
        }
    }
}
