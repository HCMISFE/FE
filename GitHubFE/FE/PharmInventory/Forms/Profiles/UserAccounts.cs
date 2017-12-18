using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BLL;
using DevExpress.XtraEditors;
using System.Collections.Generic;

namespace PharmInventory.Forms.Profiles
{
    /// <summary>
    /// Interaction logic for UserAccounts
    /// </summary>
    public partial class UserAccounts : XtraForm
    {
      
        public UserAccounts()
        {
            InitializeComponent();
        }

        int _userId = 0;
        string dispMember = "Name";
        string valMember = "ID";
        /// <summary>
        /// Loads lookups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserAccounts_Load(object sender, EventArgs e)
        {
            PopulateUser();
        }

        /// <summary>
        /// Populates user related info
        /// </summary>
        private void PopulateUser()
        {
            User us = new User();
            DataTable dtUser = us.GetUsers();
            int count = 1;
            int col = 0;
            lstUsers.Items.Clear();
            foreach (DataRow dv in dtUser.Rows)
            {
                string[] str = { count.ToString(), dv["FullName"].ToString(), dv["Mobile"].ToString(), dv["UserName"].ToString(), dv["Type"].ToString() };
                ListViewItem lst = new ListViewItem(str) {Tag = dv["ID"]};
                if (col != 0)
                {
                    lst.BackColor = Color.FromArgb(233, 247, 248);
                    col = 0;
                }
                else
                    col++;
                lstUsers.Items.Add(lst);
                count++;
            }

            UserType uType = new UserType();
            uType.LoadAll();
            cboUserType.DataSource = uType.DefaultView;

            BLL.Type comUser = new BLL.Type();
            comUser.LoadAll();
           cbCommodity.Properties.DataSource = comUser.DefaultView;
            cbCommodity.Properties.DisplayMember = "Name";
            cbCommodity.Properties.ValueMember = "ID";

            Stores storeUser = new Stores();
            storeUser.LoadAll();
            cbStore.Properties.DataSource = storeUser.DefaultView;
            cbStore.Properties.DisplayMember = "StoreName";
            cbStore.Properties.ValueMember = "ID";
        }

        /// <summary>
        /// Save user information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtConfirm.Text == txtPassword.Text)
            {
                User us = new User();
                if (_userId != 0)
                    us.LoadByPrimaryKey(_userId);
                else
                {
                    us.AddNew();
                    us.UserName = txtUsername.Text;
                    us.Password = txtPassword.Text;
                }
                us.FullName = txtFullName.Text;
                us.Address = txtAddress.Text;
                us.Mobile = txtMobile.Text;
                us.Active = ckActive.Checked;
                us.UserType = Convert.ToInt32(cboUserType.SelectedValue); 
                us.Save();


                string selectedCommodity = cbCommodity.EditValue.ToString();
                string[] comArr = selectedCommodity.Split(',');
                UserCommodityType uc = new UserCommodityType();
                uc.DeleteAllTypeForUser(us.ID);
                foreach (var t in comArr)
                {
                    uc.AddNew();
                    uc.TypeID = Convert.ToInt16(t);
                    uc.UserID = us.ID;
                    uc.Save();
                }

                string selectedStore = cbStore.EditValue.ToString();
                string[] storeArr = selectedStore.Split(',');
                UserStore ustr = new UserStore();
                ustr.DeleteAllTStoreForUser(us.ID);
                if (selectedStore != "") 
                foreach (var t in storeArr)
                {
                    ustr.AddNew();
                    ustr.StoreID = Convert.ToInt16(t);
                    ustr.UserID = us.ID;
                    ustr.Save();
                }
                XtraMessageBox.Show("successfully Saved", "Success");
                PopulateUser();
            }
            else
                XtraMessageBox.Show("Password doesnt match!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddNewUser_Click(object sender, EventArgs e)
        {
            _userId = 0;
            txtPassword.Text = "";
            txtConfirm.Text = "";
            txtFullName.Text = "";
            txtAddress.Text = "";
            txtMobile.Text = "";
            txtUsername.Text = "";
            ckActive.Checked = true;
            cboUserType.SelectedIndex = -1;
            grpLoginInfo.Enabled = true;
        }

        /// <summary>
        /// Update the form based on the selected user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItems.Count > 0)
            {
                int selected = Convert.ToInt32(lstUsers.SelectedItems[0].Tag);
                User us = new User();
                us.LoadByPrimaryKey(selected);
                txtFullName.Text = us.FullName;
                txtAddress.Text = us.Address;
                txtMobile.Text = us.Mobile;
                ckActive.Checked = us.Active;
                cboUserType.SelectedValue = us.UserType.ToString();
                _userId = us.ID;
                grpLoginInfo.Enabled = false;

                BLL.Type ucType = new BLL.Type();
                ucType.LoadAll();

                UserCommodityType usc = new UserCommodityType();
                DataTable comUser = usc.GetUserCommodityType(_userId); 

                cbCommodity.Properties.DataSource = ucType.DefaultView.ToTable();
                cbCommodity.Properties.DisplayMember = "Name";
                cbCommodity.Properties.ValueMember = "ID";
                string[] arr = new string[comUser.DefaultView.ToTable().Rows.Count];
                int index = 0;
                foreach (DataRow dr in comUser.DefaultView.ToTable().Rows)
                {
                    arr[index] = Convert.ToString(dr["ID"]);
                    index++;
                }
                char separator = cbCommodity.Properties.SeparatorChar;
                string result = string.Empty;
                foreach (var element in arr)
                {
                    result += element + separator;
                }
                cbCommodity.SetEditValue(result);

                UserStore ust = new UserStore();
                DataTable storeUser = ust.GetUserStore(_userId);
                Stores stuser = new Stores();
                stuser.LoadAll();
                cbStore.Properties.DataSource = stuser.DefaultView;
                cbStore.Properties.DisplayMember = "StoreName";
                cbStore.Properties.ValueMember = "ID";
                string[] arrst = new string[storeUser.DefaultView.ToTable().Rows.Count];
                int rowindex = 0;
                foreach (DataRow drs in storeUser.DefaultView.ToTable().Rows)
                {
                    arrst[rowindex] = Convert.ToString(drs["ID"]);
                    rowindex++;
                }
                char separatorst = cbStore.Properties.SeparatorChar;
                string resultst = string.Empty;
                foreach (var element in arrst)
                {
                    resultst += element + separatorst;
                }
                cbStore.SetEditValue(resultst);


            }
        }

        /// <summary>
        /// Handles cancelling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            _userId = 0;
            txtPassword.Text = "";
            txtConfirm.Text = "";
            txtFullName.Text = "";
            txtAddress.Text = "";
            txtMobile.Text = "";
            txtUsername.Text = "";
            ckActive.Checked = true;
            cboUserType.SelectedIndex = -1;
            grpLoginInfo.Enabled = true;
        }

        private int _sortColumn = -1;
        
        /// <summary>
        /// Handles sorting when the column of the list box is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstUsers_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != _sortColumn)
            {
                _sortColumn = e.Column;
                lstUsers.Sorting = SortOrder.Ascending;
            }
            else
            {
                lstUsers.Sorting = ((lstUsers.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
            }
            const int num = 0;
            this.lstUsers.ListViewItemSorter = new ListViewItemComparer(e.Column, lstUsers.Sorting, num);
            lstUsers.Sort();
        }

      
    }
}