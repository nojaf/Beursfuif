using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.Messages
{
    public class ErrorMessage
    {
        public string Title { get; set; }
        public List<string> Errors { get; set; }

        public ErrorMessage()
        {

        }

        public ErrorMessage(string title)
        {
            Title = title;
            Errors = new List<string>();
        }
    }
}
