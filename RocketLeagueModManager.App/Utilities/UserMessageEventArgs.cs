using System;
using System.Collections.Generic;
using System.Text;

namespace RocketLeagueModManager.App.Utilities
{
    public class UserMessageEventArgs : EventArgs
    {
        public string Message { get; }

        public UserMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
