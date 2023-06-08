using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Microsoft.AppCenter.Crashes;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Pollux.Domain.Data;
using Pollux.Domain.Processing;
using System;
using System.Collections.Generic;
using System.Timers;
using IAdapter = Plugin.BLE.Abstractions.Contracts.IAdapter;

namespace Pollux.BleMonitor
{
    [Service(Label = "BluetoothMonitor")]
    public class BleMonitorService : Service
    {
        private MonitorServiceSettings _settings;
        private static readonly int _notifId = 9000;
        private static readonly string _channelId = "9001";
        private static readonly Context _context = Application.Context;

        private IBinder _binder;
        private Timer _restartTimer;

        private IBluetoothLE _bleStateObserver;
        private IAdapter _bleConnection;
        private BleBeaconHandler _beaconHandler;

        public override void OnCreate()
        {
            base.OnCreate();
            _settings = SettingsHandler.GetMonitorServiceSettings();
            _bleStateObserver = CrossBluetoothLE.Current;
            _bleConnection = _bleStateObserver.Adapter;
            _bleConnection.ScanMode = ScanMode.LowLatency;
            _bleConnection.ScanTimeout = (_settings.DurationOfBleScansSec * 1000);
            _bleConnection.DeviceDiscovered += OnDeviceDiscovered;

            _beaconHandler = new BleBeaconHandler();
            _beaconHandler.NotificationupdateRequested += OnNotificationupdateRequested;

            _restartTimer = new Timer(_settings.TimeBetweenBleScansSec * 1000);
            _restartTimer.Elapsed += new ElapsedEventHandler(OnUpdateTimerElapsed);
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                var notif = BuildNotification(_settings.NotificationContentText);
                StartForeground(_notifId, notif);
                _restartTimer.Start();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return StartCommandResult.Sticky;
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            if(args != null && args.Device != null)
            {
                _beaconHandler.SubmitDevice(args.Device);
            }
        }

        private void OnNotificationupdateRequested(object sender, string content)
        {
            if (_settings.AllowNotificationUpdates && !string.IsNullOrEmpty(content))
            {
                UpdateNotificationContent(content);
            }
        }

        private async void OnUpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                await _bleConnection.StartScanningForDevicesAsync();
                _restartTimer.Start();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private Notification BuildNotification(string Text)
        {
            var pendingIntent = PendingIntent.GetActivity(_context, 0, new Intent(_context, typeof(MainActivity)), PendingIntentFlags.UpdateCurrent);

            var notifBuilder = new NotificationCompat.Builder(_context, _channelId)
                .SetContentTitle(_settings.NotificationTitleText)
                .SetContentText(Text)
                .SetSmallIcon(Resource.Drawable.logo)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel notificationChannel = new NotificationChannel(_channelId, "Title", NotificationImportance.High);
                notificationChannel.EnableLights(true);
                notificationChannel.EnableVibration(false);
                notificationChannel.SetShowBadge(true);
                notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });

                var notifManager = _context.GetSystemService(NotificationService) as NotificationManager;
                if (notifManager != null)
                {
                    notifBuilder.SetChannelId(_channelId);
                    notifManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return notifBuilder.Build();
        }

        private void UpdateNotificationContent(string content)
        {
            var notif = BuildNotification(content);
            var notifManager = _context.GetSystemService(NotificationService) as NotificationManager;
            notifManager.Notify(_notifId, notif);
        }

        public override IBinder OnBind(Intent intent)
        {
            _binder = new BleMonitorServiceBinder(this);
            return _binder;
        }

        public override void OnDestroy()
        {
            _bleConnection = null;
            _bleStateObserver = null;
            _binder = null;
            _restartTimer.Dispose();
            base.OnDestroy();
        }

        public Dictionary<string, HistoricalBleBeacon> GetBeaconData()
        {
            if(_beaconHandler != null)
            {
                return _beaconHandler.BeaconDictionary;
            }
            return new Dictionary<string, HistoricalBleBeacon>();
        }

        public void ResetBeaconData()
        {
            if(_beaconHandler != null)
            {
                _beaconHandler.ResetMemory();
            }
        }
    }
}