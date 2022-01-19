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

        private string targetHttpPosString;//ссылка полученая из QR кода
        private readonly char[] specialSplitSymbol = new char[] { '|'};//Символ разделения стоки, входная строка точно делится символом | !!!!!!!
        private string googleScript = "https://script.google.com/macros/s/AKfycbz72RABZJTnMsxxODSvau9Ab867uxawrdVZ69kjXF5t1lsudlytD6WJh-QjeJtjGrN2qA/exec";

        public MainPage()
        {
            InitializeComponent();
            SetTapGesture();
        }

        protected override void OnAppearing()
        {
            TestInternetConnection();
        }

        private void SetTapGesture()
        {//устанваливаем двойное нажатие по метки для открытия ссылки на обьект из гугл таблицы
            TapGestureRecognizer Tapper = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 2//количество нажатий
            };

            Tapper.Tapped += (s, e) =>
              {
                  //Open URL
                  OpenULRByTap();
              };
            ScanResultEntry.GestureRecognizers.Add(Tapper);//Привязываем таппер к тексту с резульатом скана
        }

        private async void OpenULRByTap()
        {//пытаемся открыть ссылку, полученую в QR коде
            try
            {
                if (targetHttpPosString.Length > 0)
                {
                    string url = HttpUtility.UrlDecode(targetHttpPosString);//конвертим в url
                    // Так как ссылаемся на ячейки из гугл таблиц необходимо наличие программы Google Табицы и надичие доступа к таблице
                    await Browser.OpenAsync(new Uri(targetHttpPosString), BrowserLaunchMode.SystemPreferred);
                }
            }
            catch(Exception ex)
            {

            }  
        }

        private  void FastScanBtn_Clicked(object sender, EventArgs e)
        {
            //кнопка быстрого сканирования с записью в гугл таблицу результата
            StartQRScan();
        }
        private void DBBtn_Clicked(object sender, EventArgs e)
        {
            //кнопка открытия локальной базы данных записей
        }

        private async void StartQRScan()
        {
            //запускаем поиск QR кода на платформе android
            try
            {
                var scanner = DependencyService.Get<IQrScanningService>();//andriod
                var result = await scanner.ScanAsync();
                if (result != null)
                {
                    ScanResultEntry.Text = result;
                    SplitResultString(result);
                    await GetReqAsync(result);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private  async Task PostReqAsync(string sendingData)
        {
            //пост запрос для google API, пока не используется 
            WebRequest request = WebRequest.Create(googleScript);
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
            //работает стабильно, протестировано с google API
            var currentConnect = Connectivity.NetworkAccess;
            if (currentConnect == NetworkAccess.None)
            {
                EnternetStatusLabel.Text = "Нет подключения к сети, запись в локальную базу!";
            }
            else if (currentConnect == NetworkAccess.Internet)
            {
                //Используется GoogleAppsScript для внесения данных в таблицу, пока хватает этого

                string googleAPI = googleScript;
                googleAPI = googleScript + "?sdata=";
                string scanResult = HttpUtility.UrlEncode(sendingData);
                string result = string.Format("{0}{1}", googleAPI, scanResult);
                WebRequest request = WebRequest.Create(result);
                WebResponse response = await request.GetResponseAsync();

                response.Close();

                EnternetStatusLabel.Text = "Отправлено в таблицу!\n Двойное нажатие по тексту откроет онлайн таблицу";
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

        private void SplitResultString(string str)
        {
            //Разбитие текста на составляющие 1-URL в таблице 2-... Пока так
            string[] splitedStrings = str.Split(specialSplitSymbol);
            targetHttpPosString = splitedStrings[0];
        }
    }
}
