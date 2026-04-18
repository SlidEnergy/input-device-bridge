using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tser
{
    internal class RegionManager
    {
        //private Dictionary<string, string> screenshots = new Dictionary<string, string>();

        Dictionary<string, Screenshot> data = new();

        private string filePath = "data.json";

        public void InitRegions()
        {
            if (!File.Exists(filePath))
                return;

            var json = File.ReadAllText(filePath);
            var list = JsonSerializer.Deserialize<List<Screenshot>>(json);

            data = list.ToDictionary(x => x.Name, x => x);
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(data.Values.ToList(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }

        public Dictionary<string, Region> GetScreenshotRegions(string screenshotName)
        {
            if (!data.ContainsKey(screenshotName))
                return new();

            return data[screenshotName].Regions
                   .ToDictionary(x => x.Name, x => x);
        }

        public string[] GetScreenshots()
        {
            return data.Keys.ToArray();
        }

        public Image GetScreenshot(string name)
        {
            if (!data.ContainsKey(name))
                throw new Exception("Screenshot not found");

            return Image.FromFile(data[name].FilePath);
        }

        public void AddScreenshot(string name, Image image)
        {
            string path = Path.Combine("screens", name + ".png");

            Directory.CreateDirectory("screens");
            image.Save(path);

            data[name] = new Screenshot
            {
                Name = name,
                FilePath = path
            };

            Save();
        }

        public void AddRegion(string screenshot, Region region)
        {
            if (!data.ContainsKey(screenshot))
                throw new Exception("Screenshot not found");

            var list = data[screenshot].Regions;

            var existing = list.FirstOrDefault(x => x.Name == region.Name);
            if (existing != null)
                existing.Rect = region.Rect;
            else
                list.Add(region);

            Save();
        }

        public void DeleteRegion(string screenshot, string region)
        {
            if (!data.ContainsKey(screenshot))
                throw new Exception("Screenshot not found");

            var list = data[screenshot].Regions;

            var item = list.FirstOrDefault(x => x.Name == region);
            if (item == null)
                return; // или throw, если хочешь строго

            list.Remove(item);
        }

        public Rectangle GetRect(string screenshot, string region)
        {
            if (!data.ContainsKey(screenshot))
                throw new Exception("Screenshot not found");

            var item = data[screenshot].Regions
                .FirstOrDefault(x => x.Name == region);

            if (item == null)
                throw new Exception("Region not found");

            return item.Rect;
        }
    }
}
