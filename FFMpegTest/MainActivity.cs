using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.IO;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using Android.Views;
using System.Threading.Tasks;
using System;

namespace FFMpegTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            RequestPermission();

            Button testButton = FindViewById<Button>(Resource.Id.TestButton);

            testButton.Click += async delegate
            {
                testButton.Enabled = false;
                try
                {
                    await DoTest();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.GetType().Name);
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    throw e;
                }
                finally
                {
                    testButton.Enabled = true;
                }
            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private void RequestPermission()
        {

            if (ShouldShowRequestPermissionRationale(Android.Manifest.Permission.WriteExternalStorage))
            {

            }
            else
            {
                RequestPermissions(new string[] { Android.Manifest.Permission.WriteExternalStorage }, 1);
            }
        }
        private async Task<bool> DoTest()
        {
            string ffmpegDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "FFmpeg");

            Directory.CreateDirectory(ffmpegDirectory);

            string mediaDirectory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "FFMpegTest");

            Directory.CreateDirectory(mediaDirectory);

            string videoFileName = "test.mp4";
            string videoFileFullName = Path.Combine(mediaDirectory, videoFileName);

            FFmpeg.ExecutablesPath = ffmpegDirectory;
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, FFmpeg.ExecutablesPath);

            CheckAndSetExecutable(FFmpeg.ExecutablesPath, FFmpeg.FFmpegExecutableName);
            CheckAndSetExecutable(FFmpeg.ExecutablesPath, FFmpeg.FFprobeExecutableName);

            if (File.Exists(videoFileFullName))
            {
                IMediaInfo videoMediaInfo = await MediaInfo.Get(videoFileFullName);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("File doesn't exists.");
            }

            
            return true;

        }
        private void CheckAndSetExecutable(string directory, string fileName)
        {
            Java.IO.File myFile = new Java.IO.File(Path.Combine(directory, fileName));

            if (!myFile.CanExecute())
            {
                if (myFile.SetExecutable(true, false))
                {
                    System.Diagnostics.Debug.WriteLine("File is executable");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to make the file executable");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("File is executable");
            }
        }
    }
}