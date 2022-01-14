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
using System.Threading;
using QRMeteo.Service;
using System.Threading.Tasks;
using ZXing.Mobile;
using Xamarin.Forms;


[assembly: Dependency(typeof(QRMeteo.Droid.Service.QrScanningService))]

namespace QRMeteo.Droid.Service
{
    public class QrScanningService : IQrScanningService
    {
        public async Task<string> ScanAsync()
        {
            var optionsDefault = new MobileBarcodeScanningOptions();
            var optionsCustom = new MobileBarcodeScanningOptions();

            var scanner = new MobileBarcodeScanner()
            {
                TopText = "Сканируем код",
                BottomText = "Ожидание",
            };

            try
            {
                var scanResult = await scanner.Scan(optionsCustom);
                return scanResult.Text;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}