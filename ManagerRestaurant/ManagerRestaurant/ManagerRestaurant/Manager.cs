using ManagerRestaurant.DAO;
using ManagerRestaurant.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace ManagerRestaurant
{
    public partial class Manager : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource billList = new BindingSource();
        BindingSource accountList = new BindingSource();
        BindingSource catList = new BindingSource();
        BindingSource tableList = new BindingSource();
        public Account loginAccount;

        public Manager()
        {
            InitializeComponent();
            Load();
        }
        #region Methods
        void Load()
        {
            dataGridViewFood.DataSource = foodList;
            dataGridViewBill.DataSource = billList;
            dataGridViewAccount.DataSource = accountList;
            dataGridViewCategory.DataSource = catList;
            dataGridViewTable.DataSource = tableList;

            LoadDateTimePicker();
            LoadListBillByDateAndPage(dateTimePickerCheckIn.Value, dateTimePickerCheckOut.Value, 1);
            LoadListFood();
            LoadCategoryIntoCombobox(comboBoxCategory);
            LoadAccount();
            LoadCat();
            LoadTable();

            DetailBill();
            AddFoodBinding();
            AddAccountBinding();
            AddCatBinding();
            AddTableBinding();
        }

        //Bill
        void LoadDateTimePicker()
        {
            DateTime today = DateTime.Now;
            dateTimePickerCheckIn.Value = new DateTime(today.Year, today.Month, 1);
            dateTimePickerCheckOut.Value = dateTimePickerCheckIn.Value.AddMonths(1).AddDays(-1);
        }

        void LoadListBillByDateAndPage(DateTime checkIn, DateTime checkOut, int page)
        {
            billList.DataSource = BillDAO.Instance.GetBillListByDateAndPage(checkIn, checkOut, page);
        }

        void DetailBill()
        {
            textBoxIdBill.DataBindings.Add(new Binding("Text", dataGridViewBill.DataSource, "id", true, DataSourceUpdateMode.Never));
        }

        /*bản cũ
        void ShowBill(int id)
        {
            listViewBill.Items.Clear();

            List<DTO.Menu> listMenu = MenuDAO.Instance.GetDetailBillOfBill(id);

            foreach (DTO.Menu item in listMenu)
            {
                ListViewItem lvItem = new ListViewItem(item.FoodName.ToString());
                lvItem.SubItems.Add(item.Count.ToString());
                lvItem.SubItems.Add(item.Price.ToString());
                lvItem.SubItems.Add(item.TotalPrice.ToString());

                listViewBill.Items.Add(lvItem);
            }
        }*/

        void ShowBill(int id)
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            path += string.Format(@"\Bill\{0}.xls", id);
            listViewBill.Items.Clear();
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(path);
            Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            for (int i = 0; i <= rowCount; i++)
            {
                try
                {
                    ListViewItem lvitem = new ListViewItem();
                    lvitem.Text = xlRange.Cells[i, 1].Value2.ToString();
                    lvitem.SubItems.Add(xlRange.Cells[i, 2].Value2.ToString());
                    lvitem.SubItems.Add(xlRange.Cells[i, 3].Value2.ToString());
                    lvitem.SubItems.Add(xlRange.Cells[i, 4].Value2.ToString());
                    listViewBill.Items.Add(lvitem);
                }
                catch (Exception ex)
                {
                }
            }
        }

        //Food
        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void AddFoodBinding()
        {
            textBoxNameFood.DataBindings.Add(new Binding("Text", dataGridViewFood.DataSource, "Tên món", true, DataSourceUpdateMode.Never));
            textBoxIdFood.DataBindings.Add(new Binding("Text", dataGridViewFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            comboBoxCategory.DataBindings.Add(new Binding("Text", dataGridViewFood.DataSource, "Thể loại", true, DataSourceUpdateMode.Never));
            numericUpDownPriceFood.DataBindings.Add(new Binding("Value", dataGridViewFood.DataSource, "Đơn giá", true, DataSourceUpdateMode.Never));
        }

        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "name";
        }

        System.Data.DataTable SearchFoodByName(string name)
        {
            return FoodDAO.Instance.SearchFoodByName(name);
        }

        //Account
        void AddAccountBinding()
        {
            textBoxNameAccount.DataBindings.Add(new Binding("Text", dataGridViewAccount.DataSource, "userName", true, DataSourceUpdateMode.Never));
            textBoxDisplayName.DataBindings.Add(new Binding("Text", dataGridViewAccount.DataSource, "displayName", true, DataSourceUpdateMode.Never));
            numericUpDownType.DataBindings.Add(new Binding("Value", dataGridViewAccount.DataSource, "type", true, DataSourceUpdateMode.Never));
        }

        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }

        void AddAccount(string userName, string displayName, int type)
        {
            if(AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Thêm tài khoản thành công!", "Thông báo");
            }
            else
            {
                MessageBox.Show(string.Format("Thêm tài khoản Không thành công!/nCó thể trùng tên đăng nhập"), "Thông báo");
            }

            LoadAccount();
        }

        void UpdateAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Sửa tài khoản thành công!", "Thông báo");
            }
            else
            {
                MessageBox.Show(string.Format("Sửa tài khoản Không thành công!/nKhông thể sửa tên đăng nhập"), "Thông báo");
            }

            LoadAccount();
        }

        void DeleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName)){
                MessageBox.Show("Không thể xóa chính bạn", "Thông báo");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Xóa tài khoản thành công!", "Thông báo");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản Không thành công!", "Thông báo");
            }

            LoadAccount();
        }

        void ResetPassword(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show(string.Format("Đặt lại mật khẩu thành công!\nMật khẩu của bạn là 1"), "Thông báo");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu Không thành công!", "Thông báo");
            }

            LoadAccount();
        }

        //Category
        void LoadCat()
        {
            catList.DataSource = CategoryDAO.Instance.GetListFoodCategory();
        }

        void AddCatBinding()
        {
            textBoxIdcategory.DataBindings.Add(new Binding("Text", dataGridViewCategory.DataSource, "id", true, DataSourceUpdateMode.Never));
            textBoxNameCategory.DataBindings.Add(new Binding("Text", dataGridViewCategory.DataSource, "name", true, DataSourceUpdateMode.Never));
            numericUpDownTypeCategory.DataBindings.Add(new Binding("Value", dataGridViewCategory.DataSource, "type", true, DataSourceUpdateMode.Never));
        }

        void AddCategory(string name, int type)
        {
            if (CategoryDAO.Instance.InsertCategory(name, type))
            {
                MessageBox.Show("Thêm thành công!", "Thông báo");
                if (insertCat != null)
                {
                    insertCat(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show(string.Format("Thêm Không thành công!\nCó thể trùng tên"), "Thông báo");
            }

            LoadCat();
            LoadCategoryIntoCombobox(comboBoxCategory);
            LoadListFood();
        }

        void UpdateCategory(int id, string name, int type)
        {
            if (CategoryDAO.Instance.UpdateCategory(id, name, type))
            {
                MessageBox.Show("Sửa thành công!", "Thông báo");
                if (updateCat != null)
                {
                    updateCat(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Sửa Không thành công!", "Thông báo");
            }

            LoadCat();
            LoadCategoryIntoCombobox(comboBoxCategory);
            LoadListFood();
        }

        void DeleteCategory(int id)
        {
            if (CategoryDAO.Instance.DeleteCategory(id))
            {
                MessageBox.Show("Tạm xóa thành công!", "Thông báo");
                if (deleteCat != null)
                {
                    deleteCat(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Tạm xóa Không thành công!", "Thông báo");
            }

            LoadCat();
            LoadCategoryIntoCombobox(comboBoxCategory);
            LoadListFood();
        }

        void ReCategory(int id)
        {
            if (CategoryDAO.Instance.ReCategory(id))
            {
                MessageBox.Show("Hoàn tác thành công!", "Thông báo");
                if (reCat != null)
                {
                    reCat(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Hoàn tác Không thành công!", "Thông báo");
            }

            LoadCat();
            LoadCategoryIntoCombobox(comboBoxCategory);
            LoadListFood();
        }

        void DeletePermanCategory(int id)
        {
            if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn xóa thể loại này?\nHành động này sẽ xóa tất cả món ăn trong thể loại này"), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {                
                if (CategoryDAO.Instance.DeletePermanCategory(id))
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo");
                    if (deletePermanCat != null)
                    {
                        deletePermanCat(this, new EventArgs());
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Xóa Không thành công!"), "Thông báo");
                }
                LoadCat();
                LoadCategoryIntoCombobox(comboBoxCategory);
                LoadListFood();
            }
        }

        //Table
        void LoadTable()
        {
            tableList.DataSource = TableDAO.Instance.GetListTable();
        }

        void AddTableBinding()
        {
            textBoxIdTable.DataBindings.Add(new Binding("Text", dataGridViewTable.DataSource, "id", true, DataSourceUpdateMode.Never));
            textBoxNameTable.DataBindings.Add(new Binding("Text", dataGridViewTable.DataSource, "name", true, DataSourceUpdateMode.Never));
            textBoxStatusTable.DataBindings.Add(new Binding("Text", dataGridViewTable.DataSource, "statusTable", true, DataSourceUpdateMode.Never));
        }

        void AddTable (string name)
        {
            if (TableDAO.Instance.InsertTable(name))
            {
                MessageBox.Show("Thêm bàn thành công!", "Thông báo");
                if (insertTab != null)
                {
                    insertTab(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show(string.Format("Thêm bàn Không thành công!\nCó thể trùng tên"), "Thông báo");
            }

            LoadTable();
        }

        void UpdateTable(int id, string name)
        {
            if (TableDAO.Instance.UpdateTable(id, name))
            {
                MessageBox.Show("Sửa thông tin bàn thành công!", "Thông báo");
                if (updateTab != null)
                {
                    updateTab(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Sửa thông tin bàn Không thành công!", "Thông báo");
            }

            LoadTable();     
        }

        void DeleteTable(int id)
        {
            if (TableDAO.Instance.DeleteTable(id))
            {
                MessageBox.Show("Xóa bàn thành công!", "Thông báo");
                if (deleteTab != null)
                {
                    deleteTab(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show(string.Format("Xóa bàn Không thành công!\nKiểm tra bàn còn Trống hay không?"), "Thông báo");
            }

            LoadTable();
        }

        /*
        void ReTable(int id)
        {
            if (TableDAO.Instance.ReTable(id))
            {
                MessageBox.Show("Hoàn tác bàn thành công!", "Thông báo");
                if (reTab != null)
                {
                    reTab(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Hoàn tác bàn Không thành công!", "Thông báo");
            }

            LoadTable();
        }*/
        #endregion

        #region Events
        //Bill
        private void buttonShowBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDateAndPage(dateTimePickerCheckIn.Value, dateTimePickerCheckOut.Value, 1);
            textBoxBillPage.Text = "1";
            listViewBill.Items.Clear();
        }

        private void buttonFirstBill_Click(object sender, EventArgs e)
        {
            textBoxBillPage.Text = "1";
        }

        private void buttonLastBill_Click(object sender, EventArgs e)
        {
            int sumRecord = BillDAO.Instance.GetCountBillByDate(dateTimePickerCheckIn.Value, dateTimePickerCheckOut.Value);
            int lastPage = sumRecord / 10;
            if (sumRecord % 10 > 0)
            {
                lastPage++;
            }
            textBoxBillPage.Text = lastPage.ToString();
        }

        private void textBoxBillPage_TextChanged(object sender, EventArgs e)
        {
            billList.DataSource = BillDAO.Instance.GetBillListByDateAndPage(dateTimePickerCheckIn.Value, dateTimePickerCheckOut.Value, Convert.ToInt32(textBoxBillPage.Text));
        }

        private void buttonPrevBill_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(textBoxBillPage.Text);
            if (page > 1) page--;

            textBoxBillPage.Text = page.ToString();
        }

        private void buttonNextBill_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(textBoxBillPage.Text);
            int sumRecord = BillDAO.Instance.GetCountBillByDate(dateTimePickerCheckIn.Value, dateTimePickerCheckOut.Value);
            int lastPage = sumRecord / 10;
            if (sumRecord % 10 > 0)
            {
                lastPage++;
            }
            if (page < lastPage) page++;

            textBoxBillPage.Text = page.ToString();
        }

        //Food
        private void buttonShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private void textBoxIdBill_TextChanged(object sender, EventArgs e)
        {
            int id = -1;
            if(Int32.TryParse(textBoxIdBill.Text, out id))
                ShowBill(id);
        }

        private void buttonAddFood_Click(object sender, EventArgs e)
        {
            string name = textBoxNameFood.Text;
            int idCategory = (comboBoxCategory.SelectedItem as Category).Id;
            float price = (float)numericUpDownPriceFood.Value;

            if (FoodDAO.Instance.InsertFood(name, idCategory, price))
            {
                MessageBox.Show("Thêm món thành công!", "Thông báo");
                LoadListFood();
                if (insertFood != null)
                {
                    insertFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show(string.Format("Thêm món Không thành công!\nMón ăn có thể trùng"), "Thông báo");
            }
        }

        private void buttonEditFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(textBoxIdFood.Text);
            string name = textBoxNameFood.Text;
            int idCategory = (comboBoxCategory.SelectedItem as Category).Id;
            float price = (float)numericUpDownPriceFood.Value;

            if (FoodDAO.Instance.UpdateFood(id, name, idCategory, price))
            {
                MessageBox.Show("Sửa thành công!", "Thông báo");
                LoadListFood();
                if (updateFood != null)
                {
                    updateFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show(string.Format("Sửa Không thành công!"), "Thông báo");
            }
        }

        private void buttonDeleteFood_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa món ăn này?", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                int id = Convert.ToInt32(textBoxIdFood.Text);
                if (FoodDAO.Instance.DeleteFood(id))
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo");
                    LoadListFood();
                    if (deleteFood != null)
                    {
                        deleteFood(this, new EventArgs());
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Xóa Không thành công!"), "Thông báo");
                }
            }
        }

        private void buttonSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(textBoxSearchFood.Text);
        }

        //Account
        private void buttonShowAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        private void buttonAddAccount_Click(object sender, EventArgs e)
        {
            string userName = textBoxNameAccount.Text;
            string displayName = textBoxDisplayName.Text;
            int type = (int)numericUpDownType.Value;

            AddAccount(userName, displayName, type);
        }

        private void buttonEditAccount_Click(object sender, EventArgs e)
        {
            string userName = textBoxNameAccount.Text;
            string displayName = textBoxDisplayName.Text;
            int type = (int)numericUpDownType.Value;

            UpdateAccount(userName, displayName, type);
        }

        private void buttonDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = textBoxNameAccount.Text;

            DeleteAccount(userName);
        }

        private void buttonResetPassword_Click(object sender, EventArgs e)
        {
            string userName = textBoxNameAccount.Text;

            ResetPassword(userName);
        }

        //Category
        private void buttonAddCategory_Click(object sender, EventArgs e)
        {
            string name = textBoxNameCategory.Text;
            int type = (int)numericUpDownTypeCategory.Value;
            AddCategory(name, type);
        }

        private void buttonDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(textBoxIdcategory.Text);
            int type = (int)numericUpDownTypeCategory.Value;
            if(type == 1)
            {
                DeleteCategory(id);
            }
            else if(type == 0)
            {
                ReCategory(id);
            }                
        }

        private void buttonEditCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(textBoxIdcategory.Text);
            string name = textBoxNameCategory.Text;
            int type = (int)numericUpDownTypeCategory.Value;
            UpdateCategory(id, name, type);
        }

        private void buttonShowCategory_Click(object sender, EventArgs e)
        {
            LoadCat();
        }

        private void buttonDeletePerman_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(textBoxIdcategory.Text);
            DeletePermanCategory(id);
        }

        //Table
        private void buttonAddTable_Click(object sender, EventArgs e)
        {
            string name = textBoxNameTable.Text;
            AddTable(name);
        }

        private void buttonDeleteTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(textBoxIdTable.Text);
                DeleteTable(id);        
        }

        private void buttonEditTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(textBoxIdTable.Text);
            string name = textBoxNameTable.Text;
            UpdateTable(id, name);
        }

        private void buttonShowTable_Click(object sender, EventArgs e)
        {
            LoadTable();
        }

        //Event Handler
        private event EventHandler insertFood;
        public event EventHandler InsertFood {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler insertCat;
        public event EventHandler InsertCat
        {
            add { insertCat += value; }
            remove { insertCat -= value; }
        }

        private event EventHandler updateCat;
        public event EventHandler UpdateCat
        {
            add { updateCat += value; }
            remove { updateCat -= value; }
        }

        private event EventHandler deleteCat;
        public event EventHandler DeleteCat
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler reCat;
        public event EventHandler ReCat
        {
            add { reCat += value; }
            remove { ReCat -= value; }
        }

        private event EventHandler deletePermanCat;
        public event EventHandler DeletePermanCat
        {
            add { deletePermanCat += value; }
            remove { deletePermanCat -= value; }
        }

        private event EventHandler insertTab;
        public event EventHandler InsertTab
        {
            add { insertTab += value; }
            remove { insertTab -= value; }
        }

        private event EventHandler updateTab;
        public event EventHandler UpdateTab
        {
            add { updateTab += value; }
            remove { updateTab -= value; }
        }

        private event EventHandler deleteTab;
        public event EventHandler DeleteTab
        {
            add { deleteTab += value; }
            remove { deleteTab -= value; }
        }

        private event EventHandler reTab;
        public event EventHandler ReTab
        {
            add { reTab += value; }
            remove { ReTab -= value; }
        }
        #endregion

        private void label18_Click(object sender, EventArgs e)
        {

        }
    }
}
