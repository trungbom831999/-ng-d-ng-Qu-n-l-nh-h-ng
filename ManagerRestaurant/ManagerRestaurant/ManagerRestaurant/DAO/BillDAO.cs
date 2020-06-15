using ManagerRestaurant.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerRestaurant.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;

        internal static BillDAO Instance
        {
            get { if (instance == null) instance = new BillDAO(); return instance; }
            private set => instance = value;
        }

        public BillDAO() { }

        public int GetUncheckBillIdByTableId(int id) //lấy billID, nếu thất bại trả về -1
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select * From Bill Where idTable = " + id + " And statusBill = 0");

            if(data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.Id;
            }
            return -1;
        }

        public void InsertBill(int id)
        {
            DataProvider.Instance.ExecuteQuery("Exec USP_InsertBill @idTable", new object[] { id });
        }

        public int GetMaxId()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar("Select Max(id) From Bill");
            }
            catch
            {
                return 1;
            }
        }

        public void CheckOut(int id, float totalPrice)
        {
            string query = "Update Bill Set dateCheckOut = Getdate(), statusBill = 1, totalPrice = " + totalPrice + " Where id = " + id;
            DataProvider.Instance.ExecuteQuery(query);
        }

        public DataTable GetBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery("Exec USP_GetListBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }

        public float SumBill(DateTime checkIn, DateTime checkOut)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Exec USP_GetListBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
            float sum = 0;
            foreach(DataRow item in data.Rows)
            {
                sum += (float)Convert.ToDouble(item["totalPrice"].ToString());
            }
            return sum;
        }

        public DataTable GetBillListByDateAndPage(DateTime checkIn, DateTime checkOut, int page)
        {
            return DataProvider.Instance.ExecuteQuery("Exec USP_GetListBillByDateAndPage @checkIn , @checkOut , @page", new object[] { checkIn, checkOut, page });
        }

        public int GetCountBillByDate(DateTime checkIn, DateTime checkOut)
        {
            return (int)DataProvider.Instance.ExecuteScalar("Exec USP_GetCountBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }
    }
}
