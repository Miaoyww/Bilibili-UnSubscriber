using Bilibili_UnSubscribe.Helpers;
using Bilibili_UnSubscribe.Models;
using Bilibili_UnSubscribe.Views.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Bilibili_UnSubscribe.ViewModels;

public class LoginViewModel : ObservableRecipient
{
    public LoginPage _loginPage;

    public void LoginPage_OnLoaded(object sender, RoutedEventArgs e) => _loginPage = (LoginPage)sender;

    public void ButtonBase_OnClick(object sender, RoutedEventArgs e) =>
        LoginIn(_loginPage.BiliJctTextBox.Text, _loginPage.SessdataTextBox.Text);

    public async void LoginIn(string biliJct, string sessdata)
    {
        var client = new RestClient();
        var request = new RestRequest("https://www.bilibili.com", Method.GET);
        var cookies = (await client.ExecuteAsync(request)).Cookies;
        request = new RestRequest("https://api.bilibili.com/x/space/myinfo", Method.GET);

        foreach (var cookieItem in cookies)
        {
            request.AddCookie(cookieItem.Name, cookieItem.Value);
        }

        request.AddCookie("SESSDATA", sessdata);
        request.AddCookie("csrf", biliJct);
        request.AddCookie("csrf_token", biliJct);
        request.AddHeader("Referrer", "https://www.bilibili.com");
        request.AddHeader("User-Agent", "Mozilla/5.0");
        var respResult = JObject.Parse((await client.ExecuteAsync(request)).Content);
        if (int.Parse(respResult["code"].ToString()) == 0)
        {
            AccountHelper.account.IsLogined = true;
            AccountHelper.account.Sessdata = sessdata;
            AccountHelper.account.biliJct = biliJct;
            AccountHelper.account.Uid = (int)respResult["data"]["mid"];
            var attentions =
                await HttpRequestHelper.JObjectRequest(
                    $"https://account.bilibili.com/api/member/getCardByMid?mid={AccountHelper.account.Uid}",
                    Method.GET);
            foreach (var uidItem in attentions["card"]["attentions"])
            {
                FollowingsHelper.followings.Add(new UserInfo(uidItem.ToString()));
            }
        }
    }
}