using ManagerRestaurant.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerRestaurant.DAO
{
    class AccountDAO
    {
        private static AccountDAO instance;

        internal static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return instance; }
            private set => instance = value;
        }

        public AccountDAO() { }

        public bool ToLogin(string userName, string password)
        {
            string query = "USP_Login @userName , @password ";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { userName , password });
            return result.Rows.Count > 0;
        }

        public Account GetAccountByUserName(string userName)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select * From Account Where userName = '" + userName + "'");
            foreach(DataRow item in data.Rows)
            {
                return new Account(item);
            }

            return null;
        }

        public bool UpdateAccount(string userName, string displayName, string password, string repassword)
        {
            int rs = DataProvider.Instance.ExecuteNonQuery("Exec USP_UpdateAccount @userName , @displayName , @password , @newPassword ", new object[] {userName, displayName, password, repassword });
            return rs > 0;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("Select userName, displayName, type From Account");
        }

        public bool InsertAccount(string userName, string displayName, int type)
        {
            string query = string.Format("Insert Account(userName, displayName, type) Values (N'{0}', N'{1}', {2})", userName, displayName, type);
            object check = DataProvider.Instance.ExecuteScalar(string.Format("Select * From Account Where userName = N'{0}'", userName));
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

        public bool UpdateAccount(string userName, string displayName, int type)
        {
            string query = string.Format("Update Account Set displayName = N'{0}', type = {1} Where userName = N'{2}'", displayName, type, userName);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }

        public bool DeleteAccount(string userName)
        {
            string query = string.Format("Delete Account Where userName = N'{0}'", userName);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }

        public bool ResetPassword(string userName)
        {
            string query = string.Format("Update Account Set password = N'1' Where userName = N'{0}'", userName);
            int rs = DataProvider.Instance.ExecuteNonQuery(query);

            return rs > 0;
        }
    }
}
