using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BLL;
using CalendarLib;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using DevExpress.XtraTreeList;
using PharmInventory.Forms.Modals;
using PharmInventory.HelperClasses;

namespace PharmInventory.Forms.ActivityLogs
{
    /// <summary>
    /// Shows the Loss/Adjustment log
    /// </summary>
    public partial class LogAdjustment : XtraForm
    {
        private DataTable dtRec;
        public LogAdjustment()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prepare all lookups
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
            var stor = new Stores();
            stor.GetActiveStores();
            cboStores.Properties.DataSource = stor.DefaultView;
            cboStores.ItemIndex = 0;

            var rec = new DisposalReasons();
            var dtDis = rec.GetAvailableReasons();
            //cboReasons.Properties.DataSource = dtDis.DefaultView;
            reasonBindingSource.DataSource = dtDis.DefaultView;
           // cboReasons.ItemIndex = 0;

            var itemunit = new ItemUnit();
            var units = itemunit.GetAllUnits();
            unitbindingSource.DataSource = units.DefaultView;

            var unitcolumn = ((GridView)gridAdjustments.MainView).Columns[10];
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
                //CALENDAR:
                var dtDate = new CalendarLib.DateTimePickerEx
                {
                    CustomFormat = "MM/dd/yyyy",
                    Value = DateTime.Now
                };
                var dr = (DataRowView)lstTree.GetDataRecordByNode(lstTree.Nodes[0].FirstNode);
                if (dr == null) return;

