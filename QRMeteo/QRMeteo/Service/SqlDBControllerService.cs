using System.Collections.Generic;
using QRMeteo.DBExcel;
using SQLite;

namespace QRMeteo.Service
{
    //работа с базой данных
    public class SqlDBControllerService
    {
        private SQLiteConnection sqlConnect;

        public SqlDBControllerService(string dbpath)
        {
            sqlConnect = new SQLiteConnection(dbpath);
            sqlConnect.CreateTable<InventoryObject>();
        }

        public IEnumerable<InventoryObject> GetItems()
        {
            return sqlConnect.Table<InventoryObject>().ToList();
        }

        public InventoryObject GetItem(int id)
        {
            return sqlConnect.Get<InventoryObject>(id);
        }

        public bool FindItemByHachCode(int code)//истина, если дубликат уже в БД
        {
            var conn = sqlConnect.Table<InventoryObject>().Where(v => v.HashCode == code).ToList();

            return conn.Count > 0;
        }

        public void ClearDataBase()
        {
            sqlConnect.DropTable<InventoryObject>();
            sqlConnect.CreateTable<InventoryObject>();
        }

        public int DeleteItmem(int id)
        {
            return sqlConnect.Delete<InventoryObject>(id);
        }

        public int SaveItem(InventoryObject obj)
        {
            if (obj.Id != 0) 
            {
                sqlConnect.Update(obj);
                return obj.Id;
            }
            else
            {
                return sqlConnect.Insert(obj);
            }
        }
    }
}
