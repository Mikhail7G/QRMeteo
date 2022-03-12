
using QRMeteo.DBExcel;
using QRMeteo.Service;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QRMeteo.ViewModels
{
    public class ExportingViewModel
    {
        public bool WriteDuplicates { set; get; } = true;

        public ICommand ExportToExcelCommand { private set; get; }
        public ICommand DeleteExcelFileCommand { private set; get; }
        public ICommand ClearLocalDBCommand { private set; get; }

        private ExcelServises excelService;
        public ObservableCollection<InventoryObject> inventory;

        private string fileName;//строка названия фаила
        private string filepath;//гле находится база

        public delegate void InsertDataHandler(string message);
        public event InsertDataHandler DuplicateNotify;

        private const string INDEX_FORMULA = "Для поиска индекса в счетах: =ИНДЕКС('21'!A:A;ПОИСКПОЗ(A2;'21'!B:B;0))";
        private const string NAME_FORMULA = "Для поиска названия в счетах: =ВПР(A2;'21'!B:B;1;0)";



        public ExportingViewModel()
        {
            inventory = new ObservableCollection<InventoryObject>();
           
            ExportToExcelCommand = new Command(() => ExportToExcel());

            //DeleteExcelFileCommand = new Command(() => DeleteFile());INOP
            //ClearLocalDBCommand = new Command(() => ClearWiewList());

            excelService = new ExcelServises();

            fileName = "InventoryDB.xlsx";//{Guid.NewGuid()}

            GenerateNewFile();
        }

        //добавляем один объект из сканирования
        public void AddItemToCollection(InventoryObject inv)
        {
            inventory.Add(inv);//добавляем в viewList

            if (App.Database.FindItemByHachCode(inv.HashCode))//поиск дубликатов
            {
                DuplicateNotify?.Invoke("Обнаружен дубликат");
            }
            else
            {
                        
            }
            if(WriteDuplicates)
            App.Database.SaveItem(inv); //сохраняем в локальную базу
        }

        //добавляем все объекты из базы данных
        public void AddItemsToCollection(IEnumerable<InventoryObject> inv)
        {
            inventory.Clear();

            foreach (var obj in inv)
            {
                inventory.Add(obj);
            }
        }

        public void ClearWiewList()
        {
            inventory.Clear();
            App.Database.ClearDataBase();
        }

        private void GenerateNewFile()
        {
        
            if (File.Exists(Path.Combine(excelService.appFolder, fileName)))
            {
                filepath = Path.Combine(excelService.appFolder, fileName);
            }
            else
            {
                filepath = excelService.GenerateExcel(fileName);

                var data = new ExcelStruct
                {
                    Header = new List<string>() { "Название", "Инвентарный номер", "Местонахождение", "Количество", "Номер в ведомости", "Название ", "Инв Номер",
                                                   INDEX_FORMULA, NAME_FORMULA }
                };

                excelService.SetHeaders(filepath, "Inventory", data);
            }

        }

        public void DeleteFile()
        {
            if (File.Exists(Path.Combine(excelService.appFolder, fileName)))
            {
                filepath = Path.Combine(excelService.appFolder, fileName);
                File.Delete(filepath);
                GenerateNewFile();
                // excelService.ClearCells(filepath, "Inventory");

            }
        }

        void ExportToExcel()
        {
            var data = new ExcelStruct();

            foreach (var item in inventory)
            {
                data.Values.Add(new List<string>() { item.Name, item.InventoryNumber, item.LocationItem, item.Quantity});
            }

            excelService.InsertDataIntoSheet(filepath, "Inventory", data);
        }

         public async Task OpenFile()
        {
            if (File.Exists(Path.Combine(excelService.appFolder, fileName)))
            {
                filepath = Path.Combine(excelService.appFolder, fileName);
                await Launcher.OpenAsync(new OpenFileRequest()
                {
                    File = new ReadOnlyFile(filepath)
                });
            }
        }
    }
}
