using System.ComponentModel;
using Bilibili_UnSubscribe.Helpers;
using Bilibili_UnSubscribe.Models;
using Bilibili_UnSubscribe.Views.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Bilibili_UnSubscribe.ViewModels;

public class LoginViewModel : ObservableRecipient, INotifyPropertyChanged
{
    private LoginPage _loginPage;

    private string _loginSign;

    public string LoginSign
    {
        set
        {
            _loginSign = value;
            OnPropertyChanged(nameof(LoginSign));
        }
        get => _loginSign;
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    public void LoginPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        _loginPage = (LoginPage)sender;
        if (AccountHelper.account.IsLogined)
        {
            LoginSign = $"您已登陆成功! 你好, {AccountHelper.account.Name}";
        }
        
    }

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void ButtonBase_OnClick(object sender, RoutedEventArgs e) =>
        LoginIn(_loginPage.BiliJctTextBox.Text, _loginPage.SessdataTextBox.Text);

    public async void LoginIn(string biliJct, string sessdata)
    {
        if (string.IsNullOrEmpty(biliJct) || string.IsNullOrEmpty(sessdata))
        {
            if (AccountHelper.account.IsLogined)
            {
                await FollowingsHelper.ReGetFollowings();
            }
            return;
        }
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
            AccountHelper.account.Name = respResult["data"]["name"].ToString();
            AccountHelper.account.Sessdata = sessdata;
            AccountHelper.account.biliJct = biliJct;
            AccountHelper.account.Uid = (int)respResult["data"]["mid"];
            await FollowingsHelper.ReGetFollowings();
        }

        LoginSign = $"您已登陆成功! 你好, {AccountHelper.account.Name}";
    }
}