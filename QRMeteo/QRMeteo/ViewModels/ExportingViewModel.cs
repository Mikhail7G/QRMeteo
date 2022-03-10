﻿
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
                data.Values.Add(new List<string>() { item.Name, item.InventoryNumber,item.LocationItem});
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
