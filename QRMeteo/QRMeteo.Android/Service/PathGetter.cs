using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QRMeteo.Service;
using Xamarin.Forms;

[assembly: Dependency(typeof(QRMeteo.Droid.Service.PathGetter))]

namespace QRMeteo.Droid.Service
{
    public class PathGetter : IPathGetter

    {
        [Obsolete]
        public string GetPath()
        {

            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
        }
    }
}