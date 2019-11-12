// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// contributed by Ben Zuill-Smith (https://github.com/bzuillsmith)
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.OidcClient.Browser;
using mshtml;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfSample.Auth
{
    public class WpfEmbeddedBrowserToken
    {

        public WpfEmbeddedBrowserToken()
        {

        }

        public async Task Browse(string url, string token)
        {

            var window = new Window()
            {
                Width=900,
                Height=625,
                Title = "IdentityServer Website"
            };

            // Note: Unfortunately, WebBrowser is very limited and does not give sufficient information for 
            //   robust error handling. The alternative is to use a system browser or third party embedded
            //   library (which tend to balloon the size of your application and are complicated).
            var webBrowser = new WebBrowser();

            var signal = new SemaphoreSlim(0, 1);

            var result = new BrowserResult()
            {
                ResultType = BrowserResultType.UserCancel
            };

            webBrowser.Navigating += (s, e) =>
            {
                
            };

            window.Closing += (s, e) =>
            {
                signal.Release();
            };
            window.Content = webBrowser;
            window.Show();
            webBrowser.Source = new Uri(url + "?access_token=" + token);

            await signal.WaitAsync();
            
        }
       
    }
}
