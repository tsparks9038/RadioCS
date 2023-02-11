using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RadioCS
{
    internal class SongInfo
    {
        public string GetSongName()
        {
            HttpClient client = new();
            string result;

            result = client.GetStringAsync("https://r-a-d.io/api").Result;

            JsonDocument json = JsonDocument.Parse(result);
            JsonElement root = json.RootElement;
            JsonElement main = root.GetProperty("main");
            JsonElement np = main.GetProperty("np");

            return np.GetString();
        }

        public long GetStartTime()
        {
            HttpClient client = new();
            string result;

            result = client.GetStringAsync("https://r-a-d.io/api").Result;

            JsonDocument json = JsonDocument.Parse(result);
            JsonElement root = json.RootElement;
            JsonElement main = root.GetProperty("main");
            JsonElement start_time = main.GetProperty("start_time");

            return start_time.GetInt64();
        }

        public long GetEndTime()
        {
            HttpClient client = new();
            string result;

            result = client.GetStringAsync("https://r-a-d.io/api").Result;

            JsonDocument json = JsonDocument.Parse(result);
            JsonElement root = json.RootElement;
            JsonElement main = root.GetProperty("main");
            JsonElement end_time = main.GetProperty("end_time");

            return end_time.GetInt64();
        }

        public long GetCurrent()
        {
            HttpClient client = new();
            string result;

            result = client.GetStringAsync("https://r-a-d.io/api").Result;

            JsonDocument json = JsonDocument.Parse(result);
            JsonElement root = json.RootElement;
            JsonElement main = root.GetProperty("main");
            JsonElement current = main.GetProperty("current");

            return current.GetInt64();
        }
    }
}
