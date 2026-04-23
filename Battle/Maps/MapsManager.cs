using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace tser.Battle.Maps
{
    internal class MapsManager
    {
        private bool mapsLoaded = false;

        private Dictionary<string, AvaRoadEntry> roads = new Dictionary<string, AvaRoadEntry>();
        private Dictionary<string, Map> maps = new Dictionary<string, Map>();

        public async Task Load()
        {
            if (mapsLoaded)
                return;

            await LoadMaps();
            await LoadRoads();

            mapsLoaded = true;
        }

        public async Task LoadRoads()
        {
            var json = await File.ReadAllTextAsync("assets/mapList.json");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var list = JsonSerializer.Deserialize<List<AvaRoadEntry>>(json, options);

            roads = list.ToDictionary(x => x.Name, x => x);
        }


        public async Task LoadMaps()
        {
            var json = await File.ReadAllTextAsync("assets/albionLocations.json");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var list = JsonSerializer.Deserialize<List<Map>>(json, options);

            maps = list.Where(x => x.MapCategory == "openworld").ToDictionary(x => x.DisplayName, x => x);
        }


        public AvaRoadEntry SearchRoad(string searchName)
        {
            if (!roads.ContainsKey(searchName))
                return null;

            return roads[searchName];
        }

        public Map SearchMap(string searchName)
        {
            if (!maps.ContainsKey(searchName))
                return null;

            return maps[searchName];
        }
    }
}
