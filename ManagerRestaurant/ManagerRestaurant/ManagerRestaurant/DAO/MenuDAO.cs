using ManagerRestaurant.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerRestaurant.DAO
{
    public class MenuDAO
    {
        private static MenuDAO instance;

        internal static MenuDAO Instance
        {
            get { if (instance == null) instance = new MenuDAO(); return instance; }
            private set => instance = value;
        }

        public MenuDAO() { }

        public List<Menu> GetListMenuOfTable(int id)
        {
            List<Menu> listMenu = new List<Menu>();

            string query = "Select Food.name, count, price, price*count as totalPrice From Food, Bill, BillInfo, TableInfo Where Food.id = BillInfo.idFood And Bill.id = BillInfo.idBill And Bill.idTable = TableInfo.id And statusBill = 0 And Bill.idTable = " + id +"; ";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach(DataRow item in data.Rows)
            {
                Menu menu = new Menu(item);
                listMenu.Add(menu);
            }

            return listMenu;
        }

        public List<Menu> GetDetailBillOfBill(int id)
        {
            List<Menu> listMenu = new List<Menu>();

            string query = "Select Food.name, count, price, price*count as totalPrice From Food, Bill, BillInfo, TableInfo Where Food.id = BillInfo.idFood And Bill.id = BillInfo.idBill And Bill.idTable = TableInfo.id And Bill.id = " + id + ";";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Menu menu = new Menu(item);
                listMenu.Add(menu);
            }

            return listMenu;
        }
    }
}
