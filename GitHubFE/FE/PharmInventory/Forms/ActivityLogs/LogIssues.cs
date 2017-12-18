using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BLL;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using PharmInventory.Forms.Modals;
using PharmInventory.HelperClasses;
using EthiopianDate;
using DevExpress.XtraCharts.Native;

namespace PharmInventory.Forms.ActivityLogs
{
    /// <summary>
    /// Shows the log of the issued items.
    /// It can also be used to delete unprocessed issue transactions.
    /// </summary>
    public partial class LogIssues : XtraForm
    {
        public LogIssues()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Populate the lookups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageItems_Load(object sender, EventArgs e)
        {
            var usr = new User();
            var userID = MainWindow.LoggedinId;
            usr.LoadByPrimaryKey(userID);
            if (usr.UserType == 1)
                contextMenuStrip1.Enabled = false;

            var rec = new ReceivingUnits();
            DataTable drRec = rec.GetAllApplicableDU();
            //cboIssuedTo.Properties.DataSource = drRec;
            //cboIssuedTo.ItemIndex = -1;//.SelectedIndex = -1;
            //cboIssuedTo.Text = @"Select Issue Location";


            var itemunit = new ItemUnit();
            var units = itemunit.GetAllUnits();
            unitbindingSource.DataSource = units.DefaultView;

            // populate the receiving unit's lookup edit
            var rus = new ReceivingUnits();
            rus.GetActiveDispensaries();
            lkEditReceivingUnis.DataSource = rus.DefaultView;

            //var stor = new Stores();
            //stor.GetActiveStores();
            UserStore ucs = new UserStore();
            DataTable dt = ucs.GetUserStore(MainWindow.LoggedinId);
            //cboStores.Properties.DataSource = stor.DefaultView;
            cboStores.Properties.DataSource = dt; 
            cboStores.ItemIndex = 0;
            var unitcolumn = ((GridView)gridIssues.MainView).Columns[12];
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

            try
            {
                DataRowView dr = (DataRowView)lstTree.GetDataRecordByNode(lstTree.Nodes[0].FirstNode);

                if (dr == null) return;

                //lstTransactions.Items.Clear();                
                IssueDoc iss = new IssueDoc();
                DataTable dtRec;
                if (dr["ParentID"] == DBNull.Value)
                {
                    EthiopianDate.EthiopianDate ethiopianDate = new EthiopianDate.EthiopianDate(Convert.ToInt32(dr["ID"]), 1, 1);
                    dtRec = iss.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), ethiopianDate.StartOfFiscalYear.ToGregorianDate(), ethiopianDate.EndOfFiscalYear.ToGregorianDate());
                    string dateString = dr["RefNo"].ToString();
                    lblIssDate.Text = dateString;
                }
                else
                {
                    //dtRec = iss.GetTransactionByRefNo(dr["RefNo"].ToString(), Convert.ToInt32(cboStores.EditValue), dr["Date"].ToString());
                    dtRec = iss.GetTransactionByRefNo(dr["RefNo"].ToString(), Convert.ToDateTime(dr["Date"]));
                    lblIssDate.Text = Convert.ToDateTime(dr["Date"]).ToString("MM dd,yyyy");
                }
                gridIssues.DataSource = dtRec;
                grdLogIssue.ActiveFilterString = String.Format("Quantity<>0");
            }

