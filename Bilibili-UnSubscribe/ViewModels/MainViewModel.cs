using System.ComponentModel;
using System.Windows.Input;
using Windows.UI;
using Bilibili_UnSubscribe.Helpers;
using Bilibili_UnSubscribe.Models;
using Bilibili_UnSubscribe.Views.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Bilibili_UnSubscribe.ViewModels;

public class MainViewModel : ObservableRecipient, INotifyPropertyChanged
{
    private UserInfo _currentUser;
    private Brush _face = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));
    private MainPage _mainPage;

    private string _name = "未知";
    private string _sign = "未知";
    private string _tags = "未知";

    public ICommand NextUserCommand;
    public ICommand UnSubscribeCommand;

    public MainViewModel()
    {
        NextUserCommand = new RelayCommand(NextUser);
        UnSubscribeCommand = new RelayCommand(UnSubscribe);
    }

    public UserInfo CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            Name = _currentUser.Name;
            Sign = _currentUser.Sign;
            Tags = string.Join(" - ", _currentUser.Tags);
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string Tags
    {
        get => _tags;
        set
        {
            _tags = value;
            OnPropertyChanged(nameof(Tags));
        }
    }

    public Brush Face
    {
        get => _face;
        set
        {
            _face = value;
            OnPropertyChanged(nameof(Face));
        }
    }

    public string Sign
    {
        get => _sign;
        set
        {
            _sign = value;
            OnPropertyChanged(nameof(Sign));
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    public async void PageLoaded(object sender, RoutedEventArgs e) => _mainPage = (MainPage)sender;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public async void NextUser()
    {
        if (AccountHelper.account.IsLogined)
        {
            if (FollowingsHelper.followings.Count > 1)
            {
                await FollowingsHelper.followings[0].GetInfoAsync();

                // 自动取消关注*已注销的*用户
                if (!FollowingsHelper.followings[0].IsOk)
                {
                    if (!FollowingsHelper.followings[0].IsLoaded)
                    {
                        await FollowingsHelper.followings[0].UnSubscribe();
                        FollowingsHelper.followings.RemoveAt(0);
                        GC.Collect();
                        NextUser();
                        return;
                    }
                }
                else
                {
                    CurrentUser = FollowingsHelper.followings[0];
                    await CurrentUser.GetDetails();
                    Face = CurrentUser.Face;
                    _mainPage.VideoGrid.Children.Clear();
                    foreach (var videoItem in CurrentUser.Videos)
                    {
                        _mainPage.VideoGrid.Children.Add(await getGrid(videoItem));
                    }
                }

                FollowingsHelper.followings[0].Videos.Clear();
                FollowingsHelper.followings.RemoveAt(0);
                GC.Collect();
            }
            else
            {
                Tags = "已完成浏览关注列表！";
                Name = "已完成浏览关注列表！";
                Sign = "已完成浏览关注列表！";
            }
        }
    }

    private async Task<Grid> getGrid(Video videoItem)
    {
        var grid = new Grid
        {
            Margin = new Thickness(0, 0, 8, 0)
        };
        var border = new Border
        {
            Background = videoItem.Cover,
            Width = 240,
            Height = 150,
            CornerRadius = new CornerRadius(10),
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 90)
        };
        var textBlock = new TextBlock
        {
            Text = videoItem.Title,
            FontFamily = new FontFamily("HarmonyOS Sans SC"),
            FontSize = 17,
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Left,
            Width = 240,
            TextAlignment = TextAlignment.Left,

            TextWrapping = TextWrapping.Wrap,
            Padding = new Thickness(0)
        };
        grid.Children.Add(border);
        grid.Children.Add(textBlock);
        return grid;
    }

    public async void UnSubscribe()
    {
        if (AccountHelper.account.IsLogined)
        {
            await CurrentUser.UnSubscribe();
            NextUser();
        }
    }
}