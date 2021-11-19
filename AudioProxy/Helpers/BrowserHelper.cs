using AudioProxy.Options;
using System.Diagnostics;

namespace AudioProxy.Helpers
{
    public static class BrowserHelper
    {
        public static void RunAudioProxyInBrowser(bool firstLaunch, int port)
{
            var browserStartInfo = new ProcessStartInfo(firstLaunch ? $"http://localhost:{port}/About" : $"http://localhost:{port}")
            {
                UseShellExecute = true
            };
            Process.Start(browserStartInfo);
        }
    }
}
