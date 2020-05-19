using ManagerRestaurant.DAO;
using ManagerRestaurant.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManagerRestaurant
{
    public partial class AccountInfo : Form
    {
        private Account loginAcc;

        public Account LoginAcc
        {
            get => loginAcc;
            set
            {
                loginAcc = value;
                ChangeAccount(loginAcc);
            }
        }

        public AccountInfo(Account acc)
        {
            InitializeComponent();

            LoginAcc = acc;
            
        }
        void ChangeAccount(Account account)
        {
            textBoxUserName.Text = LoginAcc.UserName;
            textBoxDisplayName.Text = LoginAcc.DisplayName;
        }

        void UpdateAccountInfo()
        {
            String userName = textBoxUserName.Text;
            String displayName = textBoxDisplayName.Text;
            String password = textBoxPassword.Text;
            String newPassword = textBoxNewPassword.Text;
            String rePassword = textBoxRePassword.Text;

            if (!newPassword.Equals(rePassword))
            {
                MessageBox.Show("Nhập lại mật khẩu không trùng khớp", "Thông báo");
            }
            else
            {
                if(AccountDAO.Instance.UpdateAccount(userName, displayName, password, newPassword))
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo");
                    if(updateAccount != null)
                    {
                        updateAccount(this, new AccountEvent(AccountDAO.Instance.GetAccountByUserName(userName)));
                    }
                }
                else
                {
                    MessageBox.Show("Mật khẩu không đúng!", "Thông báo");
                }
            }

        }

        private event EventHandler<AccountEvent> updateAccount;
        public event EventHandler<AccountEvent> UpdateAccount
        {
            add { updateAccount += value; }
            remove { updateAccount -= value; }
        }

        public class AccountEvent : EventArgs
        {
            private Account acc;
            public Account Acc { get => acc; set => acc = value; }

            public AccountEvent(Account acc)
            {
                this.Acc = acc;
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccountInfo();
        }
    }
}
