using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using QRMeteo.Service;




namespace QRMeteo
{


    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            TestInternetConnection();
        }

        private async void FastScanBtn_Clicked(object sender, EventArgs e)
        {

            try
            {
                var scanner = DependencyService.Get<IQrScanningService>();
                var result = await scanner.ScanAsync();
                if (result != null)
                {
                    ScanResultEntry.Text = result;
                    await GetReqAsync(result);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void DBBtn_Clicked(object sender, EventArgs e)
        {
           
        }

        private  async Task PostReqAsync(string sendingData)
        {
            WebRequest request = WebRequest.Create("https://script.google.com/macros/s/AKfycbzCD-6i508skk0U2hl1p7tZP1lQQ7RPSlt7vaKHgRSV_ZIM8VRJMctBethmJ0evkz6c/exec");
            request.Method = "POST";

            string scanResult = HttpUtility.UrlEncode(sendingData);
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(scanResult);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse response = await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {

                }
            }
            response.Close();
        }

        private  async Task GetReqAsync(string sendingData)
        {
            var currentConnect = Connectivity.NetworkAccess;
            if (currentConnect == NetworkAccess.None)
            {
               
            }
            else if (currentConnect == NetworkAccess.Internet)
            {

                string googleAPI = "https://script.google.com/macros/s/AKfycbzCD-6i508skk0U2hl1p7tZP1lQQ7RPSlt7vaKHgRSV_ZIM8VRJMctBethmJ0evkz6c/exec?sdata=";
                string scanResult = HttpUtility.UrlEncode(sendingData);
                string result = string.Format("{0}{1}", googleAPI, scanResult);
                WebRequest request = WebRequest.Create(result);
                WebResponse response = await request.GetResponseAsync();

                response.Close();
            }
        }

        private void TestInternetConnection()
        {
            var currentConnect = Connectivity.NetworkAccess;
            if (currentConnect == NetworkAccess.Internet)
            {
                EnternetStatusLabel.Text = "Подключение к сети!";
            }
            else if (currentConnect == NetworkAccess.None)
            {
                EnternetStatusLabel.Text = "Нет подключения к сети!";
            }

        }
    }
}
