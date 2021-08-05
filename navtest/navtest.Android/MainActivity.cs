using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using System.Diagnostics;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android;
using System.Threading.Tasks;
using System.IO;
using Android.Content;
using Xamarin.Forms.Platform.Android;

namespace navtest.Droid
{
    [Activity(Label = "navtest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted)
            {
                // We have permission
                System.Diagnostics.Debug.WriteLine("Permission coarse location granted!");
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessCoarseLocation }, 1);
            }
            else
            {
                // Camera permission is not granted.
                System.Diagnostics.Debug.WriteLine("Permission coarse location not granted!");
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessCoarseLocation }, 1);
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {
                // We have permission
                System.Diagnostics.Debug.WriteLine("Permission fine location granted!");
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessFineLocation }, 2);
            }
            else
            {
                // Camera permission is not granted.
                System.Diagnostics.Debug.WriteLine("Permission  fine location not granted!");
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessFineLocation }, 2);
            }

            if (    ContextCompat.CheckSelfPermission(this, Manifest.Permission.Bluetooth) == (int)Permission.Granted )
            {
                // We have permission
                System.Diagnostics.Debug.WriteLine("Permission ble granted!");
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Bluetooth }, 3);
            }
            else
            {
                // Camera permission is not granted.
                System.Diagnostics.Debug.WriteLine("Permission ble not granted!");
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Bluetooth }, 3);
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothAdmin) == (int)Permission.Granted)
            {
                // We have permission
                System.Diagnostics.Debug.WriteLine("Permission ble admin granted!");
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.BluetoothAdmin }, 4);
            }
            else
            {
                // Camera permission is not granted.
                System.Diagnostics.Debug.WriteLine("Permission ble admin not granted!");
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.BluetoothAdmin }, 4);
            }
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
                //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        // picture picker part
        // Field, property, and method for Picture Picker
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (intent != null))
                {
                    Android.Net.Uri uri = intent.Data;
                    Stream stream = ContentResolver.OpenInputStream(uri);

                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(stream);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }
    }
}