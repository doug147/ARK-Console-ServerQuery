using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Diagnostics;

static class ARKServerQuery
{
    public enum Platform
    {
        XBOX,
        PS4
    }
    private static Dictionary<Platform, JArray> servers = new Dictionary<Platform, JArray>
    {
        { Platform.XBOX, new JArray() },
        { Platform.PS4, new JArray() }
    };
    private static Dictionary<Platform, Stopwatch> stopwatch = new Dictionary<Platform, Stopwatch>
    {
        { Platform.XBOX, new Stopwatch() },
        { Platform.PS4, new Stopwatch() }
    };
    private static Dictionary<Platform, string> serverURL = new Dictionary<Platform, string>
    {
        { Platform.XBOX, "http://arkdedicated.com/xbox/cache/officialserverlist.json" },
        { Platform.PS4, "http://arkdedicated.com/sotfps4/cache/officialserverlist.json" }
    };
    public static void GetServerList(Platform platform = Platform.XBOX)
    {
        try
        {
            WebClient wc = new WebClient();
            byte[] raw = wc.DownloadData(serverURL[platform]);
            string data = System.Text.Encoding.UTF8.GetString(raw);
            servers[platform] = JArray.Parse(data);
        }
        catch { }
    }
    public static JToken FindServer(string strQuery, Platform platform = Platform.XBOX)
    {
        if (servers[platform].Count <= 0 || stopwatch[platform].ElapsedMilliseconds > 5000)
        {
            GetServerList(platform);
            stopwatch[platform].Restart();
        }
        if (servers.Count > 0)
        {
            try
            {
                var result = servers[platform].AsEnumerable<JToken>().Where(obj => obj["Name"].ToObject<string>().Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0].ToLower().EndsWith(strQuery.ToLower()) && obj["SessionIsPve"].ToObject<int>() == 0);
                return result.Count<JToken>() > 0 ? result.First() : null;
            }
            catch { }
        }
        return null;
    }
    public static string GetValue(JToken token, string field)
    {
        return token == null ? string.Empty : token[field] == null ? string.Empty : token[field].ToString();
    }
}
