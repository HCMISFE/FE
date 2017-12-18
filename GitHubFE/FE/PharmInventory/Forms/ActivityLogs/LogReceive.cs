using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using BLL;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using PharmInventory.Forms.Modals;
using PharmInventory.HelperClasses;


namespace PharmInventory.Forms.ActivityLogs
{
    /// <summary>
    /// Displays the log of the received items.
    /// </summary>
    public partial class LogReceive : XtraForm
    {
        private DataTable dtRec;

        public LogReceive()
        {
            InitializeComponent();
        }

        private readonly CalendarLib.DateTimePickerEx _dtDate = new CalendarLib.DateTimePickerEx();

        private void ManageItems_Load(object sender, EventArgs e)
        {
            var usr = new User();
            var userID = MainWindow.LoggedinId;
            usr.LoadByPrimaryKey(userID);
            if(usr.UserType ==1)
            contextMenuStrip1.Enabled = false;
            //var stor = new Stores();
            //stor.GetActiveStores();
            //cboStores.Properties.DataSource = stor.DefaultView;
            UserStore ucs = new UserStore();
            DataTable dt = ucs.GetUserStore(MainWindow.LoggedinId);
            cboStores.Properties.DataSource = dt;
            cboStores.ItemIndex = 0;

            var sup = new Supplier();
            var dtSup = sup.GetSuppliersWithTransaction();
            //cboSupplier.Properties.DataSource = dtSup;
            //cboSupplier.ItemIndex = -1;
            //cboSupplier.Text = "Select Supplier";

            var itemunit = new ItemUnit();
            var units = itemunit.GetAllUnits();
            unitsbindingSource.DataSource = units.DefaultView;

            // bind the supplier lookup for the grid.
            //lkEditSupplier.DataSource = dtSup;
            var unitcolumn = ((GridView)gridReceives.MainView).Columns[14];
            switch (VisibilitySetting.HandleUnits)
            {
                case 1:
                    unitcolumn.Visible = false;
                    break;
                case 2:
                    unitcolumn.Visible = true;
                    break;
                default:
                    unitcolumn.Visible = true;
                    break;
            }

            // bind the current dates
            grdLogReceive.Columns["InternalDrugCode"].Visible = Convert.ToBoolean(chkIntDrugCode.EditValue);
            try
            {
                var dr = (DataRowView) lstTree.GetDataRecordByNode(lstTree.Nodes[0].FirstNode);
                if (dr == null) return;
                var rec = new ReceiveDoc();
                var supplier= new Supplier();
                
                if (dr["ParentID"] == DBNull.Value)
                {
                   // int yr = ((dtCurrent.Month > 10) ? dtCurrent.Year : dtCurrent.Year - 1);
                    var dt1 = new DateTime(Convert.ToInt32(dr["ID"]) - 1, 11, 1);
                    var dt2 = new DateTime(Convert.ToInt32(dr["ID"]), 11, 1);
                    dtRec = rec.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), dt1, dt2);
                    lblRecDate.Text =  dr["RefNo"].ToString();
                }
                else
                {
                    dtRec = rec.GetTransactionByRefNo(dr["RefNo"].ToString(), Convert.ToInt32(cboStores.EditValue),dr["Date"].ToString());
                    lblRecDate.Text = Convert.ToDateTime(dr["Date"]).ToString("MM dd,yyyy");
                }
                gridReceives.DataSource = dtRec;
            }
            catch (Exception ex)
            {
                
            }
       
        }

        private void PopulateDocuments(DataTable dtRec)
        {
            lstTree.DataSource = dtRec;
            lstTree.ExpandAll();
            // show the last entry
        }

        private void cboStores_EditValueChanged(object sender, EventArgs e)
        {
            if (cboStores.EditValue == null) return;
            var rec = new ReceiveDoc();
            // dtRec = rec.GetDistinctRecDocments(Convert.ToInt32(cboStores.EditValue));
            UserCommodityType ucs = new UserCommodityType();
            DataTable dt = ucs.GetUserCommodityType(MainWindow.LoggedinId);

            int[] typeid = new int[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                typeid[i] = Convert.ToInt32(dt.Rows[i]["ID"]);
            }
            var typeids = string.Join(",", typeid);
            dtRec = rec.GetDistinctRecDocmentbyUsers(Convert.ToInt32(cboStores.EditValue), typeids);
            PopulateDocuments(dtRec);

        }

        private void cboSupplier_EditValueChanged(object sender, EventArgs e)
        {
           // if (cboSupplier.EditValue == null) return;
            //var rec = new ReceiveDoc();
            //DataTable dtRec = rec.GetTransactionBySupplierId(Convert.ToInt32(cboStores.EditValue),
            //                                                 Convert.ToInt32(cboSupplier.EditValue));
            //gridReceives.DataSource = dtRec;
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var us = new User();
            var userID = MainWindow.LoggedinId;
            us.LoadByPrimaryKey(userID);
            var dr = grdLogReceive.GetFocusedDataRow();

            if (dr == null) return;

            //if (us.UserName != "admin")
            //{
            //    XtraMessageBox.Show("You don't have the privilege to update reference number!", "Caution");
            //    return;
            //}

            int tranId = Convert.ToInt32(dr["ID"]);
            var rec = new ReceiveDoc();
            var iss = new IssueDoc();
            var dis = new Disposal();
            rec.LoadByPrimaryKey(tranId);
            iss.GetIssueByBatchAndId(rec.ItemID, rec.BatchNo, rec.ID);
            _dtDate.Value = DateTime.Now;
            _dtDate.CustomFormat = "MM/dd/yyyy";

            //if (iss.RowCount == 0)
            //{
                if ((iss.RowCount != 0) && (iss.RecievDocID != null && iss.RecievDocID == rec.ID))
                {
                    var edRec = new EditReceive(tranId, true,Convert.ToBoolean(chkIntDrugCode.EditValue));
                    MainWindow.ShowForms(edRec);
                }
            //}
            else if(iss.RowCount ==0)
            {
                var edRec = new EditReceive(tranId, false, Convert.ToBoolean(chkIntDrugCode.EditValue));
                MainWindow.ShowForms(edRec);
            }

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var us = new User();
            var userID = MainWindow.LoggedinId;
            us.LoadByPrimaryKey(userID);

            var dr = grdLogReceive.GetFocusedDataRow();

            if (dr == null) return;

            int tranId = Convert.ToInt32(dr["ID"]);
            var rec = new ReceiveDoc();
            rec.LoadByPrimaryKey(tranId);
            var iss = new IssueDoc();
            iss.GetIssueByBatchAndId(rec.ItemID, rec.BatchNo, rec.ID);
            _dtDate.CustomFormat = "MM/dd/yyyy";
            DateTime dtCurrent = ConvertDate.DateConverter(_dtDate.Text);
            if ((rec.Date.Year != dtCurrent.Year && rec.Date.Month < 11) || (iss.RowCount != 0))
            {
                XtraMessageBox.Show("Unable to Delete, This Transaction has been processed. Try Loss and Adjustment.",
                                    "Unable to Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {

                if (
                    XtraMessageBox.Show(
                        "Are You Sure, You want to delete this Transaction? You will not be able to restore this data.",
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //AddDeletedRecieveDoc(rec);
                    rec.MarkAsDeleted();
                    rec.Save();

                    _dtDate.Value = DateTime.Now;
                    _dtDate.CustomFormat = "MM/dd/yyyy";
                    dtCurrent = ConvertDate.DateConverter(_dtDate.Text);
                    int yr = ((dtCurrent.Month > 10) ? dtCurrent.Year : dtCurrent.Year - 1);
                    DateTime dt1 = new DateTime(yr, 11, 1);
                    DateTime dt2 = new DateTime(dtCurrent.Year, dtCurrent.Month, dtCurrent.Day);
                    DataTable dtRec = rec.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), dt1, dt2);
                    gridReceives.DataSource = dtRec;
                }
            }
        }


        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            string fileName = MainWindow.GetNewFileName("xls");
            gridReceives.ExportToXls(fileName);
            MainWindow.OpenInExcel(fileName);

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            colSOH.Visible = false;
            colDBER.Visible = false;
            colSupplier.Visible = false;
            printableComponentLink1.CreateMarginalHeaderArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalHeaderArea);
            printableComponentLink1.CreateMarginalFooterArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalFooterArea);
            printableComponentLink1.Landscape = true;
            printableComponentLink1.CreateDocument();
            printableComponentLink1.Landscape = true;
            printableComponentLink1.ShowPreview();
           // colSOH.Visible = true;
            colDBER.Visible = true;
            colSupplier.Visible = true;
        }
        private void printableComponentLink1_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
        {
            PageInfoBrick pib = new PageInfoBrick();
            pib.Format = "Page {0}/{1}";
            string Suppliername =  "Supplied By: " + dtRec.AsDataView()[0]["SupplierName"] as string;
            string Receivername = "Received By: " + dtRec.AsDataView()[0]["ReceivedBy"] as string;
            //RectangleF r = RectangleF.Empty;
            //r.Height = 20;
            //r.X = 800;


            pib = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pib.Format, Color.Black, new RectangleF(100, 0, 200, 20), BorderSide.None);
            PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pib.Format + "\n Print Date " + DateTime.Now.ToShortDateString() + " G.C",
                                  Color.Black,new RectangleF(100, 0, 200, 40), BorderSide.None);

            brick.Alignment = BrickAlignment.Far; 
            TextBrick brickleft = e.Graph.DrawString(Suppliername, Color.Navy, new RectangleF(0, 0, 200, 20),
                                        DevExpress.XtraPrinting.BorderSide.None);
            brickleft.Font = new Font("Tahoma", 10);
            brickleft.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);
            TextBrick brickrb = e.Graph.DrawString(Receivername, Color.Navy, new RectangleF(0, 20, 200, 20),
                                   DevExpress.XtraPrinting.BorderSide.None);
            brickrb.Font = new Font("Tahoma", 10);
            brickrb.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);
        }
        private void printableComponentLink1_CreateMarginalHeaderArea(object sender, CreateAreaEventArgs e)
        {
            GeneralInfo info = new GeneralInfo();
            info.LoadAll();
            //CalendarLib.DateTimePickerEx dtDate = new CalendarLib.DateTimePickerEx
            //{
            //    Value = DateTime.Now,
            //    CustomFormat = "MM/dd/yyyy"
            //};
            //DateTime dtCurrent = Convert.ToDateTime(dtDate.Text);
           
            string refNumber = lstTree.FocusedNode.GetDisplayText("RefNo");
            string rcdate = lstTree.FocusedNode.GetDisplayText("Year");
            if ((lstTree.FocusedNode.GetDisplayText("Date")) != "")
                 rcdate = (Convert.ToDateTime(lstTree.FocusedNode.GetDisplayText("Date"))).ToShortDateString() ;

            // string header = info.HospitalName + " \nReceive Activity Log Store: " + cboStores.Text + " \n RefNo:  " + refNumber + "  On " + rcdate + " E.C";
            string header = info.HospitalName + "\nStore: " + cboStores.Text;
            string headercenter = "Receive Activity Log";
            string headerRight = " \n RefNo:  " + refNumber + "  \nDate: " + rcdate + " E.C";  
            printableComponentLink1.Landscape = true;
            printableComponentLink1.PageHeaderFooter = header;

            TextBrick brickcenter = e.Graph.DrawString(headercenter, Color.Navy, new RectangleF(400, 40, 400, 100),
                                               DevExpress.XtraPrinting.BorderSide.None);
            brickcenter.Font = new Font("Tahoma", 13); 
            brickcenter.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);

            TextBrick brick = e.Graph.DrawString(header, Color.Navy, new RectangleF(0, 40, 400, 100),
                                                DevExpress.XtraPrinting.BorderSide.None);
            brick.Font = new Font("Tahoma", 13);
            brick.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);

            TextBrick brickright = e.Graph.DrawString(headerRight, Color.Navy, new RectangleF(800, 20, 400, 100),
                                                DevExpress.XtraPrinting.BorderSide.None);
            brickright.Font = new Font("Tahoma", 13);
            brickright.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);

        }

        private void pcl_CreateReportHeaderArea(object sender, DevExpress.XtraPrinting.CreateAreaEventArgs e)
        {
            GeneralInfo info = new GeneralInfo();
            info.LoadAll();
            CalendarLib.DateTimePickerEx dtDate = new CalendarLib.DateTimePickerEx
                                                      {
                                                          Value = DateTime.Now,
                                                          CustomFormat = "MM/dd/yyyy"
                                                      };
            DateTime dtCurrent = Convert.ToDateTime(dtDate.Text);
            // original header
            // string header = info.HospitalName + " Receive Activity Log " + dtCurrent.ToString("MM dd,yyyy");
            // header with reference number
            string refNumber = lstTree.FocusedNode.GetDisplayText("RefNo");
            string header = info.HospitalName + " Receive Activity Log \n" + dtCurrent.ToString("MM dd,yyyy") +
                            "   RefNo " + refNumber;

            TextBrick brick = e.Graph.DrawString(header, Color.Navy, new RectangleF(0, 0, 500, 40),
                                                 DevExpress.XtraPrinting.BorderSide.None);
            brick.Font = new Font("Arial", 16);
            brick.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Center);
        }

        private void lstTree_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            var dtDate = new CalendarLib.DateTimePickerEx
                                                      {
                                                          CustomFormat = "MM/dd/yyyy",
                                                          Value = DateTime.Now
                                                      };
            DateTime dtCurrent = ConvertDate.DateConverter(dtDate.Text);
            var dr = (DataRowView) lstTree.GetDataRecordByNode(lstTree.FocusedNode);
            if (dr == null) return;

            var rec = new ReceiveDoc();
            DataTable dtRec;
            if (dr["ParentID"] == DBNull.Value)
            {
                int yr = ((dtCurrent.Month > 10) ? dtCurrent.Year : dtCurrent.Year - 1);
                DateTime dt1 = new DateTime(Convert.ToInt32(dr["ID"]) - 1, 11, 1);
                DateTime dt2 = new DateTime(Convert.ToInt32(dr["ID"]), 11, 1);
                dtRec = rec.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), dt1, dt2);
                lblRecDate.Text = dr["RefNo"].ToString();
            }
            else
            {
                dtRec = rec.GetTransactionByRefNo(dr["RefNo"].ToString(), Convert.ToInt32(cboStores.EditValue),dr["Date"].ToString());
                lblRecDate.Text = Convert.ToDateTime(dr["Date"]).ToString("MM dd,yyyy");
            }

            gridReceives.DataSource = dtRec;
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {

            var us = new User();
            var userID = MainWindow.LoggedinId;
            us.LoadByPrimaryKey(userID); 
            if (us.UserType != 1)
            {
                DataRow dr = grdLogReceive.GetFocusedDataRow();

                if (dr == null) return;

                int tranId = Convert.ToInt32(dr["ID"]);
                ReceiveDoc rec = new ReceiveDoc();
                rec.LoadByPrimaryKey(tranId);

                _dtDate.Value = DateTime.Now;
                _dtDate.CustomFormat = "MM/dd/yyyy";
                IssueDoc iss = new IssueDoc();
                iss.GetIssueByBatchAndId(rec.ItemID, rec.BatchNo, rec.ID);
                DateTime dtCurrent = ConvertDate.DateConverter(_dtDate.Text);

                if ((rec.Date.Year != dtCurrent.Year && rec.Date.Month < 11) || (iss.RowCount != 0))
                {
                    //XtraMessageBox.Show("Unable to edit, This Transaction has been processed. Try Loss and Adjustment.", "Unable to Edit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    EditReceive edRec = new EditReceive(tranId, true, Convert.ToBoolean(chkIntDrugCode.EditValue));
                    MainWindow.ShowForms(edRec);
                }
                else
                {
                    EditReceive edRec = new EditReceive(tranId, false, Convert.ToBoolean(chkIntDrugCode.EditValue));
                    MainWindow.ShowForms(edRec);
                }
            }
               
        }

        private void btnDeleteWithRfNo_Click(object sender, EventArgs e)
        {
            CalendarLib.DateTimePickerEx dtDate = new CalendarLib.DateTimePickerEx
                                                      {
                                                          CustomFormat = "MM/dd/yyyy",
                                                          Value = DateTime.Now
                                                      };
            DateTime dtCurrent = ConvertDate.DateConverter(dtDate.Text);

            DataRowView dr = (DataRowView) lstTree.GetDataRecordByNode(lstTree.FocusedNode);

            if (dr == null) return;

            ReceiveDoc rec = new ReceiveDoc();
            DataTable dtRec;
            if (dr["ParentID"] == DBNull.Value)
            {
                int yr = ((dtCurrent.Month > 10) ? dtCurrent.Year : dtCurrent.Year - 1);
                DateTime dt1 = new DateTime(Convert.ToInt32(dr["ID"]) - 1, 11, 1);
                DateTime dt2 = new DateTime(Convert.ToInt32(dr["ID"]), 11, 1);
                dtRec = rec.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), dt1, dt2);
                string dateString = dr["RefNo"].ToString();
                lblRecDate.Text = dateString;
            }
            else
            {
                dtRec = rec.GetTransactionByRefNo(dr["RefNo"].ToString(), Convert.ToInt32(cboStores.EditValue),
                                                  dr["Date"].ToString());
                lblRecDate.Text = Convert.ToDateTime(dr["Date"]).ToString("MM dd,yyyy");
            }
            foreach (DataRow recieve in dtRec.Rows)
            {
               AddDeletedRecieveDocs(recieve);
            }

        }

        private static void AddDeletedRecieveDocs(DataRow recieve)
        {
            var recd = new ReceiveDocDeleted();
            recd.AddNew();
            recd.ID = (int)recieve["ID"];
            recd.BatchNo = (string)recieve["BatchNo"];
            recd.ItemID = (int)recieve["ItemID"];
            recd.SupplierID = (int)recieve["SupplierID"];
            recd.Quantity = Convert.ToInt64(recieve["Quantity"]);
            recd.Date = (DateTime)recieve["Date"];
            recd.ExpDate = (DateTime)recieve["ExpDate"];
            recd.Out = (bool)recieve["Out"];
            // recd.ReceivedStatus = (int) recieve["ReceivedStatus"];
            // recd.ReceivedBy = (string) recieve["ReceivedBy"];
            //recd.Remark = (string)recieve["Remark"];
            recd.StoreID = (int)recieve["StoreID"];
            recd.LocalBatchNo = (string)recieve["LocalBatchNo"];
            recd.RefNo = (string)recieve["RefNo"];
            recd.Cost = (double)recieve["Cost"];
            recd.IsApproved = (bool)recieve["IsApproved"];
            // recd.ManufacturerId = (int) recieve["ManufacturerId"];
            recd.QuantityLeft = (long)recieve["QuantityLeft"];
            recd.NoOfPack = (int)recieve["NoOfPack"];
            recd.QtyPerPack = (int)recieve["QtyPerPack"];
            // recd.BoxLevel = (int) recieve["BoxLevel"];
            recd.EurDate = (DateTime)recieve["EurDate"];
            // recd.SubProgramID = (int) recieve["SubProgramID"];
            recd.Save();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var us = new User();
            var userID = MainWindow.LoggedinId;
            us.LoadByPrimaryKey(userID);
            var dr = (DataRowView) lstTree.GetDataRecordByNode(lstTree.FocusedNode);
            if (dr == null) return;
            if (us.UserName != "admin")
            {
                XtraMessageBox.Show("You don't have the privilege to update reference number!", "Caution");
                return;
            }
            var edtloss = new EditRecieveRefrenceNo((string) dr["RefNo"]);
            edtloss.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var us = new User();
            var userID = MainWindow.LoggedinId;
            us.LoadByPrimaryKey(userID);
            var dr = (DataRowView) lstTree.GetDataRecordByNode(lstTree.FocusedNode);
            if (dr == null) return;

            if (us.UserName != "admin")
            {
                XtraMessageBox.Show("You don't have the privilege to update reference number!", "Caution");
                return;
            }
            var rec = new ReceiveDoc();
            if (XtraMessageBox.Show("Are You Sure, You want to delete this?", "Confirmation", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DataTable dtbl = rec.GetTransactionByRefNo((string) dr["RefNo"]);
                foreach (DataRow dataRow in dtbl.Rows)
                {
                   // AddReceiveDocDeleted(dataRow);
                    dataRow.Delete();
                }
                rec.MarkAsDeleted();
                rec.Save();

                XtraMessageBox.Show("Item successfully deleted.", "Success");
            }
            else
            {
                return;
            }
        }

        private static void AddReceiveDocDeleted(DataRow dataRow)
        {
            var recd = new ReceiveDocDeleted();
            recd.AddNew();
            recd.ID = (int)dataRow["ID"];
            recd.BatchNo = (string)dataRow["BatchNo"];
            recd.ItemID = (int)dataRow["ItemID"];
            recd.SupplierID = (int)dataRow["SupplierID"];
            recd.Quantity = (long)dataRow["Quantity"];
            recd.Date = (DateTime)dataRow["Date"];
            recd.ExpDate = (DateTime)dataRow["ExpDate"];
            recd.Out = (bool)dataRow["Out"];
            recd.StoreID = (int)dataRow["StoreID"];
            recd.LocalBatchNo = (string)dataRow["LocalBatchNo"];
            recd.RefNo = (string)dataRow["RefNo"];
            recd.Cost = (double)dataRow["Cost"];
            recd.IsApproved = (bool)dataRow["IsApproved"];
            recd.QuantityLeft = (long)dataRow["QuantityLeft"];
            recd.NoOfPack = (int)dataRow["NoOfPack"];
            recd.QtyPerPack = (int)dataRow["QtyPerPack"];
            recd.EurDate = (DateTime)dataRow["EurDate"];
            recd.Save();
        }

        private void unitrepositoryItemLookUpEdit_Enter(object sender, EventArgs e)
        {

        }

        private void chkIntDrugCode_CheckedChanged(object sender, EventArgs e)
        {
            grdLogReceive.Columns["InternalDrugCode"].Visible = Convert.ToBoolean(chkIntDrugCode.EditValue);
        }

     
    }
}