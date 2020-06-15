using ManagerRestaurant.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerRestaurant.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;

        internal static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return instance; }
            private set => instance = value;
        }

        public TableDAO() { }

        public static int tableWidth = 100;
        public static int tableHeight = 100;

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("USP_TableList");

            foreach(DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }
            return tableList;
        }

        public void SwitchTable(int id1, int id2)
        {
            DataProvider.Instance.ExecuteQuery("USP_SwitchTable @idTable1 , @idTable2", new object[]{id1, id2});
        }

        public DataTable GetListTable()
        {
            return DataProvider.Instance.ExecuteQuery("Select id, name, statusTable From TableInfo Where type = 1");
        }

        public bool InsertTable(string name)
        {
            string query = string.Format("Insert TableInfo(name) Values (N'{0}')", name);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;           
        }

        public bool UpdateTable(int id, string name)
        {
            string query = string.Format("Update TableInfo Set name = N'{0}' Where id = {1}", name, id);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }

        public bool DeleteTable(int id)
        {
            string query = string.Format("Update TableInfo Set type = 0 Where id = {0} and statusTable = N'Trống'", id);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }

        /*
        public bool ReTable (int id)
        {
            string query = string.Format("Update TableInfo Set type = 1 Where id = {0}", id);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }*/
    }
}
