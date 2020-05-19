using ManagerRestaurant.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerRestaurant.DAO
{
    public class FoodDAO
    {
        private static FoodDAO instance;

        public static FoodDAO Instance
        {
            get { if (instance == null) instance = new FoodDAO(); return instance; }
            private set => instance = value;
        }

        public FoodDAO() { }

        public List<Food> GetFoodByCategoryId(int id)
        {
            List<Food> listFood = new List<Food>();

            string query = "Select * From Food Where idCategory = " + id;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                listFood.Add(food);
            }

            return listFood;
        }

        public DataTable GetListFood()
        {
            return DataProvider.Instance.ExecuteQuery("Exec USP_GetListFood");
        }

        public DataTable SearchFoodByName(string name)
        {
            string query = string.Format("Select Food.id as [ID], Food.name as [Tên món], FoodCategory.name as [Thể loại], price as [Đơn giá] From Food, FoodCategory Where Food.idCategory = FoodCategory.id and dbo.fuConvertToUnsign(Food.name) like N'%' + dbo.fuConvertToUnsign(N'{0}') + '%'", name);

            return DataProvider.Instance.ExecuteQuery(query);
        }

        public bool InsertFood(string name, int idCat, float price)
        {
            string query = string.Format("Insert Food(name, idCategory, price) Values (N'{0}', {1}, {2})", name, idCat, price);
            object check = DataProvider.Instance.ExecuteScalar(string.Format("Select * From Food Where name = N'{0}' and idCategory = {1} and price = {2}", name, idCat, price));
            if (check != null)
            {
                return false;
            }
            else
            {
                int rs = DataProvider.Instance.ExecuteNonQuery(query);

                return rs > 0;
            }
        }

        public bool UpdateFood(int id, string name, int idCat, float price)
        {
            string query = string.Format("Update Food Set name = N'{0}', idCategory = {1}, price = {2} Where id = {3}", name, idCat, price, id);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;            
        }

        public bool DeleteFood(int idFood)
        {
            BillInfoDAO.Instance.DeleteBillInfoByIdFood(idFood);
            string query = string.Format("Delete Food Where id = {0}", idFood);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }

        public void DeleteFoodById(int id)
        {
            DataProvider.Instance.ExecuteQuery("Delete Food Where id = " + id);
        }
    }
}
