using System;

namespace AudioProxy.Services
{
    [Serializable]
    public sealed class WindowMessage
    {
#pragma warning disable IDE1006
        public IntPtr hwnd;

        public IntPtr lParam;

        public int message;

        public int pt_x;

        public int pt_y;

        public int time;

        public IntPtr wParam;
#pragma warning restore IDE1006
    }
}
