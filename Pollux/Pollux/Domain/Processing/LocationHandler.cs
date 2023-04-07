using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using Pollux.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Pollux.Domain.Processing
{
    public class LocationHandler
    {
        private static int _minRefreshTime = 10000;

        private DateTime _lastRefresh;
        private Coordinates _currentCoordinates;

        public LocationHandler()
        {
            _currentCoordinates = new Coordinates()
            {
                Latitude = 0,
                Longitude = 0,
                Accuracy = 0
            };
        }

        public async Task<Coordinates> GetCurrentCoordinates()
        {
            if(_currentCoordinates == null || _lastRefresh.AddMilliseconds(_minRefreshTime) < DateTime.UtcNow)
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