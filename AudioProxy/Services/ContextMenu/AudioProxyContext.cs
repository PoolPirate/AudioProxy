using System.Diagnostics;
using System.Windows.Forms;

namespace AudioProxy.Services
{
    public sealed class AudioProxyContext : ApplicationContext
    {
        public void Kill()
        {
            ExitThread();
            ExitThreadCore();

            Dispose();

            Debug.Print("HI");
        }
    }
}
