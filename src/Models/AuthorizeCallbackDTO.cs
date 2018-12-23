using System;
using System.Collections.Generic;
using System.Text;

namespace LexLibrary.Line.NotifyBot.Models
{
    public class AuthorizeCallbackDTO
    {
        public string Code { get; set; }

        public string State { get; set; }

        public string Error { get; set; }

        public string Error_Description { get; set; }
    }
}
