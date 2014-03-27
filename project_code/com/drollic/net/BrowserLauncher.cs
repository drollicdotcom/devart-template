using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;

namespace com.drollic.net
{
    public sealed class BrowserLauncher
    {
        public static void ShowInBrowser(String url)
        {            
            // launch web browser
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = url;
            Process.Start(startInfo);
        }
    }
}
