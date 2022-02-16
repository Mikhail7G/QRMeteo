using System;
using System.Collections.Generic;
using System.Text;

namespace QRMeteo.DBExcel
{
    //структура ячеек. Где header  - заголовки а Values значения
    public class ExcelStruct
    {
        public List<string> Header { set; get; } = new List<string>();
        public List<List<string>> Values { set; get; } = new List<List<string>>();
    }
}
