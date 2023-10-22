using AdventureScrolls.Model;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace AdventureScrolls.Services
{
    public class ScribeService : IScribeService
    {
        public string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScrollLibrary.json");

        private void StoreScrolls(ObservableCollection<ScrollModel> scrolls)
        {
            string json = JsonConvert.SerializeObject(scrolls, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public ObservableCollection<ScrollModel> GetScrolls()
        {
            if (!File.Exists(filePath))
            {
                CreateEmptyScrollLibrary();
            }
            string json = File.ReadAllText(filePath);
            ObservableCollection<ScrollModel> loadedScrolls = JsonConvert.DeserializeObject<ObservableCollection<ScrollModel>>(json);
            return loadedScrolls;
        }

        private void CreateEmptyScrollLibrary()
        {
            ObservableCollection<ScrollModel> emptyLibrary = new ObservableCollection<ScrollModel>();
            StoreScrolls(emptyLibrary);
        }

        public void StoreNewScroll(ScrollModel scrollToStore)
        {
            ObservableCollection<ScrollModel> scrollLibrary = GetScrolls();
            scrollLibrary.Add(scrollToStore);
            StoreScrolls(scrollLibrary);
            scrollToStore.CleanData();
        }
    }
}
