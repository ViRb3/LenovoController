using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using LenovoController.Features;

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
        private readonly RadioButton[] _fanProfileButtons;
        private readonly FanProfileFeature _fanProfileFeature = new FanProfileFeature();
        private readonly FnLockFeature _fnLockFeature = new FnLockFeature();
        private readonly OverDriveFeature _overDriveFeature = new OverDriveFeature();
        private readonly TouchpadLockFeature _touchpadLockFeature = new TouchpadLockFeature();

        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Title += $" v{AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version}";
            _fanProfileButtons = new[] {radioQuiet, radioBalance, radioPerformance};
            _batteryButtons = new[] {radioConservation, radioNormalCharge, radioRapidCharge};
            _alwaysOnUsbButtons = new[] {radioAlwaysOnUsbOff, radioAlwaysOnUsbOnWhenSleeping, radioAlwaysOnUsbOnAlways};
            Refresh();
        }

        private void Refresh()
        {
            var actions = new[]
            {
                new Tuple<Action, Action>(
                    () => _fanProfileButtons[(int) _fanProfileFeature.GetState()].IsChecked = true,
                    () => DisableButtons(_fanProfileButtons)),
                new Tuple<Action, Action>(
                    () => _batteryButtons[(int) _batteryFeature.GetState()].IsChecked = true,
                    () => DisableButtons(_batteryButtons)),
                new Tuple<Action, Action>(
                    () => _alwaysOnUsbButtons[(int) _alwaysOnUsbFeature.GetState()].IsChecked = true,
                    () => DisableButtons(_alwaysOnUsbButtons)),
                new Tuple<Action, Action>(
                    () => chkOverDrive.IsChecked = _overDriveFeature.GetState() == OverDriveState.On,
                    () => chkOverDrive.IsEnabled = false),
                new Tuple<Action, Action>(
                    () => chkTouchpadLock.IsChecked = _touchpadLockFeature.GetState() == TouchpadLockState.On,
                    () => chkTouchpadLock.IsEnabled = false),
                new Tuple<Action, Action>(
                    () => chkFnLock.IsChecked = _fnLockFeature.GetState() == FnLockState.On,
                    () => chkFnLock.IsEnabled = false)
            };

            foreach (var action in actions)
            {
                try
                {
                    action.Item1();
                }
                catch (Exception e)
                {
                    Trace.TraceInformation("Could not refresh feature: " + e);
                    action.Item2();
                }
            }
        }

        private void DisableButtons(RadioButton[] buttons)
        {
            foreach (var btn in buttons)
                btn.IsEnabled = false;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void radioFanProfile_Checked(object sender, RoutedEventArgs e)
        {
            _fanProfileFeature.SetState((FanProfileState) Array.IndexOf(_fanProfileButtons, sender));
        }

        private void radioBattery_Checked(object sender, RoutedEventArgs e)
        {
            _batteryFeature.SetState((BatteryState) Array.IndexOf(_batteryButtons, sender));
        }

        private void radioAlwaysOnUsb_Checked(object sender, RoutedEventArgs e)
        {
            _alwaysOnUsbFeature.SetState((AlwaysOnUsbState) Array.IndexOf(_alwaysOnUsbButtons, sender));
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