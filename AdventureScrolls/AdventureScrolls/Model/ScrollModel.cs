using AdventureScrolls.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventureScrolls.Model
{
    public class ScrollModel : ObservableObjects
    {
        private DateTime _entryDate;
        public DateTime EntryDate 
        { 
            get => _entryDate;
            set {
                    _entryDate = value;
                    OnPropertyChanged();
                }
        }
        private string _title;
        public string Title 
        {
            get => _title; 
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private string _scrollContent;
        public string ScrollContent 
        {
            get => _scrollContent;
            set
            {
                _scrollContent = value;
                OnPropertyChanged();
            }
        }
        private string _mood;
        public string Mood 
        {
            get => _mood; 
            set
            {
                _mood = value;
                OnPropertyChanged();
            }
        }

        public ScrollModel()
        {
            CleanData();
        }

        public void CleanData()
        {
            EntryDate = DateTime.Now;
            Title = "";
            ScrollContent = "";
            Mood = "happy";
        }
    }
}
