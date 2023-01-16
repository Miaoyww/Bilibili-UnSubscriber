using Bilibili_UnSubscribe.Helpers;
using Bilibili_UnSubscribe.Models;
using Bilibili_UnSubscribe.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Bilibili_UnSubscribe.Views.Pages;

public sealed partial class MainPage : Page
{
    public Account Account;
    public VariableSizedWrapGrid VideoGrid;

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        Account = AccountHelper.account;
        VideoGrid = VideoList;
    }

    public MainViewModel ViewModel
    {
        get;
    }
}