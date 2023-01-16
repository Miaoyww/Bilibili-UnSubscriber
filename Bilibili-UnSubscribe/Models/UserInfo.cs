using System.Diagnostics;
using Bilibili_UnSubscribe.Helpers;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using RestSharp;

namespace Bilibili_UnSubscribe.Models;

public class UserInfo
{
    public Brush Face;
    public string FaceUrl;
    public bool IsLoaded;
    public bool IsOk; // 用户是否存在
    public string Name;
    public string Sign;
    public List<string> Tags = new();
    public string Uid;
    public List<Video> Videos = new();

    public UserInfo(string uid)
    {
        Uid = uid;
    }

    public async Task GetInfoAsync()
    {
        var userInfo =
            await HttpRequestHelper.JObjectRequest($"https://api.bilibili.com/x/space/acc/info?mid={Uid}", Method.GET);
        if (int.Parse(userInfo["code"].ToString()) == 0)
        {
            IsOk = true;
            Name = userInfo["data"]["name"].ToString();
            FaceUrl = userInfo["data"]["face"] + "@200w_200h_1c_90q.webp";
            Sign = userInfo["data"]["sign"].ToString();
        }

        IsLoaded = true;
    }

    public async Task GetDetails()
    {
        Face = new ImageBrush
        {
            ImageSource = new BitmapImage(new Uri(FaceUrl))
        };
        var userVideos =
            await HttpRequestHelper.JObjectRequest($"http://api.bilibili.com/x/space/arc/search?mid={Uid}", Method.GET);
        var tagList = userVideos["data"]["list"]["tlist"];
        if (tagList != null)
        {
            foreach (var tagItem in userVideos["data"]["list"]["tlist"])
            {
                Tags.Append(tagItem.ToString());
            }
        }

        var videoList = userVideos["data"]["list"]["vlist"];
        if (videoList != null)
        {
            foreach (var videoItem in videoList)
            {
                if (Videos.Count < 4)
                {
                    Videos.Add(new Video
                    {
                        Title = videoItem["title"].ToString(),
                        Cover = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(videoItem["pic"] + "@240w_150h_1c_90q.webp"))
                        }
                    });
                }
            }
        }
    }

    public async Task UnSubscribe()
    {
        var api = "https://api.bilibili.com/x/relation/modify";
        var client = new RestClient();
        var request = new RestRequest(api, Method.POST);

        foreach (var cookieItem in HttpRequestHelper.cookies)
        {
            request.AddCookie(cookieItem.Name, cookieItem.Value);
        }

        request.AddParameter("SESSDATA", AccountHelper.account.Sessdata, ParameterType.Cookie);
        request.AddHeader("Referer", "https://www.bilibili.com");
        request.AddHeader("User-Agent", "Mozilla/5.0");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("csrf", AccountHelper.account.biliJct);
        request.AddParameter("csrf_token", AccountHelper.account.biliJct);
        request.AddParameter("fid", Uid);
        request.AddParameter("act", 2);
        request.AddParameter("re_src", 11);
        client.UserAgent = "Mozilla/5.0";
        var resp = (await client.ExecuteAsync(request)).Content;
        Debug.Write("");
    }
}