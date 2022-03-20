using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;



namespace QRMeteo.DBExcel
{
    //Рбота с таблицами в формате xlsx
    public class ExcelServises
    {
        //локальная папка с базой
        public string AppFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string FilePath { set; get; }//полная ссылка на путь до файла

        //генератор ячеек, что в ней хранить и в каом формате
        private Cell ConstructCell(string value, CellValues dataTypes) =>
               new Cell()
               {
                   CellValue = new CellValue(value),
                   DataType = new EnumValue<CellValues>(dataTypes)
               };

        public string GenerateExcel(String fileName)//создается файлик с листом
        {
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");


            FilePath = Path.Combine(AppFolder, fileName);
            var document = SpreadsheetDocument.Create(Path.Combine(AppFolder, fileName), SpreadsheetDocumentType.Workbook);

            var wbPart = document.AddWorkbookPart();
            wbPart.Workbook = new Workbook();

            var part = wbPart.AddNewPart<WorksheetPart>();
            part.Worksheet = new Worksheet(new SheetData());

            var sheets = wbPart.Workbook.AppendChild //Рабочие листы
                (
                   new Sheets(
                            new Sheet()
                            {
                                Id = wbPart.GetIdOfPart(part),
                                SheetId = 1,
                                Name = "Inventory"
                            }
                        )
                );

            wbPart.Workbook.Save();
            document.Close();

            return FilePath;
        }

        //Вставка данных в таблицу, данные вносятся последовательно, начиная с последней пустой ячейки, построчно
        public void InsertDataIntoSheet(string fileName, string sheetName, ExcelStruct data)
        {
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            using (var document = SpreadsheetDocument.Open(fileName, true))
            {
                var wbPart = document.WorkbookPart;

                var workId = wbPart.Workbook.Descendants<Sheet>().First(s => s.Name == "Inventory").Id;  //Id первого листа с датой скана
                var wsParts = wbPart.GetPartById(workId) as WorksheetPart;                               //получаем компонент по его ID
                var worksheet = wsParts.Worksheet;                                                       //Получаем дочерний компонент
                var sheetData = worksheet.GetFirstChild<SheetData>();                                    //добавляем новые ячейки данных 

                foreach (var value in data.Values)
                {
                    var dataRow = sheetData.AppendChild(new Row());

                    foreach (var dataElement in value)
                    {
                        dataRow.Append(ConstructCell(dataElement, CellValues.String));
                    }
                }
                wbPart.Workbook.Save();
            }
        }

        //установка заголовков для таблицы только 1 раз при создании.
        public void SetHeaders(string fileName, string sheetName, ExcelStruct data)
        {
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            using (var document = SpreadsheetDocument.Open(fileName, true))
            {
                var wbPart = document.WorkbookPart;

                //Осуществляем поиск необходимого листа по ID, далее ищем в фаиле рабочую зону по найденному ID для получения ячеек
                var workId = wbPart.Workbook.Descendants<Sheet>().First(s => s.Name == "Inventory").Id;  //Id первого листа с датой скана
                var wsParts = wbPart.GetPartById(workId) as WorksheetPart;                               //получаем компонент по его ID
                var worksheet = wsParts.Worksheet;                                                       //Получаем дочерний компонент
                var sheetData = worksheet.GetFirstChild<SheetData>();                                    //добавляем новые ячейки заголовков 

                var row = sheetData.AppendChild(new Row());

                foreach (var header in data.Header)
                {
                    row.Append(ConstructCell(header, CellValues.String));
                }
                wbPart.Workbook.Save();
            }

        }
        
        //очистка ячеек таблицы
         public void ClearCells(string fileName, string sheetName)
        {
            using (var document = SpreadsheetDocument.Open(fileName, true))
            {
                WorkbookPart wbPart = document.WorkbookPart;
              
                //Осуществляем поиск необходимого листа по ID, далее ищем в фаиле рабочую зону по найденному ID для получения ячеек
                var workId = wbPart.Workbook.Descendants<Sheet>().First(s => s.Name == "Inventory").Id;  //Id первого листа с датой скана
                var wsParts = wbPart.GetPartById(workId) as WorksheetPart;                               //получаем компонент по его ID
                var worksheet = wsParts.Worksheet;                                                       //Получаем дочерний компонент
                var rows = worksheet.GetFirstChild<SheetData>().Elements<Row>();                         //получаем все ячейки на нужеом листе

                foreach (var r in rows.ToList())
                {
                    r.Remove();
                }

                wbPart.Workbook.Save();
            }
        }

    }
}
