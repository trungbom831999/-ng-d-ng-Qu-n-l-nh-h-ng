using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerRestaurant.DAO
{
    public class DataProvider
    {
        private static DataProvider instance;
        public static DataProvider Instance
        {
            get { if (instance == null) instance = new DataProvider(); return instance; }
            private set => instance = value;
        }

        private DataProvider() { }

        private string connectionString = "Data Source=DESKTOP-4H2CDN2;Initial Catalog=QLNhaHang;Integrated Security=True"; //đường dẫn kết nối sql        

        public DataTable ExecuteQuery(string query, object[] parameter = null) //lấy data
        {
            DataTable data = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString)) //kết nối
            { 

                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                
                if(parameter != null)
                {
                    string[] listPara = query.Split(' '); //tách chuỗi 
                    int i = 0;
                    foreach(string item in listPara)
                    {
                        if (item.Contains('@')) //kiểm tra chuỗi có @
                        {
                            command.Parameters.AddWithValue(item, parameter[i]);
                            i++;
                        }
                    }
                }

                SqlDataAdapter adapter = new SqlDataAdapter(command); //trung gian lấy dữ liệu ra
                adapter.Fill(data);

                connection.Close();
            }

            return data;
        }

        public int ExecuteNonQuery(string query, object[] parameter = null) //số dòng thêm vào db thành công
        {
            int data = 0;

            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {

                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' '); 
                    int i = 0;
                    foreach (string item in listPara)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, parameter[i]);
                            i++;
                        }
                    }
                }

                data = command.ExecuteNonQuery(); // insert into ...

                connection.Close();
            }

            return data;
        }

        public object ExecuteScalar(string query, object[] parameter = null) //số lượng data trả ra
        {
            object data = 0;

            using (SqlConnection connection = new SqlConnection(connectionString)) { 

                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' '); 
                    int i = 0;
                    foreach (string item in listPara)
                    {
                        if (item.Contains('@')) 
                        {
                            command.Parameters.AddWithValue(item, parameter[i]);
                            i++;
                        }
                    }
                }

                data = command.ExecuteScalar(); // select count * from ...

                connection.Close();
            }

            return data;
        }


    }
}
