using ManagerRestaurant.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerRestaurant.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO instance;

        public static CategoryDAO Instance {
            get { if (instance == null) instance = new CategoryDAO(); return instance; }
            private set => instance = value;
        }

        public CategoryDAO() { }

        public List<Category> GetListCategory()
        {
            List<Category> listCategory = new List<Category>();

            string query = "Select * From FoodCategory Where type = 1";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach(DataRow item in data.Rows)
            {
                Category category = new Category(item);
                listCategory.Add(category);
            }

            return listCategory;
        }

        public DataTable GetListFoodCategory()
        {
            return DataProvider.Instance.ExecuteQuery("Select * From FoodCategory");
        }

        public bool InsertCategory(string name, int type)
        {
            string query = string.Format("Insert FoodCategory(name, type) Values (N'{0}', {1})", name, type);
            object check = DataProvider.Instance.ExecuteScalar("Select * From FoodCategory Where name = N'" + name + "'");
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

        public bool UpdateCategory(int id, string name, int type)
        {
            string query = string.Format("Update FoodCategory Set name = N'{0}', type = {1} Where id = {2}", name, type, id);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }

        public bool DeleteCategory(int id)
        {
            string query = string.Format("Update FoodCategory Set type = 0 Where id = {0}", id);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }

        public bool ReCategory(int id)
        {
            string query = string.Format("Update FoodCategory Set type = 1 Where id = {0}", id);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }

        public bool DeletePermanCategory(int id)
        {
            string query = "Select * From Food Where idCategory = " + id;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {                
                Food food = new Food(item);
                BillInfoDAO.Instance.DeleteBillInfoByIdFood(food.Id);
                FoodDAO.Instance.DeleteFoodById(food.Id);
            }

            int rs = DataProvider.Instance.ExecuteNonQuery("Delete FoodCategory Where id = " + id);

            return rs > 0;
        }
    }
}
