using Bilibili_UnSubscribe.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Bilibili_UnSubscribe.Views.Pages;

public sealed partial class LoginPage : Page
{
    public TextBox BiliJctTextBox;
    public TextBox SessdataTextBox;

    public LoginPage()
    {
        ViewModel = App.GetService<LoginViewModel>();
        InitializeComponent();
        BiliJctTextBox = biliJctTextBox;
        SessdataTextBox = sessdataTextBox;
    }

    public LoginViewModel ViewModel
    {
        get;
    }
}