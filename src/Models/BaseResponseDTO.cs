using System;
using System.Collections.Generic;
using System.Text;

namespace LexLibrary.Line.NotifyBot.Models
{
    public class BaseResponseDTO
    {
        public Dictionary<string, IEnumerable<string>> Headers { get; set; }

        public int Status { get; set; }

        public string Message { get; set; }
    }
}
