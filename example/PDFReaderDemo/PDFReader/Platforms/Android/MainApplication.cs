using Android.App;
using Android.Runtime;
using Java.Lang;
using Android.Util;

namespace PDFReader;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
 
        //try
        //{
        //    JavaSystem.LoadLibrary("fsdk"); 
        //}
        //catch (UnsatisfiedLinkError e)
        //{
        //    Log.Error("mytag", " load failed ---:" + e.Message);
        //}
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
