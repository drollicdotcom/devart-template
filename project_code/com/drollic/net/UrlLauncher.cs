using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;

namespace com.drollic.net
{
    public sealed class UrlLauncher
    {
        public static void ShowInBrowser(String url)
        {            
            // Tell the system to open this page using the default browser
            System.Diagnostics.Process.Start(url);
        }
    }
}
