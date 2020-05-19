using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerRestaurant.DTO
{
    public class Food
    {
        private int id;
        private string name;
        private int idCatergory;
        private float price;
         
        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public int IdCatergory { get => idCatergory; set => idCatergory = value; }
        public float Price { get => price; set => price = value; }

        public Food(int id, string name, int idCategory, float price)
        {
            this.Id = id;
            this.Name = name;
            this.IdCatergory = idCategory;
            this.Price = price;
        }

        public Food(DataRow row)
        {
            this.Id = (int)row["id"];
            this.Name = row["name"].ToString();
            this.IdCatergory = (int)row["idCategory"];
            this.Price = (float)Convert.ToDouble(row["price"].ToString());
        }
    }
}
