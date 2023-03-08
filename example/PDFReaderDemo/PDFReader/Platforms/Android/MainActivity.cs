using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;

namespace PDFReader;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    public const int REQUEST_ALL_FILE = 100;
    public const int REQUEST_EXTERNAL_STORAGE = 200;

    private string[] PERMISSIONS_STORAGE = {
            Android.Manifest.Permission.ReadExternalStorage,
            Android.Manifest.Permission.WriteExternalStorage
        };

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);


        bool checkPermission = com.foxit.AndroidFileUtil.CheckManageAllFilePermission(this);
        if (!checkPermission)
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                com.foxit.AndroidFileUtil.RequestManageAllFilePermission(this, REQUEST_ALL_FILE);
            }
            else
            {
                com.foxit.AndroidFileUtil.CheckStoragePermission(this, PERMISSIONS_STORAGE, REQUEST_EXTERNAL_STORAGE);
            }

        }
    }


    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        if (!VerifyPermissions(grantResults))
        {
            Android.Widget.Toast.MakeText(ApplicationContext, "", Android.Widget.ToastLength.Short).Show();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Log.Info("mytag", " OnDestroy------------------:");
    }

    private bool VerifyPermissions(Permission[] grantResults)
    {
        if (grantResults.Length < 1)
        {
            return false;
        }

        for (int i = 0; i < grantResults.Length; i++)
        {
            Permission grantResult = grantResults[i];
            if (grantResult != Android.Content.PM.Permission.Granted)
            {
                return false;
            }
        }
        return true;
    }
}
