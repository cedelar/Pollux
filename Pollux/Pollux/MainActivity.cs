using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Xamarin.Essentials;
using Android.Content;
using Pollux.BleMonitor;

namespace Pollux
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/logo", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private BleMonitorServiceConnection _serviceConnection;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton start = FindViewById<FloatingActionButton>(Resource.Id.start);
            start.Click += StartBleMonitor;

            FloatingActionButton stop = FindViewById<FloatingActionButton>(Resource.Id.stop);
            start.Click += StopBleMonitor;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void StartBleMonitor(object sender, EventArgs eventArgs)
        {
            var intent = new Intent(this, typeof(BleMonitorService));

            if (_serviceConnection == null)
            {
                _serviceConnection = new BleMonitorServiceConnection(this);
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
        }

        private void StopBleMonitor(object sender, EventArgs eventArgs)
        {
            var intent = new Intent(this, typeof(BleMonitorService));
            StopService(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
