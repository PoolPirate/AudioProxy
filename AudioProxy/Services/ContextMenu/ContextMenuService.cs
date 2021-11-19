using AudioProxy.Helpers;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioProxy.Services
{
    public sealed class ContextMenuService : Service
    {
        [Inject]
        private readonly IConfiguration Config;
        [Inject]
        private readonly AudioProxyContext ApplicationContext;

        [Inject]
        private readonly IHost Host;
        [Inject]
        private readonly IHostApplicationLifetime HostLifetime;

        private readonly Container Components;
        private readonly ContextMenuStrip ContextMenu;

        private readonly ToolStripMenuItem OpenBrowserItem;
        private readonly ToolStripMenuItem CloseAppItem;

        private NotifyIcon TrayIcon { get; set; }

        public ContextMenuService()
        {
            Components = new Container();
            ContextMenu = new ContextMenuStrip(Components)
            {
                Name = "KEKW",
                Text = "ICH BIN ES",
                RenderMode = ToolStripRenderMode.System,
                Width = 100,
                Height = 200,
                BackColor = Color.FromArgb(255, 42, 47, 56),
                ForeColor = Color.White,
            };
            
            OpenBrowserItem = new ToolStripMenuItem()
            {
                Text = "Open in Browser",
                Height = 100,
                Width = 100,
                BackColor = Color.FromArgb(255, 42, 47, 56),
                ForeColor = Color.White,
            };
            CloseAppItem = new ToolStripMenuItem()
            {
                Text = "Close AudioProxy",
                Height = 100,
                Width = 100,
                BackColor = Color.FromArgb(255, 42, 47, 56),
                ForeColor = Color.White,
            };

            OpenBrowserItem.Click += OpenBrowserItem_Click;
            CloseAppItem.Click += CloseAppItem_Click;

            ContextMenu.Items.Add(OpenBrowserItem);
            ContextMenu.Items.Add(CloseAppItem);

            TrayIcon = new NotifyIcon(Components)
            {
                ContextMenuStrip = ContextMenu,
                Text = "AudioProxy",
                Icon = new Icon("cable.ico"),
            };

            TrayIcon.DoubleClick += TrayIcon_DoubleClick;
        }

        public void ShowNotification(string title, string text)
        {
            TrayIcon.BalloonTipTitle = title;
            TrayIcon.BalloonTipText = text;
            TrayIcon.ShowBalloonTip(2500);
        }

        protected override ValueTask InitializeAsync()
        {
            HostLifetime.ApplicationStopping.Register(() => ApplicationContext.Kill());
            return base.InitializeAsync();
        }

        protected override ValueTask RunAsync()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TrayIcon.Visible = true;

            HostLifetime.ApplicationStopping.ThrowIfCancellationRequested();
            Application.Run(ApplicationContext);
            
            return base.RunAsync();
        }
        
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            BrowserHelper.RunAudioProxyInBrowser(false, Config.GetValue<int>("Port"));
        }

        private async void CloseAppItem_Click(object sender, EventArgs e)
        {
            await Host.StopAsync(TimeSpan.FromSeconds(3));
            Environment.Exit(0);
        }

        private void OpenBrowserItem_Click(object sender, EventArgs e)
        {
            BrowserHelper.RunAudioProxyInBrowser(false, Config.GetValue<int>("Port"));
        }
    }
}
