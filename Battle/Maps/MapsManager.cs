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
        private Dictionary<string, string> mapNames = new Dictionary<string, string>();
        private string[] excludeType = new[] { "arena", "expedition" };
        private string[] includeType = new[] { "openworld" };

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

            roads = list.ToDictionary(x => Normalize(x.Name), x => x);
        }


        public async Task LoadMaps()
        {
            var json = await File.ReadAllTextAsync("assets/albionLocations.json");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var list = JsonSerializer.Deserialize<List<Map>>(json, options);

            maps = list
                .Where(x => !excludeType.Contains(x.MapCategory))
                .ToDictionary(x => x.Id, x => x);
            mapNames = list
                .DistinctBy(x => x.DisplayName)
                .Where(x => !excludeType.Contains(x.MapCategory))
                .ToDictionary(x => Normalize(x.DisplayName), x => x.Id);
        }

        private string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            return s
                .ToLowerInvariant()
                .Replace('l', 'i'); // считаем l и i одинаковыми
        }


        public AvaRoadEntry SearchRoad(string searchName)
        {
            searchName = Normalize(searchName);

            if (!roads.ContainsKey(searchName))
                return null;

            return roads[searchName];
        }

        public Map SearchMap(string searchName)
        {
            searchName = Normalize(searchName);

            if (!mapNames.ContainsKey(searchName))
                return null;

            return maps[mapNames[searchName]];
        }

        public List<InterestedLocation> FindInterestedLocations(Map start)
        {
            var result = new List<InterestedLocation>();
            var visited = new HashSet<string>();

            var queue = new Queue<(string id, int depth)>();
            queue.Enqueue((start.Id, 0));
            visited.Add(start.Id);

            while (queue.Count > 0)
            {
                var (id, depth) = queue.Dequeue();

                if (depth > 5)
                    continue;

                var map = maps[id];

                var interest = GetInterest(map);
                if (interest != null && depth > 0)
                {
                    result.Add(new InterestedLocation
                    {
                        Depth = depth,
                        Map = map,
                        Description = interest.Value.description
                    });
                    continue;
                }

                foreach (var nextId in map.Neighbours)
                {
                    if (visited.Contains(nextId))
                        continue;

                    visited.Add(nextId);
                    queue.Enqueue((nextId, depth + 1));
                }
            }

            return result;
        }

        enum InterestType
        {
            City,
            Rest,
            Portal,
            DungeonGroup,
            DungeonHighTier,
            Custom
        }

        private Dictionary<string, string> customLocations = new Dictionary<string, string>() { { "Sandmount Strand", "Hideout" } };

        (string description, InterestType type)? GetInterest(Map map)
        {
            //if (map.PvpCategory != "black")
            //    return null;

            if (map.MapCategory == "portalcity")
                return ("Город", InterestType.Portal);

            if (map.MapCategory == "city")
                return ("Город", InterestType.City);

            if (map.MapCategory == "rest")
                return ("Rest", InterestType.Rest);

            if (map.MapCategory == "dungeon")
            {
                // группа данжей
                if (map.Id == "DNG-MOR-01-MAIN-04" ||
                    map.Id == "DNG-MOR-01-MAIN-09")
                {
                    return ("Треугольник", InterestType.DungeonGroup);
                }

                // high tier
                if ((map.Tier == "8" || map.Tier == "7") &&
                    (map.Quality == "5" || map.Quality == "6"))
                {
                    return ($"Статик {map.Tier}.{map.Quality}", InterestType.DungeonHighTier);
                }
            }

            if (customLocations.ContainsKey(map.DisplayName))
                return (customLocations[map.DisplayName], InterestType.Custom);

            return null;
        }
    }
}