                var disp = new Disposal();
                if (dr["ParentID"] == DBNull.Value)
                {
                    var dt1 = new DateTime(Convert.ToInt32(dr["ID"]) - 1, 11, 1);
                    var dt2 = new DateTime(Convert.ToInt32(dr["ID"]), 11, 1);
                    dtRec = disp.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue),dt1,dt2);
                    lblAdjDate.Text = dr["RefNo"].ToString();
                }
                else
                {
                    dtRec = disp.GetDocumentByRefNo(dr["RefNo"].ToString(), Convert.ToInt32(cboStores.EditValue), dr["Date"].ToString());
                    lblAdjDate.Text = Convert.ToDateTime(dr["Date"]).ToString("MM dd,yyyy");
                }
                gridAdjustments.DataSource = dtRec;
            }
            catch (Exception ex)
            {
                
            }
            grdLogAdjustment.Columns["InternalDrugCode"].Visible = Convert.ToBoolean(chkIntDrugCode.EditValue);
        }

        private void cboStores_EditValueChanged(object sender, EventArgs e)
        {
            if (cboStores.EditValue == null) return;
            var adj = new Disposal();
            dtRec = adj.GetDistinctAdjustmentDocments(Convert.ToInt32(cboStores.EditValue));
            PopulateDocuments(dtRec);

            //var dr = (DataRowView)lstTree.GetDataRecordByNode(lstTree.Nodes[0].FirstNode);
            //if (dr == null) return;

            //dtRec = adj.GetDocumentByRefNo(dr["RefNo"].ToString(), Convert.ToInt32(cboStores.EditValue), dr["Date"].ToString());
            //lblAdjDate.Text = Convert.ToDateTime(dr["Date"]).ToString("MM dd,yyyy");
            //gridAdjustments.DataSource = dtRec;
          

        }

        private void PopulateDocuments(DataTable dtRec)
        {
            lstTree.DataSource = dtRec;
            lstTree.ExpandAll();
        }


        private void cboReasons_EditValueChanged(object sender, EventArgs e)
        {
            //if (cboStores.EditValue != null && cboReasons.EditValue != null)
            //{
            //    var adj = new Disposal();
            //    dtRec = adj.GetTransactionByReason(Convert.ToInt32(cboStores.EditValue),Convert.ToInt32(cboReasons.EditValue));
            //    //PopulateDocuments(dtRec);
            //    gridAdjustments.DataSource = dtRec;                
            //}
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            //CALENDAR:
            //Disposal adj = new Disposal();
            //DataTable dtRec;
           // dtFrom.CustomFormat = "MM/dd/yyyy";
           // dtTo.CustomFormat = "MM/dd/yyyy";
           // DateTime dteFrom = ConvertDate.DateConverter(dtFrom.Text);
            //DateTime dteTo = ConvertDate.DateConverter(dtTo.Text);
            
           // dtRec = dteFrom < dteTo ? adj.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), dteFrom,dteTo) : adj.GetAllTransaction(Convert.ToInt32(cboStores.EditValue));
           // gridAdjustments.DataSource = dtRec;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = @"Microsoft Excel | *.xls";

            if (DialogResult.OK == saveDlg.ShowDialog())
            {
                gridAdjustments.MainView.ExportToXls(saveDlg.FileName);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            colApprovedBy.Visible = false;
            colReason.Visible = false;
            printableComponentLink1.CreateMarginalHeaderArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalHeaderArea);
            printableComponentLink1.CreateMarginalFooterArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalFooterArea);
            printableComponentLink1.CreateDocument();
            printableComponentLink1.ShowPreview();

            colApprovedBy.Visible = true;
            colReason.Visible = true;
        }
        private void printableComponentLink1_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
        {
            DataTable dt = (DataTable)gridAdjustments.DataSource;
            DataView dv = new DataView(dt);
            string Reason = "Reason: " + dv[0]["Reason"] as string;
            string ApprovedBy = "Approved By: " + dv[0]["ApprovedBy"] as string;
            PageInfoBrick pib = new PageInfoBrick();
            pib.Format = "Page {0}/{1}";

            //RectangleF r = RectangleF.Empty;
            //r.Height = 20;

            //pib = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pib.Format, Color.Black, r, BorderSide.None);
            //PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, "Print Date " + DateTime.Now.ToShortDateString() + " G.C",
            //                      Color.Black, r, BorderSide.None);
            //brick.Alignment = BrickAlignment.Far;
            pib = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pib.Format, Color.Black, new RectangleF(100, 0, 200, 20), BorderSide.None);
            PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pib.Format + "\n Print Date " + DateTime.Now.ToShortDateString() + " G.C",
                                  Color.Black, new RectangleF(100, 0, 200, 40), BorderSide.None);

            brick.Alignment = BrickAlignment.Far;
            TextBrick brickleft = e.Graph.DrawString(Reason, Color.Navy, new RectangleF(0, 0, 200, 40),
                                        DevExpress.XtraPrinting.BorderSide.None);
            brickleft.Font = new Font("Tahoma", 10);
            brickleft.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);
            TextBrick brickrb = e.Graph.DrawString(ApprovedBy, Color.Navy, new RectangleF(0, 20, 200, 40),
                                   DevExpress.XtraPrinting.BorderSide.None);
            brickrb.Font = new Font("Tahoma", 10);
            brickrb.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);

        }
        private void printableComponentLink1_CreateMarginalHeaderArea(object sender, CreateAreaEventArgs e)
        {
            var info = new GeneralInfo();
            info.LoadAll();
            //var dtDate = new DateTimePickerEx
            //{
            //    Value = DateTime.Now,
            //    CustomFormat = "MM/dd/yyyy"
            //};
            //DateTime dtCurrent = Convert.ToDateTime(dtDate.Text);
            string refNumber = lstTree.FocusedNode.GetDisplayText("RefNo");
            string adjDate = lstTree.FocusedNode.GetDisplayText("Year");
            if ((lstTree.FocusedNode.GetDisplayText("Date"))!="")
                adjDate = (Convert.ToDateTime(lstTree.FocusedNode.GetDisplayText("Date"))).ToShortDateString();

            //string header = info.HospitalName + "\n Loss/Adjustment Activity Log Store: " +cboStores.Text+ " \n RefNo:  " + refNumber + " On " + adjDate + " E.C";

            //printableComponentLink1.Landscape = true;
            //printableComponentLink1.PageHeaderFooter = header;

            //TextBrick brick = e.Graph.DrawString(header, Color.Navy, new RectangleF(0, 0, 800, 80),
            //                                    DevExpress.XtraPrinting.BorderSide.None);
            //brick.Font = new Font("Tahoma", 13);
            //brick.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Center);
            string header = info.HospitalName + "\nStore: " + cboStores.Text;
            string headercenter = "Loss/Adjustment Activity Log";
            string headerRight = " \n RefNo:  " + refNumber + "  \nDate: " + adjDate + " E.C";
          

            TextBrick brickcenter = e.Graph.DrawString(headercenter, Color.Navy, new RectangleF(400, 40, 400, 100),
                                               DevExpress.XtraPrinting.BorderSide.None);
            brickcenter.Font = new Font("Tahoma", 13);
            brickcenter.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);

            TextBrick brick = e.Graph.DrawString(header, Color.Navy, new RectangleF(0, 40, 300, 100),
                                                DevExpress.XtraPrinting.BorderSide.None);
            brick.Font = new Font("Tahoma", 13);
            brick.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);

            TextBrick brickright = e.Graph.DrawString(headerRight, Color.Navy, new RectangleF(800, 20, 400, 100),
                                                DevExpress.XtraPrinting.BorderSide.None);
            brickright.Font = new Font("Tahoma", 13);
            brickright.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);
            printableComponentLink1.Landscape = true;
            printableComponentLink1.PageHeaderFooter = header;
        }
        private static void PclCreateReportHeaderArea(object sender, DevExpress.XtraPrinting.CreateAreaEventArgs e)
        {
            var info = new GeneralInfo();
            info.LoadAll();
            var dtDate = new DateTimePickerEx
                                                      {
                                                          Value = DateTime.Now,
                                                          CustomFormat = "MM/dd/yyyy"
                                                      };
            DateTime dtCurrent = Convert.ToDateTime(dtDate.Text);
            //Old header
            string header = info.HospitalName + " Loss/Adjustment Activity Log " + dtCurrent.ToString("MM dd,yyyy");
            TextBrick brick = e.Graph.DrawString(header, Color.Navy, new RectangleF(0, 0, 500, 40),
                                                 DevExpress.XtraPrinting.BorderSide.None);
            brick.Font = new Font("Arial", 16);
            brick.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Center);
        }

        private void lstTree_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            var dtDate = new DateTimePickerEx
            {
                CustomFormat = "MM/dd/yyyy",
                Value = DateTime.Now
            };
            DateTime dtCurrent = ConvertDate.DateConverter(dtDate.Text);
            var dr = (DataRowView)lstTree.GetDataRecordByNode(lstTree.FocusedNode);
            if (dr == null) return;

            var adj = new Disposal();
            DataTable dtRec;
            if (dr["ParentID"] == DBNull.Value)
            {
                int yr = ((dtCurrent.Month > 10) ? dtCurrent.Year : dtCurrent.Year - 1);
                DateTime dt1 = new DateTime(Convert.ToInt32(dr["ID"]) - 1, 11, 1);
                DateTime dt2 = new DateTime(Convert.ToInt32(dr["ID"]), 11, 1);
                dtRec = adj.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), dt1, dt2);
                lblAdjDate.Text = dr["RefNo"].ToString();
            }
            else
            {
                dtRec = adj.GetDocumentByRefNo(dr["RefNo"].ToString(), Convert.ToInt32(cboStores.EditValue), dr["Date"].ToString());
                lblAdjDate.Text = Convert.ToDateTime(dr["Date"]).ToString("MM dd,yyyy");
            }

            gridAdjustments.DataSource = dtRec;
        }

       
        /// <summary>
        /// Delete a row of adjustmen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var us = new User();
            var userID = MainWindow.LoggedinId;
            us.LoadByPrimaryKey(userID);

            var dataRow = grdLogAdjustment.GetFocusedDataRow();
            if (dataRow == null) return;

            if (us.UserName != "admin")
            {
                XtraMessageBox.Show("You don't have the privilege to update reference number!", "Caution");
                return;
            }
            //get the primary key of the row
            var ID = Convert.ToInt32(dataRow["ID"]);

            var disposal = new Disposal();
            var receiveDoc = new ReceiveDoc();

            //Retrieve the adjustment with the value of the primary key(id)
            disposal.LoadByPrimaryKey(ID);

             int recieveID = disposal.RecID;
           

            receiveDoc.LoadByPrimaryKey(recieveID);
            if (XtraMessageBox.Show("Are You Sure, You want to delete this?", "Confirmation", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //check for losss
             
               
                if (disposal.Losses) //it was loss
                {
                    receiveDoc.LoadByPrimaryKey(recieveID);
                    var s = receiveDoc.Quantity;
                    receiveDoc.Quantity = receiveDoc.Quantity + disposal.Quantity;
                    receiveDoc.QuantityLeft = receiveDoc.QuantityLeft + disposal.Quantity;
                    if (receiveDoc.Out)
                        receiveDoc.Out = false;
                    disposal.Quantity = 0;
                }
                else // it was adjustment
                {
                    receiveDoc.LoadByPrimaryKey(recieveID);
                    var s = receiveDoc.Quantity;
                    receiveDoc.Quantity = receiveDoc.Quantity - disposal.Quantity;
                    receiveDoc.QuantityLeft = receiveDoc.QuantityLeft - disposal.Quantity;
                    if (receiveDoc.Quantity == 0)
                        receiveDoc.Out = true;
                    disposal.Quantity = 0;
                }

                // proceed deletion and make the necessary changes on the database tables.
                DisposalDelete ddel;
                AddDeletedDisposal(disposal, out ddel);

                receiveDoc.Save();
                disposal.MarkAsDeleted();
                disposal.Save();

                //Repopulate the grid
                //DataTable dtRec;
               // dtFrom.CustomFormat = "MM/dd/yyyy";
                //dtTo.CustomFormat = "MM/dd/yyyy";
                //DateTime from = ConvertDate.DateConverter(dtFrom.Text);
                //DateTime to = ConvertDate.DateConverter(dtTo.Text);
                //dtRec = disposal.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), from, to);
             //   gridAdjustments.DataSource = dtRec;
            }
        }

        private static void AddDeletedDisposal(Disposal disposal, out DisposalDelete ddel)
        {
            ddel = new DisposalDelete();
            ddel.AddNew();
            ddel.ID = disposal.ID;
            ddel.ItemID = disposal.ItemID;
            ddel.StoreId = disposal.StoreId;
            ddel.ReasonId = disposal.ReasonId;
            ddel.Quantity = disposal.Quantity;
            ddel.Date = disposal.Date;
            ddel.ApprovedBy = disposal.ApprovedBy;
            ddel.Losses = disposal.Losses;
            ddel.BatchNo = disposal.BatchNo;
            ddel.Remark = disposal.Remark;
            ddel.Cost = disposal.Cost;
            ddel.RefNo = disposal.RefNo;
            ddel.EurDate = disposal.EurDate;
            ddel.RecID = disposal.RecID;
            ddel.Save();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
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
            var edtloss = new EditLossAndAdjustment((string)dr["RefNo"]);
            edtloss.ShowDialog();
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
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
            if (XtraMessageBox.Show("Are You Sure, You want to delete this?", "Confirmation", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
               var dis = new Disposal();
               DataTable dtbl =  dis.GetTransactionByRefNo((string) dr["RefNo"]);
                foreach (DataRow dataRow in dtbl.Rows)
                {
                    AddlossDeleted(dataRow);
                    dataRow.Delete();
                }
                dis.MarkAsDeleted();
                dis.Save();
                XtraMessageBox.Show("Item successfully deleted.","Success");
            }
        }

        private static void AddlossDeleted(DataRow dataRow)
        {
            var dispdelete = new DisposalDelete();
            dispdelete.AddNew();
            dispdelete.ID = (int) dataRow["ID"];
            dispdelete.ItemID = (int) dataRow["ItemID"];
            dispdelete.StoreId = (int) dataRow["StoreID"];
            dispdelete.ReasonId = (int) dataRow["ReasonID"];
            dispdelete.Quantity = (long) dataRow["Quantity"];
            dispdelete.Date = (DateTime) dataRow["Date"];
            //dispdelete.ApprovedBy = (string) dataRow["ApprovedBy"];
            dispdelete.Losses = (bool) dataRow["Losses"];
            dispdelete.BatchNo = (string) dataRow["BatchNo"];
           // dispdelete.Remark = (string) dataRow["Remark"];
            dispdelete.Cost = (double) dataRow["Cost"];
            dispdelete.RefNo = (string) dataRow["RefNo"];
            dispdelete.EurDate = (DateTime) dataRow["EurDate"];
            dispdelete.RecID = (int) dataRow["RecID"];
            dispdelete.Save();
        }

        private void lstTree_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var treeList = sender as TreeList;
            if (treeList != null && treeList.FocusedNode.Id != 0)
            {
                e.Cancel = true;
            }
        }

        private void chkIntDrugCode_CheckedChanged(object sender, EventArgs e)
        {
            grdLogAdjustment.Columns["InternalDrugCode"].Visible = Convert.ToBoolean(chkIntDrugCode.EditValue);
        }
    }
}