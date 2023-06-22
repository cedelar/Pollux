using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Pollux.Adapters;
using Pollux.BleMonitor;
using Pollux.Domain.Data;
using Pollux.Domain.Helper;
using Pollux.Domain.Processing;
using System;
using System.Linq;
using System.Timers;
using Xamarin.Essentials;

namespace Pollux
{
    [Activity(Label = "ServiceMonitor")]
    public class MonitorActivity : AppCompatActivity
    {
        private static string _dateTimeFormat => "HH:mm:ss";
        private string[][] _listViewData;
        private BleMonitorServiceConnection _serviceConnection;
        private GuiSettings _guiSettings;
        private BeaconHandlerSettings _beaconHandlerSettings;

        private ListView _beaconListView;
        private Timer _refreshTimer;

        private Button _toggleSortByRssiButton;
        private Button _toggleSortByMacButton;
        private ImageButton _refreshListButton;
        private TextView _beaconAmountInListLabel;
        private TextView _currentFilterLabel;

        private bool _sortByRssiEnabled;
        private bool _sortByMacEnabled;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_monitor);

            if (this.IsServiceRunning(nameof(BleMonitorService)))
            {
                var intent = new Intent(this, typeof(BleMonitorService));

                if (_serviceConnection == null)
                {
                    _serviceConnection = new BleMonitorServiceConnection();
                    _serviceConnection.OnConnectionChanged += OnServiceConnectionStatusChanged;
                }

                BindService(intent, _serviceConnection, Bind.AutoCreate);
            }

            _guiSettings = SettingsHandler.GetGuiSettings();
            _beaconHandlerSettings = SettingsHandler.GetBeaconHandlerSettings();    

            _beaconListView = FindViewById<ListView>(Resource.Id.listview);
            _refreshTimer = new Timer(1000);
            _refreshTimer.Elapsed += OnRefreshTimerElapsed;

            _currentFilterLabel = FindViewById<TextView>(Resource.Id.currentFilter);
            _beaconAmountInListLabel = FindViewById<TextView>(Resource.Id.beaconAmountInList);
            _toggleSortByMacButton = FindViewById<Button>(Resource.Id.macSortButton);
            _toggleSortByRssiButton = FindViewById<Button>(Resource.Id.rssiSortButton);
            _refreshListButton = FindViewById<ImageButton>(Resource.Id.refreshList);
            _toggleSortByMacButton.Click += ToggleSortByMAC;
            _toggleSortByRssiButton.Click += ToggleSortByRssi;
            _refreshListButton.Click += RefreshList;
            ToggleSortByRssi(this, null);

            _beaconAmountInListLabel.Text = "0";
        }

        private void ToggleSortByRssi(object sender, EventArgs eventArgs)
        {
            _sortByRssiEnabled = !_sortByRssiEnabled;
            _sortByMacEnabled = false;
            SetCurrentFilterLabel();
        }

        private void ToggleSortByMAC(object sender, EventArgs eventArgs)
        {
            _sortByMacEnabled = !_sortByMacEnabled;
            _sortByRssiEnabled = false;
            SetCurrentFilterLabel();
        }

        private void RefreshList(object sender, EventArgs eventArgs)
        {
            if(_serviceConnection != null)
            {
                _serviceConnection.ResetBeaconData();
                OnRefreshTimerElapsed(null, null);
            }
        }

        private void SetCurrentFilterLabel()
        {
            var text = "(None)";
            if (_sortByMacEnabled)
            {
                text = "(Mac)";
            }else if (_sortByRssiEnabled)
            {
                text = "(Rssi)";
            }
            _currentFilterLabel.Text = text;    
        }

        private void OnRefreshTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _listViewData = GetAndConvertBeaconData();
            _beaconAmountInListLabel.Text = _listViewData.Length.ToString();
            RunOnUiThread(() => {
                var adapter = (DualLineListViewAdapter)_beaconListView.Adapter;
                adapter.SetItems(_listViewData);
                adapter.NotifyDataSetChanged();
            });
        }

        public void OnServiceConnectionStatusChanged(object sender, bool IsConnected)
        {
            _listViewData = GetAndConvertBeaconData();
            _beaconListView.Adapter = new DualLineListViewAdapter(this, _listViewData);
            _refreshTimer.Start();
        }

        private string[][] GetAndConvertBeaconData()
        {
            if(_serviceConnection == null)
            {
                return new string[0][];
            }
            var data = _serviceConnection.GetBeaconData().Values.ToList();

            if(_beaconHandlerSettings.CommonWhiteListEnabled && _guiSettings.HideNonWhitelistedBeaconsInMonitor)
            {
                data = data.Where(x => _beaconHandlerSettings.CommonWhitelist.Contains(x.MacAdress)).ToList();
            }

            if (_sortByMacEnabled)
            {
                data = data.OrderBy(x => x.MacAdress).ToList();
            }
            if (_sortByRssiEnabled)
            {
                data = data.OrderByDescending(x => x.LastRssi).ToList();
            }

            return data.Select(x => new string[] { 
                x.MacAdress + " (" + x.LastRssi + "dB)  Seen: " + DateTimeConverter(x.LastSeenTime),
                "Movement: " + DateTimeConverter(x.TimeOfEvent(Domain.Data.EventTypes.MovementSend)) + ", Tlm: " + DateTimeConverter(x.TimeOfEvent(Domain.Data.EventTypes.TlmSend))
            }).ToArray();        
        }

        private string DateTimeConverter(DateTime dateTime)
        {
            if(dateTime == DateTime.MinValue)
            {
                return "-";
            }
            return dateTime.ToLocalTime().ToString(_dateTimeFormat);
        }
    }

   
}