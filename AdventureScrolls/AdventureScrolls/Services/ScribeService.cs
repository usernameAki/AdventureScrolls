using AdventureScrolls.Core;
using AdventureScrolls.Model;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AdventureScrolls.Services
{
    public class ScribeService : IScribeService
    {
        public string filePath;
        public ObservableCollection<ScrollModel> ScrollLibrary { get; set; }

        public ScribeService()
        {
            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScrollLibrary.json");
            //File.Delete(filePath); //DELETE DATA - for test purpose
            ScrollLibrary = GetScrolls();
        }
        private void StoreScrolls(ObservableCollection<ScrollModel> scrollLibraryToStore)
        {
            string json = JsonConvert.SerializeObject(scrollLibraryToStore, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public ObservableCollection<ScrollModel> GetScrolls()
        {
            if (!File.Exists(filePath))
            {
                CreateEmptyScrollLibrary();
            }
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<ObservableCollection<ScrollModel>>(json);
        }

        private void CreateEmptyScrollLibrary()
        {
            ScrollLibrary = new ObservableCollection<ScrollModel>();
            StoreScrolls(ScrollLibrary);
        }

        public void StoreNewScroll(ScrollModel scrollToStore)
        {
            ScrollLibrary.Add(new ScrollModel(scrollToStore));
            StoreScrolls(ScrollLibrary);
        }
        public async void RemoveScroll(object scrollToDelete)
        {
            try
            {
                int index = ScrollLibrary.IndexOf(scrollToDelete);
                ScrollLibrary.RemoveAt(index);
                StoreScrolls(ScrollLibrary);
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("error", "Scroll could not be removed. Exception: " + e.ToString(), "ok" );
            }
        }
    }
}
