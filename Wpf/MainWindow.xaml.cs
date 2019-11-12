// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// contributed by Ben Zuill-Smith (https://github.com/bzuillsmith)
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.OidcClient;
using System;
using System.Net;
using System.Windows;
using WpfSample.Auth;

namespace WpfSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OidcClient _oidcClient = null;
        private LoginResult _result;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Start;
        }

        public async void Start(object sender, RoutedEventArgs e)
        {
            var options = new OidcClientOptions()
            {
                Authority = "https://demo.identityserver.io/",
                ClientId = "native.code",
                Scope = "openid profile email",
                RedirectUri = "http://127.0.0.1/sample-wpf-app",
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                Browser = new WpfEmbeddedBrowser()
            };

            _oidcClient = new OidcClient(options);

            LoginResult result;
            try
            {
                result = await _oidcClient.LoginAsync();
            }
            catch (Exception ex)
            {
                Message.Text = $"Unexpected Error: {ex.Message}";
                return;
            }

            if (result.IsError)
            {
                Message.Text = result.Error == "UserCancel" ? "The sign-in window was closed before authorization was completed." : result.Error;
            }
            else
            {
                var name = result.User.Identity.Name;
                _result = result;
                Message.Text = $"Hello {name}";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var browse = new WpfEmbeddedBrowserToken();
            //browse.Browse("https://localhost:44312/", _result.AccessToken);
            var request = HttpWebRequest.Create("https://localhost:44312/");
            request.Headers.Add("Authorization", "Bearer " +_result.AccessToken);
            request.Headers.Add("Cookie", "access_token="+_result.AccessToken);
            var res = request.GetResponse();
            var stream = res.GetResponseStream();
        }
    }
}
