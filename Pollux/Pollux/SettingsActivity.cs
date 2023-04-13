using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Snackbar;
using Pollux.Domain.Data;
using Pollux.Domain.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Pollux
{
    //Note: This can probably be done much nicer/less duplication but oh well, fine for now...
    [Activity(Label = "Settings")]
    public class SettingsActivity : AppCompatActivity
    {
        private Dictionary<string, EditText> _editTexts;
        private Dictionary<string, CheckBox> _checkBoxes;
        private Button _saveButton;

        private PolarisConnectionSettings _polarisConnectionSettings;
        private BeaconHandlerSettings _beaconHandlerSettings;
        private MonitorServiceSettings _monitorServiceSettings;
        private LocationHandlerSettings _locationHandlerSettings;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);

            _polarisConnectionSettings = SettingsHandler.GetPolarisConnectionSettings();
            _beaconHandlerSettings = SettingsHandler.GetBeaconHandlerSettings();
            _monitorServiceSettings = SettingsHandler.GetMonitorServiceSettings();
            _locationHandlerSettings = SettingsHandler.GetLocationHandlerSettings();

            SetupGui();
        }

        private void SetupGui()
        {
            _editTexts = new Dictionary<string, EditText>()
            {
                { nameof(PolarisConnectionSettings.ApiAdress), FindViewById<EditText>(Resource.Id.apiAdress) },
                { nameof(PolarisConnectionSettings.PolarisUsername), FindViewById<EditText>(Resource.Id.polarisUsername) },
                { nameof(PolarisConnectionSettings.PolarisPassword), FindViewById<EditText>(Resource.Id.polarisPassword) },
                { nameof(BeaconHandlerSettings.MovementDestinationCode), FindViewById<EditText>(Resource.Id.movementDestinationCode) },
                { nameof(BeaconHandlerSettings.MovementMinSendIntervalSec), FindViewById<EditText>(Resource.Id.movementMinSendIntervalSec) },
                { nameof(BeaconHandlerSettings.TlmMinSendIntervalSec), FindViewById<EditText>(Resource.Id.tlmMinSendIntervalSec) },
                { nameof(MonitorServiceSettings.DurationOfBleScansSec), FindViewById<EditText>(Resource.Id.durationOfBleScansSec) },
                { nameof(MonitorServiceSettings.TimeBetweenBleScansSec), FindViewById<EditText>(Resource.Id.timeBetweenBleScansSec) },
                { nameof(LocationHandlerSettings.LocationRefreshTimeSec), FindViewById<EditText>(Resource.Id.locationRefreshTimeSec) },

            };
            _checkBoxes = new Dictionary<string, CheckBox>()
            {
                { nameof(BeaconHandlerSettings.CommonWhiteListEnabled), FindViewById<CheckBox>(Resource.Id.commonWhiteListEnabled) },
                { nameof(MonitorServiceSettings.AllowNotificationUpdates), FindViewById<CheckBox>(Resource.Id.allowNotificationUpdates) }
            };
            _saveButton = FindViewById<Button>(Resource.Id.saveButton);

            _editTexts[nameof(PolarisConnectionSettings.ApiAdress)].Text = _polarisConnectionSettings.ApiAdress;
            _editTexts[nameof(PolarisConnectionSettings.PolarisUsername)].Text = _polarisConnectionSettings.PolarisUsername;
            _editTexts[nameof(PolarisConnectionSettings.PolarisPassword)].Text = _polarisConnectionSettings.PolarisPassword;
            _editTexts[nameof(BeaconHandlerSettings.MovementDestinationCode)].Text = _beaconHandlerSettings.MovementDestinationCode;
            _editTexts[nameof(BeaconHandlerSettings.MovementMinSendIntervalSec)].Text = _beaconHandlerSettings.MovementMinSendIntervalSec.ToString();
            _editTexts[nameof(BeaconHandlerSettings.TlmMinSendIntervalSec)].Text = _beaconHandlerSettings.TlmMinSendIntervalSec.ToString();
            _editTexts[nameof(MonitorServiceSettings.DurationOfBleScansSec)].Text = _monitorServiceSettings.DurationOfBleScansSec.ToString();
            _editTexts[nameof(MonitorServiceSettings.TimeBetweenBleScansSec)].Text = _monitorServiceSettings.TimeBetweenBleScansSec.ToString();
            _editTexts[nameof(LocationHandlerSettings.LocationRefreshTimeSec)].Text = _locationHandlerSettings.LocationRefreshTimeSec.ToString();

            _checkBoxes[nameof(BeaconHandlerSettings.CommonWhiteListEnabled)].Checked = _beaconHandlerSettings.CommonWhiteListEnabled;
            _checkBoxes[nameof(MonitorServiceSettings.AllowNotificationUpdates)].Checked = _monitorServiceSettings.AllowNotificationUpdates;

            _saveButton.Click += SaveSettings;
        }

        private void SaveSettings(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            try
            {
                _polarisConnectionSettings.ApiAdress = _editTexts[nameof(PolarisConnectionSettings.ApiAdress)].Text;
                _polarisConnectionSettings.PolarisUsername = _editTexts[nameof(PolarisConnectionSettings.PolarisUsername)].Text;
                _polarisConnectionSettings.PolarisPassword = _editTexts[nameof(PolarisConnectionSettings.PolarisPassword)].Text;
                _beaconHandlerSettings.MovementDestinationCode = _editTexts[nameof(BeaconHandlerSettings.MovementDestinationCode)].Text;
                _beaconHandlerSettings.MovementMinSendIntervalSec = Convert.ToInt32(_editTexts[nameof(BeaconHandlerSettings.MovementMinSendIntervalSec)].Text);
                _beaconHandlerSettings.TlmMinSendIntervalSec = Convert.ToInt32(_editTexts[nameof(BeaconHandlerSettings.TlmMinSendIntervalSec)].Text);
                _monitorServiceSettings.DurationOfBleScansSec = Convert.ToInt32(_editTexts[nameof(MonitorServiceSettings.DurationOfBleScansSec)].Text);
                _monitorServiceSettings.TimeBetweenBleScansSec = Convert.ToInt32(_editTexts[nameof(MonitorServiceSettings.TimeBetweenBleScansSec)].Text);
                _locationHandlerSettings.LocationRefreshTimeSec = Convert.ToInt32(_editTexts[nameof(LocationHandlerSettings.LocationRefreshTimeSec)].Text);

                _beaconHandlerSettings.CommonWhiteListEnabled = _checkBoxes[nameof(BeaconHandlerSettings.CommonWhiteListEnabled)].Checked;
                _monitorServiceSettings.AllowNotificationUpdates = _checkBoxes[nameof(MonitorServiceSettings.AllowNotificationUpdates)].Checked;

                _polarisConnectionSettings.Save();
                _beaconHandlerSettings.Save();
                _monitorServiceSettings.Save();
                _locationHandlerSettings.Save();
            }catch(Exception ex)
            {
                Snackbar.Make(view, "Error: " + ex.Message, Snackbar.LengthLong).SetAction("Action", (View.IOnClickListener)null).Show();
                return;
            }
            Snackbar.Make(view, "Settings saved", Snackbar.LengthLong).SetAction("Action", (View.IOnClickListener)null).Show();
            //Finish();
        }
    }
}