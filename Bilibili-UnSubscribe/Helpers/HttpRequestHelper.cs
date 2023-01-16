using Newtonsoft.Json.Linq;
using RestSharp;

namespace Bilibili_UnSubscribe.Helpers;

public static class HttpRequestHelper
{
    public static IList<RestResponseCookie> cookies;

    public static async Task<List<object>> GetHead(string url, Method way)
    {
        var client = new RestClient();
        if (cookies == null)
        {
            var cookieRequest = new RestRequest("https://www.bilibili.com", Method.GET);
            cookies = (await client.ExecuteAsync(cookieRequest)).Cookies;
        }

        var request = new RestRequest(url, way);
        foreach (var cookieItem in cookies)
        {
            request.AddCookie(cookieItem.Name, cookieItem.Value);
        }

        if (AccountHelper.account.IsLogined)
        {
            request.AddCookie("SESSDATA", AccountHelper.account.Sessdata);
            request.AddCookie("csrf", AccountHelper.account.biliJct);
            request.AddCookie("csrf_token", AccountHelper.account.biliJct);
        }

        request.AddHeader("Referrer", "https://www.bilibili.com");
        request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
        return new List<object> {client, request};
    }

    public static async Task<string> StringRequest(string url, Method way)
    {
        var header = await GetHead(url, way);
        return (await ((RestClient)header[0]).ExecuteAsync((RestRequest)header[1])).Content;
    }

    public static async Task<JObject> JObjectRequest(string url, Method way) =>
        JObject.Parse(await StringRequest(url, way));
}