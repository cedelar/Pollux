using Microsoft.AppCenter.Crashes;
using Pollux.Domain.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Pollux.Domain.Processing
{
    public class LocationHandler
    {
        private readonly LocationHandlerSettings _settings;

        private DateTime _lastRefresh;
        private Coordinates _currentCoordinates;

        public LocationHandler()
        {
            _settings = SettingsHandler.GetLocationHandlerSettings();
            _currentCoordinates = new Coordinates()
            {
                Latitude = 0,
                Longitude = 0,
                Accuracy = 0
            };
        }

        public async Task<Coordinates> GetCurrentCoordinates()
        {
            if(_currentCoordinates == null || _lastRefresh.AddSeconds(_settings.LocationRefreshTimeSec) < DateTime.UtcNow)
            {
                await Refresh();
            }
            return _currentCoordinates;
        }

        private async Task Refresh()
        {
            try
            {
                CancellationTokenSource cts;

                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(60));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                _currentCoordinates = new Coordinates()
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Accuracy = location.Accuracy ?? 0
                };        
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

    }
}