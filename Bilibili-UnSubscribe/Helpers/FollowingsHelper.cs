using Bilibili_UnSubscribe.Models;
using RestSharp;

namespace Bilibili_UnSubscribe.Helpers;

public static class FollowingsHelper
{
    public static List<UserInfo> followings = new();

    public static async Task ReGetFollowings()
    {
        followings.Clear();
        var attentions =
            await HttpRequestHelper.JObjectRequest(
                $"https://account.bilibili.com/api/member/getCardByMid?mid={AccountHelper.account.Uid}",
                Method.GET);
        foreach (var uidItem in attentions["card"]["attentions"])
        {
            followings.Add(new UserInfo(uidItem.ToString()));
        }
    }
}