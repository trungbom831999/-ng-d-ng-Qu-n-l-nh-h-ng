using ManagerRestaurant.DAO;
using ManagerRestaurant.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ManagerRestaurant.AccountInfo;
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace ManagerRestaurant
{
    public partial class TableManager : Form
    {
        private Account loginAcc;

        public Account LoginAcc {
            get => loginAcc;
            set
            {
                loginAcc = value;
                ChangeAccount(loginAcc.Type);
            }
        }

        public TableManager(Account acc)
        {
            InitializeComponent();

            this.LoginAcc = acc;
            LoadTable();
            LoadCategory();
            LoadComboBoxTable(comboBoxSwitchTable);
        }

        #region Methods

        void LoadTable()
        {
            flowLayoutPanelTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                System.Windows.Forms.Button btn = new System.Windows.Forms.Button { Width = TableDAO.tableWidth, Height = TableDAO.tableHeight };
                flowLayoutPanelTable.Controls.Add(btn);
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                btn.Click += btn_Click;
                btn.Tag = item;

                if (item.Status == "Trống")
                    btn.BackColor = Color.Lavender;
                else
                    btn.BackColor = Color.OrangeRed;
            }
        }

        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            comboBoxCategory.DataSource = listCategory;
            comboBoxCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryId(id);
            comboBoxFood.DataSource = listFood;
            comboBoxFood.DisplayMember = "Name";
        }

        void ShowBill(int id)
        {
            listViewBill.Items.Clear();
            float totalPrice = 0; //tổng hóa đơn

            List<DTO.Menu> listMenu = MenuDAO.Instance.GetListMenuOfTable(id);

            foreach(DTO.Menu item in listMenu)
            {
                ListViewItem lvItem = new ListViewItem(item.FoodName.ToString());
                lvItem.SubItems.Add(item.Count.ToString());
                lvItem.SubItems.Add(item.Price.ToString());
                lvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;

                listViewBill.Items.Add(lvItem);
            }
            textBoxTotalPrice.Text = totalPrice.ToString("#,000");
        }

        void LoadComboBoxTable(ComboBox comboBox)
        {
            comboBox.DataSource = TableDAO.Instance.LoadTableList();
            comboBox.DisplayMember = "Name";
        }

        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thongTinToolStripMenuItem.Text += " " + LoginAcc.DisplayName;
        }

        /*bản 1
        private void ExportBill(int id)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "|*.xls", ValidateNames = true })
            {
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                    Workbook wb = app.Workbooks.Add(XlSheetType.xlWorksheet);
                    Worksheet ws = (Worksheet)app.ActiveSheet;
                    app.Visible = false;
                    ws.Cells[1, 1] = "Tên món";
                    ws.Cells[1, 2] = "SL";
                    ws.Cells[1, 3] = "Đơn giá";
                    ws.Cells[1, 4] = "Tổng";
                    int i = 2;
                    foreach(ListViewItem item in listViewBill.Items)
                    {
                        ws.Cells[i, 1] = item.SubItems[0].Text;
                        ws.Cells[i, 2] = item.SubItems[1].Text;
                        ws.Cells[i, 3] = item.SubItems[2].Text;
                        ws.Cells[i, 4] = item.SubItems[3].Text;
                        i++;
                    }
                    wb.SaveAs(string.Format("E:\\Project II(hết series)\\ManagerRestaurant\\ManagerRestaurant\\Bill\\{0}.xls", id), XlFileFormat.xlWorkbookDefault, Type.Missing,Type.Missing,true,false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                    //wb.Save();
                    //wb.SaveAs(string.Format("E:\\Project II(hết series)\\ManagerRestaurant\\ManagerRestaurant\\Bill\\{0}.xls", id));
                    app.Quit();
                    MessageBox.Show("Save Bill successful!");
                }
            }
        }
        */

        private void ExportBill(int id)
        {
            try
            {
                string[] st = new string[listViewBill.Columns.Count];
                DirectoryInfo di = new DirectoryInfo(@"Bill\");
                if (di.Exists == false)
                    di.Create();
                FileStream fs = new FileStream(@"Bill\" + id + ".xls", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Encoding.Unicode)
                {
                    AutoFlush = true
                };

                int rowIndex = 1;
                int row = 0;
                string st1 = "";
                for (row = 0; row < listViewBill.Items.Count; row++)
                {
                    if (rowIndex <= listViewBill.Items.Count)
                        rowIndex++;
                    st1 = "";
                    for (int col = 0; col < listViewBill.Columns.Count; col++)
                    {
                        st1 = st1 + listViewBill.Items[row].SubItems[col].Text.ToString() + "\t";
                    }
                    sw.WriteLine(st1);
                }
                sw.Close();
                FileInfo fil = new FileInfo(@"Bill\" + id + ".xls");
                //if (fil.Exists == true)
                   //MessageBox.Show("Process Completed", "Export to Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
            }
        }

            #endregion

            #region Events

            private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountInfo a = new AccountInfo(LoginAcc);
            a.UpdateAccount += a_UpdateAccount;
            a.ShowDialog();
        }

        private void a_UpdateAccount(object sender, AccountEvent e)
        {
            thongTinToolStripMenuItem.Text = "Thông tin tài khoản " + e.Acc.DisplayName;
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manager m = new Manager();
            m.loginAccount = loginAcc;
            m.InsertFood += M_InsertFood;
            m.UpdateFood += M_UpdateFood;
            m.DeleteFood += M_DeleteFood;
            m.InsertCat += M_InsertCat;
            m.UpdateCat += M_UpdateCat;
            m.DeleteCat += M_DeleteCat;
            m.ReCat += M_ReCat;
            m.DeletePermanCat += M_DeletePermanCat;
            m.InsertTab += M_InsertTab;
            m.UpdateTab += M_UpdateTab;
            m.DeleteTab += M_DeleteTab;
            m.ReTab += M_ReTab;
            m.ShowDialog();
        }

        private void M_DeletePermanCat(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).Id);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
            LoadTable();
        }

        private void M_ReTab(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboBoxTable(comboBoxSwitchTable);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
            //listViewBill.Controls.Clear();
        }

        private void M_DeleteTab(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboBoxTable(comboBoxSwitchTable);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
            //listViewBill.Controls.Clear();
        }

        private void M_UpdateTab(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboBoxTable(comboBoxSwitchTable);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
            //listViewBill.Controls.Clear();
        }

        private void M_InsertTab(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboBoxTable(comboBoxSwitchTable);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
            //listViewBill.Controls.Clear();
        }

        private void M_ReCat(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).Id);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
            LoadTable();
        }

        private void M_DeleteCat(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).Id);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
            LoadTable();
        }

        private void M_UpdateCat(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).Id);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
        }

        private void M_InsertCat(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).Id);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
        }

        private void M_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).Id);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
            LoadTable();
        }

        private void M_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).Id);
            if(listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
        }

        private void M_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).Id);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).Id);
        }

        private void btn_Click(object sender, EventArgs e)
        {
            int tableId = ((sender as System.Windows.Forms.Button).Tag as Table).Id;
            listViewBill.Tag = (sender as System.Windows.Forms.Button).Tag;
            textBoxTableName.Text = ((sender as System.Windows.Forms.Button).Tag as Table).Name;
            ShowBill(tableId);
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;

            Category category = cb.SelectedItem as Category;
            id = category.Id;

            LoadFoodListByCategoryID(id);
        }

        private void buttonAddFood_Click(object sender, EventArgs e)
        {
            Table table = listViewBill.Tag as Table;
            
            if (table == null)
            {
                MessageBox.Show("Vui lòng chọn bàn", "Thông báo");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIdByTableId(table.Id);
            int idFood = (comboBoxFood.SelectedItem as Food).Id;
            int count = (int)numericCountFood.Value;
            int check = 1;

            if (count <= 0)
            {
                string nameFood = (comboBoxFood.SelectedItem as Food).Name;
                List<DTO.Menu> listMenu = MenuDAO.Instance.GetListMenuOfTable(table.Id);
                int countFood = 0;
                foreach (DTO.Menu item in listMenu)
                {
                    string nameCheck = item.FoodName.ToString();
                    countFood++;
                    
                    if (String.Compare(nameCheck, nameFood, false) != 0)
                    {
                        check = 0;
                    }
                    else
                    {
                        check = 1;
                        break;
                    }
                }
                if (countFood == 0 || count == 0)
                {
                    check = 0;
                }
            }

            if (check == 1)
            {
                if (idBill == -1) //Bill mới
                {
                    BillDAO.Instance.InsertBill(table.Id);
                    BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxId(), idFood, count);
                    ShowBill(table.Id);
                    LoadTable();
                }
                else
                {
                    BillInfoDAO.Instance.InsertBillInfo(idBill, idFood, count);
                    ShowBill(table.Id);
                    LoadTable();
                }
            }
        }

        private void buttonCheckOut_Click(object sender, EventArgs e)
        {
            Table table = listViewBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Vui lòng chọn bàn", "Thông báo");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIdByTableId(table.Id);

            if (idBill != -1)
            {
                float totalPrice = 0; //tổng hóa đơn
                int countFood = 0;

                List<DTO.Menu> listMenu = MenuDAO.Instance.GetListMenuOfTable(table.Id);

                foreach (DTO.Menu item in listMenu)
                {
                    countFood++;
                    totalPrice += item.TotalPrice;
                }
                if (countFood > 0)
                {
                    if (MessageBox.Show(string.Format("Bạn muốn thanh toán {0}\nTổng tiền: {1}", table.Name, textBoxTotalPrice.Text), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        ExportBill(idBill);
                        BillDAO.Instance.CheckOut(idBill, totalPrice);
                        ShowBill(table.Id);
                        LoadTable();
                    }
                }
            }

        }

        private void buttonSwitchTable_Click(object sender, EventArgs e)
        {
            int id1 = (listViewBill.Tag as Table).Id;
            int id2 = (comboBoxSwitchTable.SelectedItem as Table).Id;

            if (MessageBox.Show(string.Format("Bạn chắc chắn chuyển bàn {0} sang bàn {1}", (listViewBill.Tag as Table).Name, (comboBoxSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);
            }
            
            LoadTable();
            ShowBill(id1);
        }
        #endregion

    }
}
