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

        public ScrollModel(string title, string scrollContext)
        {
            EntryDate = DateTime.Now;
            Title = title;
            ScrollContent = scrollContext;
        }
    }
}
