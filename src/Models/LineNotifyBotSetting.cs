using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexLibrary.Line.NotifyBot.Models
{
    /// <summary>
    /// LineNotifyBotSetting
    /// </summary>
    public class LineNotifyBotSetting
    {
        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// ClientSecret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// AuthorizeApi 位址
        /// </summary>
        public string AuthorizeApi { get; set; }

        /// <summary>
        /// TokenApi 位址
        /// </summary>
        public string TokenApi { get; set; }

        /// <summary>
        /// NotifyApi 位址
        /// </summary>
        public string NotifyApi { get; set; }

        /// <summary>
        /// StatusApi 位址
        /// </summary>
        public string StatusApi { get; set; }

        /// <summary>
        /// RevokeApi 位址
        /// </summary>
        public string RevokeApi { get; set; }
    }
}
