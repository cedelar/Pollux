using Android.App;
using Android.Content;
namespace Pollux.Domain.Helper
{
    public static  class ServiceHelper
    {
        public static bool IsServiceRunning(this Activity activity, string serviceClass)
        {
            if (activity == null)
            {
                return false;
            }
            ActivityManager manager = (ActivityManager)activity.GetSystemService(Context.ActivityService);
            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName.EndsWith(serviceClass))
                {
                    return true;
                }
            }
            return false;
        }
    }
}