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
using DocumentFormat.OpenXml;
using QRMeteo.Service;
using QRMeteo.ViewModels;
using QRMeteo.DBExcel;


namespace QRMeteo
{
    public partial class MainPage : ContentPage
    {

        private string targetHttpPosString;//ссылка полученая из QR кода
        private readonly char[] specialSplitSymbol = new char[] { '|'};//Символ разделения стоки, входная строка точно делится символом < | > 
        private readonly string googleScript = "https://script.google.com/macros/s/AKfycbz72RABZJTnMsxxODSvau9Ab867uxawrdVZ69kjXF5t1lsudlytD6WJh-QjeJtjGrN2qA/exec";

        private string badFormatString = null;//при получении неформатной строки

     
        private InventoryObject inventoryScanResult;
        ExportingViewModel Model = new ExportingViewModel();

        public MainPage()
        {
            inventoryScanResult = new InventoryObject();

            BindingContext = Model;
            Model.DuplicateNotify += PrintDuplicateData;

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
        private async void DBBtn_Clicked(object sender, EventArgs e)
        {
            LocalDBPage page = new LocalDBPage();
            await Navigation.PushAsync(page);
            page.SetViewModel(Model);
            Model.AddItemsToCollection(App.Database.GetItems());
  
        }
        private async void ExcelDB_Clicked(object sendr,EventArgs e)
        {
           await Model.OpenFile();
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
                    SplitResultString(result);
                    //await GetReqAsync(result);
                }
            }
            catch (Exception ex)
            {

            }
        }
        [Obsolete]
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
        [Obsolete]
        private  async Task GetReqAsync(string sendingData)
        {
            //работает стабильно, протестировано с google API
            var currentConnect = Connectivity.NetworkAccess;
            if (currentConnect == NetworkAccess.None)
            {
                InternetStatusLabel.Text = "Нет подключения к сети, запись в локальную базу!";
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

                InternetStatusLabel.Text = "Отправлено в таблицу!\n Двойное нажатие по тексту откроет онлайн таблицу";
            }
        }

        private void TestInternetConnection()
        {
            var currentConnect = Connectivity.NetworkAccess;
            if (currentConnect == NetworkAccess.Internet)
            {
                InternetStatusLabel.Text = "Подключение к сети!";
            }
            else if (currentConnect == NetworkAccess.None)
            {
                InternetStatusLabel.Text = "Нет подключения к сети!";
            }
        }

        private async void Settings_Clicked(object sender, EventArgs e)
        {
            await PickAndShow(PickOptions.Default);
        }

        async Task<FileResult> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    string filePath = result.FullPath;
                    ScanResultEntry.Text = filePath;
                }

                return result;
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        private void SplitResultString(string str)
        {
            //Разбитие текста на составляющие 1-URL в таблице 2-... Пока так
            string[] splitedStrings = str.Split(specialSplitSymbol);
            targetHttpPosString = splitedStrings[0];

            if (splitedStrings.Length > 4)
            {
                
                inventoryScanResult.TargetHttpPosString = splitedStrings[0];
                inventoryScanResult.PosInDBList = splitedStrings[1];
                inventoryScanResult.Name = splitedStrings[2];
                inventoryScanResult.InventoryNumber = splitedStrings[3];
                inventoryScanResult.LocationItem = splitedStrings[4];
                PrintReqData();

                //добавляем объект в базу данных
                InventoryObject tempObj = new InventoryObject()
                {
                    Name = inventoryScanResult.Name,
                    InventoryNumber = inventoryScanResult.InventoryNumber,
                    LocationItem = inventoryScanResult.LocationItem,
                    TargetHttpPosString = inventoryScanResult.TargetHttpPosString,
                    HashCode = inventoryScanResult.TargetHttpPosString.GetHashCode()
                };

                Model.AddItemToCollection(tempObj);  //ViewModel

            }
            else
            {
                badFormatString = str;
                PrintBadFormat();
            }
        }

        private void PrintDuplicateData(string mes)
        {
            FormattedString formattedString = new FormattedString();
            formattedString.Spans.Add(new Span
            {
                Text = mes + ":\n",
                ForegroundColor = Color.Aquamarine
            });
            formattedString.Spans.Add(new Span
            {
                Text = ScanResultEntry.Text
            });
            ScanResultEntry.FormattedText = formattedString;
        }

        private void PrintReqData()
        {
            ScanResultEntry.Text = String.Format("Ведомость №: {0} \n Название: {1} \n Инв номер: {2} \n Находится: {3}", inventoryScanResult.PosInDBList, inventoryScanResult.Name, inventoryScanResult.InventoryNumber, inventoryScanResult.LocationItem);
        }

        private void PrintBadFormat()
        {
            ScanResultEntry.Text = String.Format("Неверный формат входной строки! \n {0}", badFormatString);
        }

        private void Duplicate_Toggled(object sender, ToggledEventArgs e)
        {
            Model.WriteDuplicates = e.Value;
        }
    }
}