            catch (Exception ex)
            {
                // do nothing
            }
            grdLogIssue.Columns["InternalDrugCode"].Visible = Convert.ToBoolean(chkIntDrugCode.EditValue);
        }

        private void cboStores_EditValueChanged(object sender, EventArgs e)
        {
            if (cboStores.EditValue == null) return;
            var iss = new IssueDoc();
           //  var dtRec = iss.GetDistinctIssueDocments(Convert.ToInt32(cboStores.EditValue));
            UserCommodityType ucs = new UserCommodityType();
            DataTable dt = ucs.GetUserCommodityType(MainWindow.LoggedinId);

            int[] typeid = new int[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                typeid[i] = Convert.ToInt32(dt.Rows[i]["ID"]);
            }
            var typeids = string.Join(",", typeid);
            var dtRec = iss.GetDistinctIssueDocmentsUsers(Convert.ToInt32(cboStores.EditValue), typeids);

            lstTree.DataSource = dtRec;

            DateTime dt1 = EthiopianDate.EthiopianDate.Now.StartOfFiscalYear.ToGregorianDate();
            DateTime dt2 = DateTime.Now;
            dtRec = iss.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue),dt1,dt2);
            lblIssDate.Text = @"Current Year";
            gridIssues.DataSource = dtRec;
        }

        private void cboIssuedTo_EditValueChanged(object sender, EventArgs e)
        {
            //if (cboIssuedTo.EditValue == null || cboStores.EditValue == null) return;

            //IssueDoc iss = new IssueDoc();

            //DataTable dtRec = iss.GetTransactionByReceivingUnit(Convert.ToInt32(cboStores.EditValue), Convert.ToInt32(cboIssuedTo.EditValue));
            //gridIssues.DataSource = dtRec;
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            //CALENDAR:
            IssueDoc iss = new IssueDoc();
            //dtFrom.CustomFormat = "MM/dd/yyyy";
            //dtTo.CustomFormat = "MM/dd/yyyy";

            //DateTime dteFrom = ConvertDate.DateConverter(dtFrom.Text);
            //DateTime dteTo = ConvertDate.DateConverter(dtTo.Text);
            
         //   DataTable dtRec = dtFrom.Value < dtTo.Value ? iss.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), dtFrom.Value, dtTo.Value) : iss.GetAllTransaction(Convert.ToInt32(cboStores.EditValue));
           // gridIssues.DataSource = dtRec;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

            var us = new User();
            var userID = MainWindow.LoggedinId;
            us.LoadByPrimaryKey(userID);

            DataRow dr = grdLogIssue.GetFocusedDataRow();

            if (dr == null) return;

             int tranId = Convert.ToInt32(dr["ID"]);
            var rec = new ReceiveDoc();
            var iss = new IssueDoc();
            iss.LoadByPrimaryKey(tranId);

            string batchNo = iss.BatchNo;
            try
            {
                if (iss.RecievDocID.ToString() != "")
                    rec.LoadByPrimaryKey(iss.RecievDocID);
                else
                    rec.GetTransactionByBatch(iss.ItemID, iss.BatchNo, iss.StoreId);
            }
            catch
            {
                rec.GetTransactionByBatch(iss.ItemID, iss.BatchNo, iss.StoreId);
            }

            if ( batchNo != rec.BatchNo && batchNo !=string.Empty)
            {
                XtraMessageBox.Show("Unable to Delete, This Transaction has been processed. Try Loss and Adjustment.", "Unable to Delete", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
            else if (batchNo == string.Empty || batchNo ==rec.BatchNo)
            {
                if (XtraMessageBox.Show("Are You Sure, You want to delete this Transaction? You will not be able to restore this data.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    rec.QuantityLeft = rec.QuantityLeft + iss.Quantity;
                    if (rec.QuantityLeft > 0)
                        rec.Out = false;
                    rec.Save();
                    AddIssueLodDelete(iss);
                    iss.MarkAsDeleted();
                    iss.Save();

                    DataTable dtRec = iss.GetAllTransaction(Convert.ToInt32(cboStores.EditValue));
                    gridIssues.DataSource = dtRec;
                }
            }
        }

        private static void AddIssueLodDelete(IssueDoc iss)
        {
            IssueDocDeleted isdelete = new IssueDocDeleted();
            isdelete.AddNew();
            isdelete.ID = iss.ID;
            isdelete.ItemID = iss.ItemID;
            isdelete.StoreId = iss.StoreId;
            isdelete.ReceivingUnitID = iss.ReceivingUnitID;
            isdelete.LocalBatchNo = iss.LocalBatchNo;
            isdelete.Quantity = iss.Quantity;
            isdelete.Date = iss.Date;
            isdelete.IsTransfer = iss.IsTransfer;
            isdelete.IssuedBy = iss.IssuedBy;
            isdelete.Remark = iss.Remark;
            isdelete.RefNo = iss.RefNo;
            isdelete.BatchNo = iss.BatchNo;
            isdelete.IsApproved = iss.IsApproved;
            isdelete.Cost = iss.Cost;
            isdelete.NoOfPack = iss.NoOfPack;
            isdelete.QtyPerPack = iss.QtyPerPack;
            isdelete.DUSOH = iss.DUSOH;
            isdelete.EurDate = iss.EurDate;
            isdelete.RecievDocID = iss.RecievDocID;
            isdelete.RecomendedQty = iss.RecomendedQty;
            isdelete.Save();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DataRow dr = grdLogIssue.GetFocusedDataRow();

            if (dr == null) return;

            int tranId = Convert.ToInt32(dr["ID"]);
            EditIssue edRec = new EditIssue(tranId);
            MainWindow.ShowForms(edRec);
            IssueDoc iss = new IssueDoc();
            DataTable dtRec = iss.GetAllTransaction(Convert.ToInt32(cboStores.EditValue));
            /*PopulateTransaction(dtRec);*/
            gridIssues.DataSource = dtRec;
        }


        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog {Filter = @"Microsoft Excel | *.xls"};

            if (DialogResult.OK == saveDlg.ShowDialog())
            {
                gridIssues.MainView.ExportToXls(saveDlg.FileName);
                
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            colIssuedBy.Visible = false;
            colIssuedTo.Visible = false;
            colDBEI.Visible = false;
            colIssuedToto.Visible = false;
            printableComponentLink1.CreateMarginalHeaderArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalHeaderArea);
            printableComponentLink1.CreateMarginalFooterArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalFooterArea);
            printableComponentLink1.CreateDocument();
            printableComponentLink1.ShowPreview();
            colIssuedBy.Visible = true;
            colIssuedTo.Visible = true;
            colDBEI.Visible = true;
            colIssuedToto.Visible = true;
        }
        private void printableComponentLink1_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
        {
            PageInfoBrick pib = new PageInfoBrick();  
            pib.Format =  "Page {0}/{1}";
            DataTable dt = (DataTable)gridIssues.DataSource;  
            DataView dv = new DataView(dt); 
            string Issuedto = "Issued To: " + dv[0]["IssuedTo"] as string;
            string Issuedby = "Issued By: " + dv[0]["IssuedBy"] as string;
            //RectangleF r = RectangleF.Empty;
            //r.Height = 20;

            //pib = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pib.Format,  Color.Black, r, BorderSide.None);
            //PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, "Print Date " +DateTime.Now.ToShortDateString() + " G.C",
            //                      Color.Black, r, BorderSide.None);
            //brick.Alignment = BrickAlignment.Far; 
            pib = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pib.Format, Color.Black, new RectangleF(100, 0, 200, 20), BorderSide.None);
            PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pib.Format + "\n Print Date " + DateTime.Now.ToShortDateString() + " G.C",
                                  Color.Black, new RectangleF(100, 0, 200, 40), BorderSide.None);

            brick.Alignment = BrickAlignment.Far;
            TextBrick brickleft = e.Graph.DrawString(Issuedto, Color.Navy, new RectangleF(0, 0, 600, 40),
                                        DevExpress.XtraPrinting.BorderSide.None);
            brickleft.Font = new Font("Tahoma", 10);
            brickleft.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);
            TextBrick brickrb = e.Graph.DrawString(Issuedby, Color.Navy, new RectangleF(0, 20, 600, 40),
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
            string issdate = lstTree.FocusedNode.GetDisplayText("Date");
            //  string header = info.HospitalName + "\n Issue Activity Log, Store: " + cboStores.Text + " \n RefNo:  " + refNumber + " On " + issdate + " E.C"; ;
            string header = info.HospitalName + "\nStore: " + cboStores.Text;
            string headercenter = "Issue Activity Log";
            string headerRight = " \n RefNo:  " + refNumber + "  \nDate: " + issdate + " E.C";
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
            printableComponentLink1.Landscape = true;
            printableComponentLink1.PageHeaderFooter = header;

            //TextBrick brick = e.Graph.DrawString(header, Color.Navy, new RectangleF(0, 0, 1000, 100),
            //                                    DevExpress.XtraPrinting.BorderSide.None);
            //brick.Font = new Font("Tahoma", 13);
            //brick.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Center);
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
            //old header
            //string header = info.HospitalName + " Issue Activity Log " + dtCurrent.ToString("MM dd,yyyy");
            
            string refNumber = lstTree.FocusedNode.GetDisplayText("RefNo");
            //header that includes refno
            string header = info.HospitalName + " Issue Activity Log \n" + dtCurrent.ToString("MM dd,yyyy") + "  RefNumber  " + refNumber;

            TextBrick brick = e.Graph.DrawString(header, Color.Navy, new RectangleF(0, 0, 800, 40),
                                                 DevExpress.XtraPrinting.BorderSide.None);
            brick.Font = new Font("Arial", 16);
            brick.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Center);
        }

        private void detailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dr = grdLogIssue.GetFocusedDataRow();

            if (dr == null) return;

            int tranId = Convert.ToInt32(dr["ID"]);

            var dtDate = new CalendarLib.DateTimePickerEx
                                                      {
                                                          Value = DateTime.Now,
                                                          CustomFormat = "MM/dd/yyyy"
                                                      };
            DateTime dtCur = ConvertDate.DateConverter(dtDate.Text);
            int itemId = Convert.ToInt32(dr["ItemID"]);
            int yr = ((dtCur.Month < 11) ? dtCur.Year : dtCur.Year + 1);
            switch (VisibilitySetting.HandleUnits)
            {
                case 1:
                    {
                        var con = new ItemDetailReport(itemId, Convert.ToInt32(cboStores.EditValue), yr, 0);
                        con.ShowDialog();
                    }
                    break;
                case 2:
                    {
                        int unitId = Convert.ToInt32(dr["UnitID"]);
                        var con = new ItemDetailReport(itemId, Convert.ToInt32(cboStores.EditValue), yr, 0,unitId);
                        con.ShowDialog();
                    }
                    break;
                case 3:
                    {
                        int unitId = Convert.ToInt32(dr["UnitID"]);
                        var con = new ItemDetailReport(itemId, Convert.ToInt32(cboStores.EditValue), yr, 0,unitId);
                        con.ShowDialog();
                    }
                    break;
            }
        }
        
        private void lstTree_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            var dr = (DataRowView)lstTree.GetDataRecordByNode(lstTree.FocusedNode);
            if (dr == null) return;
            //lstTransactions.Items.Clear();                
            var iss = new IssueDoc();
            DataTable dtRec;
            if (dr["ParentID"] == DBNull.Value)
            {
                EthiopianDate.EthiopianDate ethiopianDate=new EthiopianDate.EthiopianDate(Convert.ToInt32(dr["ID"]),1,1);
                dtRec = iss.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), ethiopianDate.StartOfFiscalYear.ToGregorianDate(), ethiopianDate.EndOfFiscalYear.ToGregorianDate());
                string dateString = dr["RefNo"].ToString();
                lblIssDate.Text = dateString;
            }
            else
            {
                //dtRec = iss.GetTransactionByRefNo(dr["RefNo"].ToString(), Convert.ToInt32(cboStores.EditValue), dr["Date"].ToString());
                dtRec = iss.GetTransactionByRefNo(dr["RefNo"].ToString(), Convert.ToDateTime(dr["Date"]));
                lblIssDate.Text = Convert.ToDateTime(dr["Date"]).ToString("MM dd,yyyy");
                
            }
            gridIssues.DataSource = dtRec;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var us = new User();
            var userID = MainWindow.LoggedinId;
            us.LoadByPrimaryKey(userID);
            var dr = (DataRowView)lstTree.GetDataRecordByNode(lstTree.FocusedNode);
            if (dr == null) return;
            if (us.UserName != "admin")
            {
                XtraMessageBox.Show("You don't have the privilege to update reference number!", "Caution");
                return;
            }
            var iss = new EditIssueDocRefrenceNo((string)dr["RefNo"]);
            iss.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var us = new User();
            var userID = MainWindow.LoggedinId;
            us.LoadByPrimaryKey(userID);
            var dr = (DataRowView)lstTree.GetDataRecordByNode(lstTree.FocusedNode);
            if(dr==null)return;
            if (us.UserName != "admin")
            {
                XtraMessageBox.Show("You don't have the privilege to update reference number!", "Caution");
                return;
            }
            if (XtraMessageBox.Show("Are You Sure, You want to delete this?", "Confirmation", MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var dis = new IssueDoc();
                DataTable dtbl = dis.GetTransactionByRefNo((string)dr["RefNo"]);
                foreach (DataRow dataRow in dtbl.Rows)
                {
                    AddIssueDocDeleted(dataRow);
                    dataRow.Delete();
                }
                dis.MarkAsDeleted();
                dis.Save();
                XtraMessageBox.Show("Item successfully deleted.", "Success");
            }
        }

        private static void AddIssueDocDeleted(DataRow dataRow)
        {
            var isdelete = new IssueDocDeleted();
            isdelete.AddNew();
            isdelete.ID = (int) dataRow["ID"];
            isdelete.ItemID = (int) dataRow["ItemID"];
            isdelete.StoreId = (int) dataRow["StoreId"];
            isdelete.ReceivingUnitID = (int) dataRow["ReceivingUnitID"];
            //isdelete.LocalBatchNo = (string) dataRow["LocalBatchNo"];
            isdelete.Quantity = (long) dataRow["Quantity"];
            isdelete.Date = (DateTime) dataRow["Date"];
            isdelete.IsTransfer = (bool) dataRow["IsTransfer"];
            isdelete.IssuedBy = (string) dataRow["IssuedBy"];
            //isdelete.Remark = (string) dataRow["Remark"];
            isdelete.RefNo = (string) dataRow["RefNo"];
            isdelete.BatchNo = (string) dataRow["BatchNo"];
            isdelete.IsApproved = (bool) dataRow["IsApproved"];
            isdelete.Cost = (double) dataRow["Cost"];
            isdelete.NoOfPack = (int) dataRow["NoOfPack"];
            isdelete.QtyPerPack = (int) dataRow["QtyPerPack"];
            isdelete.DUSOH = (long) dataRow["DUSOH"];
            isdelete.EurDate = (DateTime) dataRow["EurDate"];
            isdelete.RecievDocID = (int) dataRow["RecievDocID"];
            isdelete.RecomendedQty = (long) dataRow["RecomendedQty"];
            isdelete.Save();
        }

        private void chkIntDrugCode_CheckedChanged(object sender, EventArgs e)
        {
            grdLogIssue.Columns["InternalDrugCode"].Visible = Convert.ToBoolean(chkIntDrugCode.EditValue);
        }
    }
}