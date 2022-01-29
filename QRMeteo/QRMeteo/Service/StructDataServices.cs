using System;
using System.Collections.Generic;
using System.Text;

namespace QRMeteo.Service
{
    public struct ResievedData
    {
        public string targetHttpPosString;//ссылка на бд в гугл таблице 
        public string posInDBList;        //позиция в бд для ручного поиска
        public string name;               //название объекта       
        public string inventoryNumber;    //инвентарный номер
        public string locationItem;       //местонахождение объекта
    };
}
