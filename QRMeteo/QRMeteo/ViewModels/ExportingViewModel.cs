
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
        public ICommand ExportToExcelCommand { private set; get; }
        private ExcelServises excelService;
        public ObservableCollection<IntventoryObject> inventory;

        private string fileName;//строка названия фаила
        private string filepath;//гле находится база


        public ExportingViewModel()
        {
            inventory = new ObservableCollection<IntventoryObject>();
            //{
            //   new IntventoryObject{ Name ="dddd", InventoryNumber = "12345"},
            //   new IntventoryObject{ Name ="aaaa", InventoryNumber = "3421"}
            //};

            ExportToExcelCommand = new Command(() => ExportToExcel());
            excelService = new ExcelServises();

            fileName = "InventoryDB.xlsx";//{Guid.NewGuid()}

            GenerateNewFile();
        }

        public void AddItemToCollection(IntventoryObject inv)
        {
            inventory.Add(inv);
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
                    Header = new List<string>() { "Название", "Инвентарный номер", "Местонахождение" }
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
            }
        }

        void ExportToExcel()
        {
            var data = new ExcelStruct();

            foreach (var item in inventory)
            {
                data.Values.Add(new List<string>() { item.Name, item.InventoryNumber,item.LocationItem });
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
