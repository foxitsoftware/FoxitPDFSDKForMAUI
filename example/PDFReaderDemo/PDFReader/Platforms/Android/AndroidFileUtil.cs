using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Content;
using Android.Provider;
using AndroidX.Core.App;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;

namespace com.foxit
{
    public class AndroidFileUtil
    {

        public static bool CheckManageAllFilePermission(Context context)
        {

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                return Android.OS.Environment.IsExternalStorageManager;
            }

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                
                return ContextCompat.CheckSelfPermission(context, Android.Manifest.Permission.ReadExternalStorage) == Android.Content.PM.Permission.Granted
                        && ContextCompat.CheckSelfPermission(context, Android.Manifest.Permission.WriteExternalStorage) == Android.Content.PM.Permission.Granted;
            }
            return true;
        }

        public static void RequestManageAllFilePermission(Activity act, int reqCode)
        {
            Intent intent = new Intent(Settings.ActionManageAppAllFilesAccessPermission);
            intent.SetData(Android.Net.Uri.Parse("package:" + act.ApplicationContext.PackageName));
            act.StartActivityForResult(intent, reqCode);
        }

        public static void CheckStoragePermission(Activity act, string[] permissions, int reqCode)
        {
            ActivityCompat.RequestPermissions(act, permissions, reqCode);
        }

    }


}
