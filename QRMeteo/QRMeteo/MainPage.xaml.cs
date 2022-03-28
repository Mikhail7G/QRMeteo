using System;
using System.Net;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using QRMeteo.Service;
using QRMeteo.ViewModels;
using QRMeteo.DBExcel;


namespace QRMeteo
{
    public partial class MainPage : ContentPage
    {

        private string targetHttpPosString;//ссылка полученая из QR кода
        private readonly char[] specialSplitSymbol = new char[] { '|'};//Символ разделения стоки, входная строка точно делится символом < | > 
        private string badFormatString = null;//при получении неформатной строки
     
        private InventoryObject inventoryScanResult;
        public ExportingViewModel Model = new ExportingViewModel();

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
                    // Так как ссылаемся на ячейки из гугл таблиц необходимо наличие программы Google Табицы и надичие доступа к таблице
                    await Browser.OpenAsync(new Uri(targetHttpPosString), BrowserLaunchMode.SystemPreferred);
                }
            }
            catch(Exception ex)
            {

            }  
        }

        private void StartScanButtonClicked(object sender, EventArgs e)
        {
            //кнопка быстрого сканирования с записью в гугл таблицу результата
            StartQRScan();
        }

        private async void DataBaseButtonClicked(object sender, EventArgs e) //открытие окна локальной базы сканов
        {
            LocalDBPage page = new LocalDBPage();
            await Navigation.PushAsync(page);

            page.SetViewModel(Model);
            Model.AddItemsToCollection(App.Database.GetItems());
  
        }
        private async void ExcelButtonClicked(object sendr,EventArgs e)
        {
           if(await Model.OpenFile())
            {

            }
           else
            {
                SetTextResultLabel("Файл не найден, создайте или загрузите с хранилища");
            }
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
                }
            }
            catch (Exception ex)
            {

            }
        }
      
        private void TestInternetConnection()
        {
            var currentConnect = Connectivity.NetworkAccess;

            switch (currentConnect)
            {
                case NetworkAccess.Internet:
                    SetInternetLabelText("Подключение к сети!");
                    break;
                case NetworkAccess.None:
                    SetInternetLabelText("Нет подключения к сети!");
                    break;
            }
        }

        private void SetInternetLabelText(string text)
        {
            InternetStatusLabel.Text = text.Length > 0 ? text : "";
        }

        private async void FileLoaderButtonClicked(object sender, EventArgs e)
        {
            await PickAndShow(PickOptions.Default);
        }

        private void DuplicateToggled(object sender, ToggledEventArgs e)
        {
            Model.WriteDuplicates = e.Value;
        }

        async Task<FileResult> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    await result.OpenReadAsync();
                    string filePath = result.FullPath;
                    ScanResultEntry.Text = filePath;
                    Model.ImportFIle(filePath);
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
            SetFormattedTextResultLabel(formattedString);
        }

        private void SetTextResultLabel(string text)
        {
            if (text.Length > 0)
            {
                ScanResultEntry.Text = text;
            }
        }
        private void SetFormattedTextResultLabel(FormattedString text)
        {
            if (text.Spans.Count > 0)  
            {
                ScanResultEntry.FormattedText = text;
            }
        }

        private void PrintReqData()
        {
            ScanResultEntry.Text = String.Format("Ведомость №: {0} \n Название: {1} \n Инв номер: {2} \n Находится: {3}", inventoryScanResult.PosInDBList, inventoryScanResult.Name, inventoryScanResult.InventoryNumber, inventoryScanResult.LocationItem);
        }

        private void PrintBadFormat()
        {
            ScanResultEntry.Text = String.Format("Неверный формат входной строки! \n {0}", badFormatString);
        }
    }
}
