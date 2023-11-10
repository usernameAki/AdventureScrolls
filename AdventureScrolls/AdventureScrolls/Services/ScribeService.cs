using AdventureScrolls.Core;
using AdventureScrolls.Model;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AdventureScrolls.Services
{
    public class ScribeService : IScribeService
    {
        public string filePath;
        public ObservableCollection<ScrollModel> ScrollLibrary {get; set;}

        public ScribeService()
        {
            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScrollLibrary.json");
            ScrollLibrary = new ObservableCollection<ScrollModel>();
            GetScrolls();
        }

        public void GetScrolls()
        {
            if (!File.Exists(filePath))
            {
                CreateEmptyScrollLibrary();
            }
            string json = File.ReadAllText(filePath);
            ObservableCollection<ScrollModel> temp = JsonConvert.DeserializeObject<ObservableCollection<ScrollModel>>(json);
            temp = new ObservableCollection<ScrollModel>(temp.OrderByDescending(x => x.EntryDate));
            ScrollLibrary.Clear();
            foreach(ScrollModel scroll in temp)
            {
                ScrollLibrary.Add(scroll);
            }
        }
        private void CreateEmptyScrollLibrary()
        {
            StoreScrolls(ScrollLibrary);
        }
        private void StoreScrolls(ObservableCollection<ScrollModel> scrollLibraryToStore)
        {
            string json = JsonConvert.SerializeObject(scrollLibraryToStore, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public void StoreNewScroll(ScrollModel scrollToStore)
        {
            ScrollLibrary.Add(new ScrollModel(scrollToStore));
            StoreScrolls(ScrollLibrary);
            GetScrolls();
        }
        public void OverrideScroll()
        {
            StoreScrolls(ScrollLibrary);
            GetScrolls();
        }
        public void RemoveScroll(object scrollToDelete)
        {
            try
            {
                int index = ScrollLibrary.IndexOf(scrollToDelete);
                ScrollLibrary.RemoveAt(index);
                StoreScrolls(ScrollLibrary);
                GetScrolls();
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("error " + ScrollLibrary.IndexOf(scrollToDelete), 
                    "Scroll could not be removed. Exception: " + e.ToString(), "ok" );
            }
        }
    }
}
