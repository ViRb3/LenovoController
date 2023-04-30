using LenovoController.Features;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using MouseButtons = System.Windows.Forms.MouseButtons;

namespace LenovoController
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RadioButton[] _alwaysOnUsbButtons;
        private readonly AlwaysOnUsbFeature _alwaysOnUsbFeature = new AlwaysOnUsbFeature();
        private readonly RadioButton[] _batteryButtons;
        private readonly BatteryFeature _batteryFeature = new BatteryFeature();
        private readonly NotifyIcon _notifyIcon;
        private readonly RadioButton[] _powerModeButtons;
        private readonly PowerModeFeature _powerModeFeature = new PowerModeFeature();
        private readonly FnLockFeature _fnLockFeature = new FnLockFeature();
        private readonly OverDriveFeature _overDriveFeature = new OverDriveFeature();
        private readonly TouchpadLockFeature _touchpadLockFeature = new TouchpadLockFeature();

        public MainWindow()
        {
            InitializeComponent();

            // Check if the program was started with the "-minimized" command-line argument
            var startMinimized = false;
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1] == "-minimized")
            {
                startMinimized = true;
            }

            if (startMinimized)
            {
                // Set the WindowState property to WindowState.Minimized to start the program minimized
                WindowState = WindowState.Minimized;
            }

            mainWindow.Title += $" v{AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version}";
            _powerModeButtons = new[] { radioQuiet, radioBalance, radioPerformance };
            _batteryButtons = new[] { radioConservation, radioNormalCharge, radioRapidCharge };
            _alwaysOnUsbButtons = new[] { radioAlwaysOnUsbOff, radioAlwaysOnUsbOnWhenSleeping, radioAlwaysOnUsbOnAlways };
            Refresh();

            // Initialize NotifyIcon
            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("icon.ico"),
                Visible = true,
                Text = "Lenovo Controller"
            };
            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = WindowState.Normal;
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            base.OnStateChanged(e);
        }

        private class FeatureCheck
        {
            private readonly Action _check;
            private readonly Action _disable;

            internal FeatureCheck(Action check, Action disable)
            {
                _check = check;
                _disable = disable;
            }

            internal void Check() => _check();
            internal void Disable() => _disable();
        }

        private void Refresh()
        {
            var features = new[]
            {
                new FeatureCheck(
                    () => _powerModeButtons[(int) _powerModeFeature.GetState()].IsChecked = true,
                    () => DisableControls(_powerModeButtons)),
                new FeatureCheck(
                    () => _batteryButtons[(int) _batteryFeature.GetState()].IsChecked = true,
                    () => DisableControls(_batteryButtons)),
                new FeatureCheck(
                    () => _alwaysOnUsbButtons[(int) _alwaysOnUsbFeature.GetState()].IsChecked = true,
                    () => DisableControls(_alwaysOnUsbButtons)),
                new FeatureCheck(
                    () => chkOverDrive.IsChecked = _overDriveFeature.GetState() == OverDriveState.On,
                    () => chkOverDrive.IsEnabled = false),
                new FeatureCheck(
                    () => chkTouchpadLock.IsChecked = _touchpadLockFeature.GetState() == TouchpadLockState.On,
                    () => chkTouchpadLock.IsEnabled = false),
                new FeatureCheck(
                    () => chkFnLock.IsChecked = _fnLockFeature.GetState() == FnLockState.On,
                    () => chkFnLock.IsEnabled = false)
            };

            foreach (var feature in features)
            {
                try
                {
                    feature.Check();
                }
                catch (Exception e)
                {
                    Trace.TraceInformation("Could not refresh feature: " + e);
                    feature.Disable();
                }
            }
        }

        private void DisableControls(Control[] buttons)
        {
            foreach (var btn in buttons)
                btn.IsEnabled = false;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void radioPowerMode_Checked(object sender, RoutedEventArgs e)
        {
            _powerModeFeature.SetState((PowerModeState)Array.IndexOf(_powerModeButtons, sender));
        }

        private void radioBattery_Checked(object sender, RoutedEventArgs e)
        {
            _batteryFeature.SetState((BatteryState)Array.IndexOf(_batteryButtons, sender));
        }

        private void radioAlwaysOnUsb_Checked(object sender, RoutedEventArgs e)
        {
            _alwaysOnUsbFeature.SetState((AlwaysOnUsbState)Array.IndexOf(_alwaysOnUsbButtons, sender));
        }

        private void chkOverDrive_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkOverDrive.IsChecked.GetValueOrDefault(false)
                ? OverDriveState.On
                : OverDriveState.Off;
            _overDriveFeature.SetState(state);
        }

        private void chkTouchpadLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkTouchpadLock.IsChecked.GetValueOrDefault(false)
                ? TouchpadLockState.On
                : TouchpadLockState.Off;
            _touchpadLockFeature.SetState(state);
        }

        private void chkFnLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkFnLock.IsChecked.GetValueOrDefault(false)
                ? FnLockState.On
                : FnLockState.Off;
            _fnLockFeature.SetState(state);
        }
    }
}