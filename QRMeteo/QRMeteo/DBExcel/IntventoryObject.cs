using System;
using System.Collections.Generic;
using System.Text;

namespace QRMeteo.DBExcel
{
    public class IntventoryObject
    {


        public string TargetHttpPosString { set; get; } //ссылка на бд в гугл таблице 
        public string PosInDBList { set; get; }         //позиция в бд для ручного поиска
        public string Name { set; get; }               //название объекта       
        public string InventoryNumber { set; get; }    //инвентарный номер
        public string LocationItem { set; get; }       //местонахождение объекта
    }
}
