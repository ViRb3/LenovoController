using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using LenovoController.Features;

namespace LenovoController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FanProfileFeature _fanProfileFeature = new FanProfileFeature();
        private readonly BatteryFeature _batteryFeature = new BatteryFeature();
        private readonly AlwaysOnUsbFeature _alwaysOnUsbFeature = new AlwaysOnUsbFeature();
        private readonly OverDriveFeature _overDriveFeature = new OverDriveFeature();
        private readonly TouchpadLockFeature _touchpadLockFeature = new TouchpadLockFeature();
        private readonly FnLockFeature _fnLockFeature = new FnLockFeature();

        private readonly RadioButton[] _fanProfileButtons;
        private readonly RadioButton[] _alwaysOnUsbButtons;
        private readonly RadioButton[] _batteryButtons;

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
            _fanProfileButtons[(int) _fanProfileFeature.GetState()].IsChecked = true;
            _batteryButtons[(int) _batteryFeature.GetState()].IsChecked = true;
            _alwaysOnUsbButtons[(int) _alwaysOnUsbFeature.GetState()].IsChecked = true;
            chkOverDrive.IsChecked = _overDriveFeature.GetState() == OverDriveState.On;
            chkTouchpadLock.IsChecked = _touchpadLockFeature.GetState() == TouchpadLockState.On;
            chkFnLock.IsChecked = _fnLockFeature.GetState() == FnLockState.On;
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