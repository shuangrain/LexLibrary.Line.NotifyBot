using System;
using System.Collections.Generic;
using System.Text;

namespace LexLibrary.Line.NotifyBot.Models
{
    public class StatusResponseDTO : BaseResponseDTO
    {
        public string TargetType { get; set; }

        public string Target { get; set; }
    }


}
