using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Beursfuif.Server.Messages
{
    public class DialogMessage
    {
        public string Title { get; set; }
        public List<string> Errors { get; set; }
        public Visibility Nay { get; set; }
        public event EventHandler<AnswerChangedArgs> AnswerChanged;
        private bool _answer;
        public bool Answer
        {
            get { return _answer; }
            set
            {
                if (_answer != value)
                {
                    _answer = value;
                    if (AnswerChanged != null)
                    {
                        AnswerChanged(this, new AnswerChangedArgs(value));
                    }
                }
            }
        }


        public DialogMessage()
        {
            Errors = new List<string>();
        }

        public DialogMessage(string title)
        {
            Title = title;
            Errors = new List<string>();
        }

    }

    public class AnswerChangedArgs:EventArgs
    {
        public bool Value { get; set; }

        public AnswerChangedArgs(bool value)
        {
            Value = value;
        }
    }
}
