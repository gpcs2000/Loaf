using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using iNKORE.UI.WPF.Modern.Controls.Primitives;
using Loaf.Event;
using Prism.Events;

namespace Loaf.Views
{
    /// <summary>
    /// FullScreenWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FullScreenWindow : Window
    {
        public FullScreenWindow(IEventAggregator aggregator)
        {
            InitializeComponent();
            FullScreenHelper.StartFullScreen(this);
            Loaded += MainWindow_Loaded;
            InitializeWindow();
            _aggregator = aggregator;
            _aggregator.GetEvent<CloseEvent>().Subscribe(async void () =>
            {
                await Task.Delay(new Random().Next(1000, 5000));
                Dispatcher.Invoke(SafeClose);
            });
        }

        private void InitializeWindow()
        {
            // 设置窗口样式
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            Topmost = true;
            ResizeMode = ResizeMode.NoResize;

            // 注册键盘钩子
            _hookID = SetHook(_proc);

            // 注册事件
            PreviewKeyDown += MainWindow_PreviewKeyDown;
            Closed += MainWindow_Closed;

            // 禁用系统热键
            SourceInitialized += (s, e) => DisableSystemHotKeys();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置窗口置顶，并且保持在其他置顶窗口的前面
            SetWindowPos(new WindowInteropHelper(this).Handle, -1, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        private void FullScreenWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }


        [Flags]
        private enum SetWindowPosFlags
        {
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_DRAWFRAME = 0x0020,
            SWP_NOREPOSITION = 0x0200,
            SWP_DEFERERASE = 0x2000,
            SWP_ASYNCWINDOWPOS = 0x4000
        }

        #region 事件处理

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SafeClose();
            }
            else
            {
                e.Handled = true;
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
            EnableSystemHotKeys();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(handle)?.AddHook(WndProc);

            // 防止任务管理器
            int style = GetWindowLong(handle, GWL_EXSTYLE);
            SetWindowLong(handle, GWL_EXSTYLE, style | WS_EX_NOACTIVATE);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_allowClosing)
            {
                e.Cancel = true;
            }

            base.OnClosing(e);
        }

        #endregion

        #region 键盘钩子相关方法

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            int vkCode = Marshal.ReadInt32(lParam);
            Key key = KeyInterop.KeyFromVirtualKey(vkCode);

            if (wParam != WM_KEYDOWN && wParam != WM_SYSKEYDOWN)
            {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            // 如果不是ESC键，则拦截
            return !_allowedKeys.Contains(key) ? 1 : CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #endregion

        #region 系统热键控制

        private void DisableSystemHotKeys()
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;

            // 禁用各种组合键
            RegisterHotKey(handle, 0, (uint) ModifierKeys.Alt, 0x09); // Alt + Tab
            RegisterHotKey(handle, 1, (uint) (ModifierKeys.Control | ModifierKeys.Alt), 0x2E); // Ctrl + Alt + Delete
            RegisterHotKey(handle, 2, (uint) ModifierKeys.Windows, 0x44); // Win + D
            RegisterHotKey(handle, 3, (uint) ModifierKeys.Windows, 0x52); // Win + R
            RegisterHotKey(handle, 4, (uint) ModifierKeys.Windows, 0x45); // Win + E
            RegisterHotKey(handle, 5, 0, (uint) Key.LWin); // Left Windows Key
            RegisterHotKey(handle, 6, 0, (uint) Key.RWin); // Right Windows Key
        }

        private void EnableSystemHotKeys()
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;

            // 重新启用热键
            for (int i = 0; i < 7; i++)
            {
                UnregisterHotKey(handle, i);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WM_SYSCOMMAND)
            {
                return IntPtr.Zero;
            }

            int command = wParam.ToInt32() & 0xFFF0;
            if (command != SC_TASKLIST)
            {
                return IntPtr.Zero;
            }

            handled = true;
            return IntPtr.Zero;
        }

        private void SafeClose()
        {
            _allowClosing = true;
            Close();
        }

        #endregion

        #region Win32 API

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion


        #region 字段和常量

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_TASKLIST = 0xF130;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        private static readonly LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private bool _allowClosing;
        private readonly IEventAggregator _aggregator;

        // 定义允许的按键
        private static readonly HashSet<Key> _allowedKeys = [Key.Escape];

        #endregion
    }
}