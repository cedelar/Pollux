using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Snackbar;
using Xamarin.Essentials;
using Android.Content;
using Pollux.BleMonitor;
using Android.Widget;
using Pollux.Domain.Helper;
using Microsoft.AppCenter.Crashes;
using AndroidX.Core.Content;
using Android;
using Android.Content.PM;

namespace Pollux
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/logo", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private bool _bleServiceRunning;
        private BleMonitorServiceConnection _serviceConnection;

        private TextView _welcomeText;
        private Button _startButton;
        private Button _stopButton;
        private Button _monitorButton;
        private Button _settingsButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            _bleServiceRunning = this.IsServiceRunning(nameof(BleMonitorService));
            SetupGui();
            SetupCrashReporting();
            //GetRequiredPermissions();
        }

        private void SetupGui()
        {
            _welcomeText = FindViewById<TextView>(Resource.Id.welcomeText);
            _welcomeText.Text = "Welcome to the Pollux BLE middleware POC software.";

            _startButton = FindViewById<Button>(Resource.Id.startButton);
            _startButton.Text = "Start Service";
            _startButton.Click += StartBleService;

            _stopButton = FindViewById<Button>(Resource.Id.stopButton);
            _stopButton.Text = "Stop Service";
            _stopButton.Click += StopBleService;

            _monitorButton = FindViewById<Button>(Resource.Id.monitorButton);
            _monitorButton.Text = "Monitor Service";
            _monitorButton.Click += NavigateToMonitorActivity;

            _settingsButton = FindViewById<Button>(Resource.Id.settingsButton);
            _settingsButton.Text = "Settings";
            _settingsButton.Click += NavigateToSettingsActivity;

            EvaluateButtonEnabled();
        }

        private void GetRequiredPermissions()
        {
            this.CheckAndRequestPermission(Manifest.Permission.Bluetooth);
            this.CheckAndRequestPermission(Manifest.Permission.BluetoothAdmin);
            this.CheckAndRequestPermission(Manifest.Permission.ForegroundService);
            this.CheckAndRequestPermission(Manifest.Permission.AccessNetworkState);
            this.CheckAndRequestPermission(Manifest.Permission.AccessCoarseLocation);
            this.CheckAndRequestPermission(Manifest.Permission.AccessFineLocation);
        }

        private async void SetupCrashReporting()
        {
           if(!await Crashes.IsEnabledAsync())
           {
                await Crashes.SetEnabledAsync(true);
           }
        }

        private void StartBleService(object sender, EventArgs eventArgs)
        {
            var intent = new Intent(this, typeof(BleMonitorService));

            if (_serviceConnection == null)
            {
                _serviceConnection = new BleMonitorServiceConnection();
            }

            BindService(intent, _serviceConnection, Bind.AutoCreate);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(intent);
            }
            else
            {
                StartService(intent);
            }

            View view = (View) sender;
            Snackbar.Make(view, "BleMonitor started", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
            EvaluateButtonEnabled();
        }

        private void StopBleService(object sender, EventArgs eventArgs)
        {
            var intent = new Intent(this, typeof(BleMonitorService));
            if(_serviceConnection != null && _serviceConnection.IsConnected)
            {
                UnbindService(_serviceConnection);
            }
            StopService(intent);
            EvaluateButtonEnabled();
        }

        private void NavigateToMonitorActivity(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(MonitorActivity));
        }

        private void NavigateToSettingsActivity(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(SettingsActivity));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void EvaluateButtonEnabled()
        {
            _bleServiceRunning = this.IsServiceRunning(nameof(BleMonitorService));
            _startButton.Enabled = !_bleServiceRunning;
            _stopButton.Enabled = _bleServiceRunning;
            _monitorButton.Enabled = _bleServiceRunning;
            _settingsButton.Enabled = !_bleServiceRunning;
        }

    }
}
