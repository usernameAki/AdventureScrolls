using System;
using System.Collections.Generic;
using System.Text;

namespace AdventureScrolls.Model
{
    public class ScrollModel
    {
        public DateTime EntryDate { get; set; }
        public string Title { get; set; }
        public string ScrollContent { get; set; }
        public string Mood { get; set; }

        public ScrollModel()
        {
            EntryDate = DateTime.Now;
            Title = "";
            ScrollContent = "";
            Mood = "happy";
        }
    }
}
