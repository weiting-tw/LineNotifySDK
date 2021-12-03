# LineNotifySDK

[![Build and deploy](https://github.com/weiting-tw/LineNotifySDK/actions/workflows/lineNotifyDemo.yml/badge.svg?branch=master)](https://github.com/weiting-tw/LineNotifySDK/actions/workflows/lineNotifyDemo.yml)

## Info

[Nuget Package](https://www.nuget.org/packages/LineNotifySDK/)

[Demo Site](https://LineNotifyDemo.azurewebsites.net/)

## Register Line Notify Services

[Line Notify Document](https://notify-bot.line.me/doc/en/)

1. Register Services <https://notify-bot.line.me/my/services/new>
2. Add url to `Callback URL`. like `https://localhost:44352/home/BindCallback`
3. Copy `Client ID` ,`Client Secret` and `Callback URL` to `appsettings.json`

## Usage

Add to [Startup.cs](https://github.com/weiting-tw/LineNotifySDK/blob/master/LineNotifySample/Startup.cs)

```cs
 services.AddLineNotifyServices((_, options) =>
{
    options.ClientId = Configuration.GetValue("LineNotifyOptions:ClientId", "CHANGE_ME");
    options.ClientSecret = Configuration.GetValue("LineNotifyOptions:ClientSecret", "CHANGE_ME");
    options.RedirectUri = Configuration.GetValue("LineNotifyOptions:RedirectUri", "CHANGE_ME");
});
```

Set authorize and callback with [Controller](https://github.com/weiting-tw/LineNotifySDK/blob/master/LineNotifySample/Controllers/HomeController.cs)
