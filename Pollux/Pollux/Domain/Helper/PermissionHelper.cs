using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pollux.Domain.Helper
{
    public static class PermissionHelper
    {
        public static void CheckAndRequestPermission(this Activity callingActivity, string requestedPermission)
        {
            int requestCode = 999;
            // Check for permissions
            if (ContextCompat.CheckSelfPermission(callingActivity, requestedPermission) != (int)Permission.Granted)
            {
                callingActivity.RequestPermissions(new string[] { requestedPermission }, requestCode);
            }
        }
    }
}