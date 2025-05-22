using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics;
using WinRT.Interop;

namespace test.Helpers
{
    public static class WindowHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static (int X, int Y) GetWindowPosition(Window window)
        {
            var hwnd = WindowNative.GetWindowHandle(window);
            if (GetWindowRect(hwnd, out var rect))
            {
                return (rect.Left, rect.Top);
            }

            return (0, 0);
        }

        public static AppWindow GetAppWindow(Window window)
        {
            var hwnd = WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            return AppWindow.GetFromWindowId(windowId);
        }

        //move window to the center of the screen
        public static void CenterWindow(Window window)
        {
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);

            if (displayArea != null)
            {
                var centerX = displayArea.WorkArea.Width / 2 - appWindow.Size.Width / 2;
                var centerY = displayArea.WorkArea.Height / 2 - appWindow.Size.Height / 2;

                appWindow.Move(new PointInt32(centerX, centerY));
            }
        }

        public static void Maximize(Window window)
        {
            
            if (window != null)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);

                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.Maximize();
                }
            }
        }

    }
}


