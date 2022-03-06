using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace QRMeteo.DBExcel
{
    [Table("Inventory")]
    public class InventoryObject
    {

        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { set; get; }

        public string TargetHttpPosString { set; get; } //ссылка на бд в гугл таблице 
        public string PosInDBList { set; get; }         //позиция в бд для ручного поиска
        public string Name { set; get; }               //название объекта       
        public string InventoryNumber { set; get; }    //инвентарный номер
        public string LocationItem { set; get; }       //местонахождение объекта
        public string Comments { set; get; }           //комментарии к объекту

        public int HashCode { set; get; }              //для поиска дубликатов, код берется от http строки из гугл базы
    }
}
