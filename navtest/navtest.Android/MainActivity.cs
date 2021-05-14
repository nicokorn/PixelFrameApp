using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using System.Diagnostics;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android;

namespace navtest.Droid
{
    [Activity(Label = "navtest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Bluetooth) == (int)Permission.Granted)
            {
                // We have permission
                System.Diagnostics.Debug.WriteLine("Permission granted!");
            }
            else
            {
                // Camera permission is not granted.
                System.Diagnostics.Debug.WriteLine("Permission not granted!");
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Bluetooth }, 1);
            }

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 1)
            {
                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    System.Diagnostics.Debug.WriteLine("BLE permission granted");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("BLE permission not granted");
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }
    }
}