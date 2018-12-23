using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexLibrary.Line.NotifyBot.Models
{
    public class LineNotifyBotSetting
    {
        public string ClientID { get; set; }

        public string ClientSecret { get; set; }

        public string AuthorizeApi { get; set; }

        public string TokenApi { get; set; }

        public string NotifyApi { get; set; }

        public string StatusApi { get; set; }

        public string RevokeApi { get; set; }
    }
}
