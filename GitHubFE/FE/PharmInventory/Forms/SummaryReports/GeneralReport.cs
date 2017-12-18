using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BLL;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using System.Threading;
using DevExpress.XtraPrinting;
using PharmInventory.HelperClasses;

namespace PharmInventory
{
    public partial class GeneralReport : XtraForm
    {
        public GeneralReport()
        {
            InitializeComponent();
        }
        private System.IO.Stream streamToPrint;
        string streamType;

        int storeId = 0;
        int categoryid = 0;
        private void GeneralReport_Load(object sender, EventArgs e)
        {
            Stores stor = new Stores();
            DataTable dtStor = stor.GetActiveStores();
            DataRow dtStorRow = dtStor.NewRow();
            dtStorRow["ID"] = "0";
            dtStorRow["StoreName"] = "All Stores";
            dtStor.Rows.InsertAt(dtStorRow, 0);

            cboStores.DataSource = dtStor;

            var type = new BLL.Type();
            DataTable alltypes = type.GetAllCategory();
            DataRow alltypesRow = alltypes.NewRow();
            alltypesRow["ID"] = "0";
            alltypesRow["Name"] = "All Categories";
            alltypes.Rows.InsertAt(alltypesRow, 0);

            lkCategory.Properties.DataSource = alltypes;
            lkCategory.Properties.DisplayMember = "Name";
            lkCategory.Properties.ValueMember = "ID";
            lkCategory.ItemIndex = 0;

            dtDate.Value = DateTime.Now;
            dtDate.CustomFormat = "MM/dd/yyyy";
            dtCurrent = ConvertDate.DateConverter(dtDate.Text);
            curMont = dtCurrent.Month; // (dtCurrent.Month < 11) ? dtCurrent.Month + 2 : ((dtCurrent.Month == 11) ? 1 : 2);
            curYear = dtCurrent.Year;// (dtCurrent.Month < 11) ? dtCurrent.Year : dtCurrent.Year - 1;

            var dtyears = Items.AllYearsString();
            cboYear.Properties.DataSource = dtyears;
            cboYear.EditValue = curYear;

            var bw = new BackgroundWorker();
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();

            ManageLabelLinksStatus();
        }

        private void ManageLabelLinksStatus()
        {
            #region Stockstatus Summary tab
            listInStock.Visible = lblInStock.Text != "0 (.0%)";
            listStockOut.Visible = lblStockOut.Text != "0 (.0%)";
            listNearEOP.Visible = lblNearEOP.Text != "0 (.0%)";
            listOverstock.Visible = lblOverStocked.Text != "0 (.0%)";
            listBelowEOP.Visible = lblBelowEOP.Text != "0 (.0%)";
            listBelowMin.Visible = lblBelowMin.Text != "0 (.0%)";
            #endregion Stockstatus Summary tab

            #region Never Received Summary tab
            linkLabel10.Visible = lblNeverRecived.Text != "0 (.0%)";
            #endregion Never Received Summary tab
            
            #region Never Issued Summary tab
            linkLabel21.Visible = lblNeverIssued.Text != "0 (.0%)";
            #endregion Never Issued Summary tab
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (storeId == 0)
            {
                if (Convert.ToInt32(lkCategory.EditValue) == 0)
                {
                    PopulateSStatusByYearForAllStore();
                    PopulateStockStatusByYearForAllStore();
                }
                else
                {
                    PopulateSStatusByCategoryAndYearForAllStore();
                    PopulateStockStatusByCategoryAndYearForAllStore();
                }
            }
            else
            {
                if (Convert.ToInt32(lkCategory.EditValue) == 0)
                {
                    PopulateSStatusByYear();
                    PopulateStockStatusByYear();
                }
                else
                {
                    PopulateSStatusByCategoryAndYear();
                    PopulateStockStatusByCategoryAndYear();
                }
            }
        }

        DateTime dtCurrent = new DateTime();
        int curMont = 0;
        int curYear = 0;

        private void PopulateSStatus1()
        {
            if (curMont != 0 && curYear != 0)
            {
                storeId = Convert.ToInt32(cboStores.SelectedValue);

                Balance blnc = new Balance();
                DataTable dtbl = blnc.GetSOH(storeId, curMont, curYear);

                Items itm = new Items();
                Balance bal = new Balance();
                ReceiveDoc rec = new ReceiveDoc();

                Programs prog = new Programs();
                prog.GetProgramByName("Family Planning");
                DataTable dtItm = itm.GetItemsByProgram(prog.ID);
                int totalECLS = dtItm.Rows.Count;
                lblNoECLS.Text = totalECLS.ToString();
                int neverRec = rec.CountNeverReceivedItemsByCateogryAndYear(storeId,categoryid, curYear);
                int stockin = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Normal" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                               && ((ckExclude.Checked)? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (stockin == 0)
                {
                    listInStock.Visible = false;
                }
                else
                {
                    listInStock.Visible = true;
                }
                
                //progressBar1.PerformStep();
                int stockout = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Stock Out" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (stockout == 0)
                {
                    listStockOut.Visible = false;
                }
                else
                {
                    listStockOut.Visible = true;
                }
                //((ckExclude.Checked)? (bal.CountStockOut(storeId, curMont, curYear)- neverRec) : bal.CountStockOut(storeId, curMont, curYear));
                //progressBar1.PerformStep();
                int overstock = (from m in dtbl.AsEnumerable()
                                 where m["Status"].ToString() == "Over Stocked" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                 && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                 select m).Count();
                if (overstock == 0)
                {
                    listOverstock.Visible = false;
                }
                else
                {
                    listOverstock.Visible = true;
                }
                // progressBar1.PerformStep();
                int nearEOP = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Near EOP" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                               && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (nearEOP == 0)
                {
                    listNearEOP.Visible = false;
                }
                else 
                { 
                    listNearEOP.Visible = true; 
                }
                //progressBar1.PerformStep();
                int belowEOP = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Below EOP" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (belowEOP == 0)
                {
                    listBelowEOP.Visible = false;
                }
                else
                {
                    listBelowEOP.Visible = true;
                }
                // progressBar1.PerformStep();
                int belowMin = 0;//bal.CountBelowMin(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                int freeStockOut = (from m in dtbl.AsEnumerable()
                                    where m["Status"].ToString() == "Stock Out" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                    && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                    select m).Count();
                if (freeStockOut == 0)
                {
                    lblFreeStockedout.Visible = false;
                }
                else
                {
                    lblFreeStockedout.Visible = true;
                }
                // progressBar1.PerformStep();
                int vitalStockOut = bal.CountVitalItemsStockOut(storeId, curMont, curYear);
                //int eclsStockout = bal.CountECLSItemsStockOut(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                object[] obj = { stockin, stockout, overstock, nearEOP, belowEOP };
                int totalItm = stockin + stockout + nearEOP + overstock + belowEOP;

                decimal percen = ((totalItm != 0) ? (Convert.ToDecimal(stockin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblInStock.Text = stockin.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(stockout) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblStockOut.Text = stockout.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(overstock) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblOverStocked.Text = overstock.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(nearEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblNearEOP.Text = nearEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowEOP.Text = belowEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowMin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowMin.Text = belowMin.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblFreeStockedout.Text = freeStockOut.ToString();
                if (freeStockOut == 0)
                {
                    linkLabel6.Visible = false;
                }
                else
                {
                    linkLabel6.Visible = true;
                }
                int totalFree = itm.CountFreeItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(freeStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblFreeStock.Text = freeStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblVitalStockedout.Text = vitalStockOut.ToString();
                if (vitalStockOut == 0)
                {
                    linkLabel7.Visible = false;
                }
                else
                {
                    linkLabel7.Visible = true;
                }
                totalFree = itm.CountVitalItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(vitalStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblVitalStock.Text = vitalStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                //totalFree = itm.CountVitalItems();
                //percen = ((totalECLS != 0)?(Convert.ToDecimal(eclsStockout) / Convert.ToDecimal(totalECLS)) * 100:0);
                //percen = Decimal.Round(percen, 0);
                //lblEclsStock.Text = eclsStockout.ToString() + " (" + percen.ToString("#.0") + "%)"; 

                GenerateStockStatusPieChart(obj);
            }
        }

        private void PopulateSStatusByCategoryAndYear()
        {
            if (curMont != 0 && curYear != 0)
            {
                storeId = Convert.ToInt32(cboStores.SelectedValue);
                curYear = Convert.ToInt32(cboYear.EditValue);

                Balance blnc = new Balance();
                DataTable dtbl = blnc.GetSOH(storeId, curMont, curYear);

                Items itm = new Items();
                Balance bal = new Balance();
                ReceiveDoc rec = new ReceiveDoc();

                Programs prog = new Programs();
                prog.GetProgramByName("Family Planning");
                DataTable dtItm = itm.GetItemsByProgram(prog.ID);
                int totalECLS = dtItm.Rows.Count;
                lblNoECLS.Text = totalECLS.ToString();
                int neverRec = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                int stockin = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Normal" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                               && ((ckExclude.Checked)? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (stockin == 0)
                {
                    listInStock.Visible = false;
                }
                else
                {
                    listInStock.Visible = true;
                }
                
                //progressBar1.PerformStep();
                int stockout = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Stock Out" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (stockout == 0)
                {
                    listStockOut.Visible = false;
                }
                else
                {
                    listStockOut.Visible = true;
                }
                //((ckExclude.Checked)? (bal.CountStockOut(storeId, curMont, curYear)- neverRec) : bal.CountStockOut(storeId, curMont, curYear));
                //progressBar1.PerformStep();
                int overstock = (from m in dtbl.AsEnumerable()
                                 where m["Status"].ToString() == "Over Stocked" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                 && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                 select m).Count();
                if (overstock == 0)
                {
                    listOverstock.Visible = false;
                }
                else
                {
                    listOverstock.Visible = true;
                }
                // progressBar1.PerformStep();
                int nearEOP = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Near EOP" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                               && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (nearEOP == 0)
                {
                    listNearEOP.Visible = false;
                }
                else 
                { 
                    listNearEOP.Visible = true; 
                }
                //progressBar1.PerformStep();
                int belowEOP = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Below EOP" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (belowEOP == 0)
                {
                    listBelowEOP.Visible = false;
                }
                else
                {
                    listBelowEOP.Visible = true;
                }
                // progressBar1.PerformStep();
                int belowMin = 0;//bal.CountBelowMin(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                int freeStockOut = (from m in dtbl.AsEnumerable()
                                    where m["Status"].ToString() == "Stock Out" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                    && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                    select m).Count();
                if (freeStockOut == 0)
                {
                    lblFreeStockedout.Visible = false;
                }
                else
                {
                    lblFreeStockedout.Visible = true;
                }
                // progressBar1.PerformStep();
                int vitalStockOut = bal.CountVitalItemsStockOut(storeId, curMont, curYear);
                //int eclsStockout = bal.CountECLSItemsStockOut(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                object[] obj = { stockin, stockout, overstock, nearEOP, belowEOP };
                int totalItm = stockin + stockout + nearEOP + overstock + belowEOP;

                decimal percen = ((totalItm != 0) ? (Convert.ToDecimal(stockin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblInStock.Text = stockin.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(stockout) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblStockOut.Text = stockout.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(overstock) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblOverStocked.Text = overstock.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(nearEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblNearEOP.Text = nearEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowEOP.Text = belowEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowMin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowMin.Text = belowMin.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblFreeStockedout.Text = freeStockOut.ToString();
                if (freeStockOut == 0)
                {
                    linkLabel6.Visible = false;
                }
                else
                {
                    linkLabel6.Visible = true;
                }
                int totalFree = itm.CountFreeItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(freeStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblFreeStock.Text = freeStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblVitalStockedout.Text = vitalStockOut.ToString();
                if (vitalStockOut == 0)
                {
                    linkLabel7.Visible = false;
                }
                else
                {
                    linkLabel7.Visible = true;
                }
                totalFree = itm.CountVitalItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(vitalStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblVitalStock.Text = vitalStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                //totalFree = itm.CountVitalItems();
                //percen = ((totalECLS != 0)?(Convert.ToDecimal(eclsStockout) / Convert.ToDecimal(totalECLS)) * 100:0);
                //percen = Decimal.Round(percen, 0);
                //lblEclsStock.Text = eclsStockout.ToString() + " (" + percen.ToString("#.0") + "%)"; 

                GenerateStockStatusPieChart(obj);
            }
            else
            {
                ReceiveDoc rec = new ReceiveDoc();
                int neverRec = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear);
             }
        }

        private void PopulateSStatusByCategoryAndYearForAllStore()
        {
            if (curMont != 0 && curYear != 0)
            {
                curYear = Convert.ToInt32(cboYear.EditValue);

                Balance blnc = new Balance();
                DataTable dtbl = blnc.GetSOHForAllStores(curMont, curYear);

                Items itm = new Items();
                Balance bal = new Balance();
                ReceiveDoc rec = new ReceiveDoc();

                Programs prog = new Programs();
                prog.GetProgramByName("Family Planning");
                DataTable dtItm = itm.GetItemsByProgram(prog.ID);
                int totalECLS = dtItm.Rows.Count;
                lblNoECLS.Text = totalECLS.ToString();
                int neverRec = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                int stockin = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Normal" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                               && ((ckExclude.Checked)? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (stockin == 0)
                {
                    listInStock.Visible = false;
                }
                else
                {
                    listInStock.Visible = true;
                }
                
                //progressBar1.PerformStep();
                int stockout = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Stock Out" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (stockout == 0)
                {
                    listStockOut.Visible = false;
                }
                else
                {
                    listStockOut.Visible = true;
                }
                //((ckExclude.Checked)? (bal.CountStockOut(storeId, curMont, curYear)- neverRec) : bal.CountStockOut(storeId, curMont, curYear));
                //progressBar1.PerformStep();
                int overstock = (from m in dtbl.AsEnumerable()
                                 where m["Status"].ToString() == "Over Stocked" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                 && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                 select m).Count();
                if (overstock == 0)
                {
                    listOverstock.Visible = false;
                }
                else
                {
                    listOverstock.Visible = true;
                }
                // progressBar1.PerformStep();
                int nearEOP = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Near EOP" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                               && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (nearEOP == 0)
                {
                    listNearEOP.Visible = false;
                }
                else 
                { 
                    listNearEOP.Visible = true; 
                }
                //progressBar1.PerformStep();
                int belowEOP = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Below EOP" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (belowEOP == 0)
                {
                    listBelowEOP.Visible = false;
                }
                else
                {
                    listBelowEOP.Visible = true;
                }
                // progressBar1.PerformStep();
                int belowMin = 0;//bal.CountBelowMin(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                int freeStockOut = (from m in dtbl.AsEnumerable()
                                    where m["Status"].ToString() == "Stock Out" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                    && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                    select m).Count();
                if (freeStockOut == 0)
                {
                    lblFreeStockedout.Visible = false;
                }
                else
                {
                    lblFreeStockedout.Visible = true;
                }
                // progressBar1.PerformStep();
                int vitalStockOut = bal.CountVitalItemsStockOutForAllStore(curMont, curYear);
                //int eclsStockout = bal.CountECLSItemsStockOut(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                object[] obj = { stockin, stockout, overstock, nearEOP, belowEOP };
                int totalItm = stockin + stockout + nearEOP + overstock + belowEOP;

                decimal percen = ((totalItm != 0) ? (Convert.ToDecimal(stockin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblInStock.Text = stockin.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(stockout) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblStockOut.Text = stockout.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(overstock) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblOverStocked.Text = overstock.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(nearEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblNearEOP.Text = nearEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowEOP.Text = belowEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowMin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowMin.Text = belowMin.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblFreeStockedout.Text = freeStockOut.ToString();
                if (freeStockOut == 0)
                {
                    linkLabel6.Visible = false;
                }
                else
                {
                    linkLabel6.Visible = true;
                }
                int totalFree = itm.CountFreeItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(freeStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblFreeStock.Text = freeStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblVitalStockedout.Text = vitalStockOut.ToString();
                if (vitalStockOut == 0)
                {
                    linkLabel7.Visible = false;
                }
                else
                {
                    linkLabel7.Visible = true;
                }
                totalFree = itm.CountVitalItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(vitalStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblVitalStock.Text = vitalStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                //totalFree = itm.CountVitalItems();
                //percen = ((totalECLS != 0)?(Convert.ToDecimal(eclsStockout) / Convert.ToDecimal(totalECLS)) * 100:0);
                //percen = Decimal.Round(percen, 0);
                //lblEclsStock.Text = eclsStockout.ToString() + " (" + percen.ToString("#.0") + "%)"; 

                GenerateStockStatusPieChart(obj);
            }
            else
            {
                ReceiveDoc rec = new ReceiveDoc();
                int neverRec = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear);

            }
        }

        private void PopulateSStatusByYear()
        {
            if (curMont != 0 && curYear != 0)
            {
                storeId = Convert.ToInt32(cboStores.SelectedValue);
                curYear = Convert.ToInt32(cboYear.EditValue);

                Balance blnc = new Balance();
                DataTable dtbl = blnc.GetSOH(storeId, curMont, curYear);

                Items itm = new Items();
                Balance bal = new Balance();
                ReceiveDoc rec = new ReceiveDoc();

                Programs prog = new Programs();
                prog.GetProgramByName("Family Planning");
                DataTable dtItm = itm.GetItemsByProgram(prog.ID);
                int totalECLS = dtItm.Rows.Count;
                lblNoECLS.Text = totalECLS.ToString();
                int neverRec = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                int stockin = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Normal"
                               && ((ckExclude.Checked)? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (stockin == 0)
                {
                    listInStock.Visible = false;
                }
                else
                {
                    listInStock.Visible = true;
                }
                
                //progressBar1.PerformStep();
                int stockout = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Stock Out"
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (stockout == 0)
                {
                    listStockOut.Visible = false;
                }
                else
                {
                    listStockOut.Visible = true;
                }
                //((ckExclude.Checked)? (bal.CountStockOut(storeId, curMont, curYear)- neverRec) : bal.CountStockOut(storeId, curMont, curYear));
                //progressBar1.PerformStep();
                int overstock = (from m in dtbl.AsEnumerable()
                                 where m["Status"].ToString() == "Over Stocked"
                                 && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                 select m).Count();
                if (overstock == 0)
                {
                    listOverstock.Visible = false;
                }
                else
                {
                    listOverstock.Visible = true;
                }
                // progressBar1.PerformStep();
                int nearEOP = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Near EOP"
                               && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (nearEOP == 0)
                {
                    listNearEOP.Visible = false;
                }
                else 
                { 
                    listNearEOP.Visible = true; 
                }
                //progressBar1.PerformStep();
                int belowEOP = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Below EOP"
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (belowEOP == 0)
                {
                    listBelowEOP.Visible = false;
                }
                else
                {
                    listBelowEOP.Visible = true;
                }
                // progressBar1.PerformStep();
                int belowMin = 0;//bal.CountBelowMin(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                int freeStockOut = (from m in dtbl.AsEnumerable()
                                    where m["Status"].ToString() == "Stock Out"
                                    && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                    select m).Count();
                if (freeStockOut == 0)
                {
                    lblFreeStockedout.Visible = false;
                }
                else
                {
                    lblFreeStockedout.Visible = true;
                }
                // progressBar1.PerformStep();
                int vitalStockOut = bal.CountVitalItemsStockOut(storeId, curMont, curYear);
                //int eclsStockout = bal.CountECLSItemsStockOut(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                object[] obj = { stockin, stockout, overstock, nearEOP, belowEOP };
                int totalItm = stockin + stockout + nearEOP + overstock + belowEOP;

                decimal percen = ((totalItm != 0) ? (Convert.ToDecimal(stockin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblInStock.Text = stockin.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(stockout) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblStockOut.Text = stockout.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(overstock) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblOverStocked.Text = overstock.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(nearEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblNearEOP.Text = nearEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowEOP.Text = belowEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowMin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowMin.Text = belowMin.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblFreeStockedout.Text = freeStockOut.ToString();
                if (freeStockOut == 0)
                {
                    linkLabel6.Visible = false;
                }
                else
                {
                    linkLabel6.Visible = true;
                }
                int totalFree = itm.CountFreeItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(freeStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblFreeStock.Text = freeStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblVitalStockedout.Text = vitalStockOut.ToString();
                if (vitalStockOut == 0)
                {
                    linkLabel7.Visible = false;
                }
                else
                {
                    linkLabel7.Visible = true;
                }
                totalFree = itm.CountVitalItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(vitalStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblVitalStock.Text = vitalStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                //totalFree = itm.CountVitalItems();
                //percen = ((totalECLS != 0)?(Convert.ToDecimal(eclsStockout) / Convert.ToDecimal(totalECLS)) * 100:0);
                //percen = Decimal.Round(percen, 0);
                //lblEclsStock.Text = eclsStockout.ToString() + " (" + percen.ToString("#.0") + "%)"; 

                GenerateStockStatusPieChart(obj);
            }
            else
            {
                ReceiveDoc rec = new ReceiveDoc();
                int neverRec = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, categoryid, curYear);

            }
        }

        private void PopulateSStatusByYearForAllStore()
        {
            if (curMont != 0 && curYear != 0)
            {
                curYear = Convert.ToInt32(cboYear.EditValue);

                Balance blnc = new Balance();
                DataTable dtbl = blnc.GetSOHForAllStores(curMont, curYear);

                Items itm = new Items();
                Balance bal = new Balance();
                ReceiveDoc rec = new ReceiveDoc();

                Programs prog = new Programs();
                prog.GetProgramByName("Family Planning");
                DataTable dtItm = itm.GetItemsByProgram(prog.ID);
                int totalECLS = dtItm.Rows.Count;
                lblNoECLS.Text = totalECLS.ToString();
                int neverRec = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                int stockin = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Normal"
                               && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (stockin == 0)
                {
                    listInStock.Visible = false;
                }
                else
                {
                    listInStock.Visible = true;
                }

                //progressBar1.PerformStep();
                int stockout = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Stock Out"
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (stockout == 0)
                {
                    listStockOut.Visible = false;
                }
                else
                {
                    listStockOut.Visible = true;
                }
                //((ckExclude.Checked)? (bal.CountStockOut(storeId, curMont, curYear)- neverRec) : bal.CountStockOut(storeId, curMont, curYear));
                //progressBar1.PerformStep();
                int overstock = (from m in dtbl.AsEnumerable()
                                 where m["Status"].ToString() == "Over Stocked"
                                 && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                 select m).Count();
                if (overstock == 0)
                {
                    listOverstock.Visible = false;
                }
                else
                {
                    listOverstock.Visible = true;
                }
                // progressBar1.PerformStep();
                int nearEOP = (from m in dtbl.AsEnumerable()
                               where m["Status"].ToString() == "Near EOP"
                               && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                               select m).Count();
                if (nearEOP == 0)
                {
                    listNearEOP.Visible = false;
                }
                else
                {
                    listNearEOP.Visible = true;
                }
                //progressBar1.PerformStep();
                int belowEOP = (from m in dtbl.AsEnumerable()
                                where m["Status"].ToString() == "Below EOP"
                                && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                select m).Count();
                if (belowEOP == 0)
                {
                    listBelowEOP.Visible = false;
                }
                else
                {
                    listBelowEOP.Visible = true;
                }
                // progressBar1.PerformStep();
                int belowMin = 0;//bal.CountBelowMin(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                int freeStockOut = (from m in dtbl.AsEnumerable()
                                    where m["Status"].ToString() == "Stock Out"
                                    && ((ckExclude.Checked) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                    select m).Count();
                if (freeStockOut == 0)
                {
                    lblFreeStockedout.Visible = false;
                }
                else
                {
                    lblFreeStockedout.Visible = true;
                }
                // progressBar1.PerformStep();
                int vitalStockOut = bal.CountVitalItemsStockOutForAllStore(curMont, curYear);
                //int eclsStockout = bal.CountECLSItemsStockOut(storeId, curMont, curYear);
                // progressBar1.PerformStep();
                object[] obj = { stockin, stockout, overstock, nearEOP, belowEOP };
                int totalItm = stockin + stockout + nearEOP + overstock + belowEOP;

                decimal percen = ((totalItm != 0) ? (Convert.ToDecimal(stockin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblInStock.Text = stockin.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(stockout) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblStockOut.Text = stockout.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(overstock) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblOverStocked.Text = overstock.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(nearEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblNearEOP.Text = nearEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowEOP) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowEOP.Text = belowEOP.ToString() + " (" + percen.ToString("#.0") + "%)";

                percen = ((totalItm != 0) ? (Convert.ToDecimal(belowMin) / Convert.ToDecimal(totalItm)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblBelowMin.Text = belowMin.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblFreeStockedout.Text = freeStockOut.ToString();
                if (freeStockOut == 0)
                {
                    linkLabel6.Visible = false;
                }
                else
                {
                    linkLabel6.Visible = true;
                }
                int totalFree = itm.CountFreeItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(freeStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblFreeStock.Text = freeStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                lblVitalStockedout.Text = vitalStockOut.ToString();
                if (vitalStockOut == 0)
                {
                    linkLabel7.Visible = false;
                }
                else
                {
                    linkLabel7.Visible = true;
                }
                totalFree = itm.CountVitalItems();
                percen = ((totalFree != 0) ? (Convert.ToDecimal(vitalStockOut) / Convert.ToDecimal(totalFree)) * 100 : 0);
                percen = Decimal.Round(percen, 0);
                lblVitalStock.Text = vitalStockOut.ToString() + " (" + percen.ToString("#.0") + "%)";

                //totalFree = itm.CountVitalItems();
                //percen = ((totalECLS != 0)?(Convert.ToDecimal(eclsStockout) / Convert.ToDecimal(totalECLS)) * 100:0);
                //percen = Decimal.Round(percen, 0);
                //lblEclsStock.Text = eclsStockout.ToString() + " (" + percen.ToString("#.0") + "%)"; 

                GenerateStockStatusPieChart(obj);
            }
            else {
                ReceiveDoc rec = new ReceiveDoc();
                int neverRec = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, categoryid,curYear); 

            }
        }

        private void PopulateStockStatus()
        {
           // progressBar1.Visible = true;

            if (curMont != 0 && curYear != 0)
            {
                IssueDoc iss = new IssueDoc();
                ReceiveDoc rec = new ReceiveDoc();
                Balance blnc = new Balance();

                DataTable dtbl = blnc.GetSOH(storeId, curMont, curYear);

                DateTime lastRec = rec.GetLastReceivedDate(storeId);
                DateTime lastIss = iss.GetLastIssuedDate(storeId);
               //time here
                TimeSpan tt = new TimeSpan();
                string noDays = "";
                tt = new TimeSpan(dtCurrent.Ticks - lastRec.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";

                lblLastReceived.Text = (tt.TotalDays < 30000) ? lastRec.ToString("MM dd,yyyy") : "Never";
                lblRecDays.Text = noDays;
                //progressBar1.PerformStep();
                tt = new TimeSpan(dtCurrent.Ticks - lastIss.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";
                lblLastIssued.Text = (tt.TotalDays < 30000) ? lastIss.ToString("MM dd,yyyy") : "Never";
                lblIssuedDays.Text = noDays;

                DataTable dtStockout = (from m in dtbl.AsEnumerable()
                                        where m["Status"].ToString() == "Normal"
                                        && ((ckExclude.Checked == true)? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                        select m).CopyToDataTable();
                //StatusGroup.Text = "Stocked In Items";
                PopulateList(dtStockout, listStatused);

                DataTable dtFreeStockOut = (from m in dtbl.AsEnumerable()
                                            where m["Status"].ToString() == "Stock Out"
                                            && ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                            select m).CopyToDataTable();
                //groupStatusTrend.Text = "Stocked out Free Items ";
                PopulateList(dtFreeStockOut, listStatused);

                DataTable dtRec = rec.GetTopReceivedItems(storeId);
               // groupRecSummary.Text = "Top 10 Most Received Items";
                PopulateList(dtRec, listReceiveSum);
                lblNeverRecived.Text = rec.CountNeverReceivedItemsByCateogryAndYear(storeId,categoryid, curYear).ToString();
                if (lblNeverRecived.Text == "0")
                {
                    linkLabel10.Visible = false;
                }
                else 
                {
                    linkLabel10.Visible = true;
                }
                lblNeverIssued.Text = rec.CountReceivedNotIssuedItems(storeId, curYear).ToString();
                if (lblNeverIssued.Text == "0")
                {
                    linkLabel21.Visible = false;
                }
                else
                {
                    linkLabel21.Visible = true;
                }
               // progressBar1.PerformStep();
                DataTable dtIss = iss.GetTopIssuedItems(storeId);
               // groupIssued.Text = "Top 10 Most Issued Items";
                PopulateList(dtIss, listIssued);

                //Balance bal = new Balance();
                GeneralInfo info = new GeneralInfo();
                info.LoadAll();
                DataTable dtList = new DataTable();
                DataTable dtCons = new DataTable();
                string[] co = { "Ham", "Neh", "Mes", "Tek", "Hed", "Tah", "Tir", "Yek", "Meg", "Miz", "Gen", "Sen" };

                //foreach(string s in co)
                //{
                dtList.Columns.Add("Month");
                dtList.Columns.Add("Value");
                dtList.Columns[1].DataType = typeof(Int64);

                dtCons.Columns.Add("Month");
                dtCons.Columns.Add("Value");
                dtCons.Columns[1].DataType = typeof(Int64);

                int[] mon = { 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                double[] cos = new double[12];
                DataTable dtBal = new DataTable();
                Items recd = new Items();
               //time here
                int year = dtCurrent.Year;
                
                chartReceiveCost.Series.Clear();

                Series ser = new Series("Received Cost In Birr", ViewType.Line);
                ser.DataSource = dtList;
                ser.ArgumentScaleType = ScaleType.Qualitative;
                ser.ArgumentDataMember = "Month";
                ser.ValueScaleType = ScaleType.Numerical;
                
                ser.ValueDataMembers.AddRange(new string[] { "Value" });
                chartReceiveCost.Series.Add(ser);

                chartIssueCost.Series.Clear();

                Series serIss = new Series("Issued Cost In Birr", ViewType.Line);
                serIss.DataSource = dtCons;
                serIss.ArgumentScaleType = ScaleType.Qualitative;
                serIss.ArgumentDataMember = "Month";
                serIss.ValueScaleType = ScaleType.Numerical;

                serIss.ValueDataMembers.AddRange(new string[] { "Value" });
                chartIssueCost.Series.Add(serIss);

            }
        }

        private void PopulateStockStatusByCategoryAndYear()
        {
           // progressBar1.Visible = true;

            if (curMont != 0 && curYear != 0)
            {
                IssueDoc iss = new IssueDoc();
                ReceiveDoc rec = new ReceiveDoc();
                Balance blnc = new Balance();

                DataTable dtbl = blnc.GetSOH(storeId, curMont, curYear);

                DateTime lastRec = rec.GetLastReceivedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                DateTime lastIss = iss.GetLastIssuedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
               //time here
                TimeSpan tt = new TimeSpan();
                string noDays = "";
                tt = new TimeSpan(dtCurrent.Ticks - lastRec.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";

                lblLastReceived.Text = (tt.TotalDays < 30000) ? lastRec.ToString("MM dd,yyyy") : "Never";
                lblRecDays.Text = noDays;
                //progressBar1.PerformStep();
                tt = new TimeSpan(dtCurrent.Ticks - lastIss.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";
                lblLastIssued.Text = (tt.TotalDays < 30000) ? lastIss.ToString("MM dd,yyyy") : "Never";
                lblIssuedDays.Text = noDays;

                DataTable dtStockout;
                try
                {
                    dtStockout = (from m in dtbl.AsEnumerable()
                                            where m["Status"].ToString() == "Normal" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                            && ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                            select m).CopyToDataTable();
                }
                catch (InvalidOperationException)
                {
                    dtStockout = null;
                }

                PopulateList(dtStockout, listStatused);

                DataTable dtFreeStockOut;
                try
                {
                    dtFreeStockOut = (from m in dtbl.AsEnumerable()
                                                where m["Status"].ToString() == "Stock Out" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                                && ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                                select m).CopyToDataTable();
                }
                catch (InvalidOperationException)
                {
                    dtFreeStockOut = null;
                }

                PopulateList(dtFreeStockOut, listStatused);

                DataTable dtRec = rec.GetTopReceivedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
               // groupRecSummary.Text = "Top 10 Most Received Items";
                PopulateList(dtRec, listReceiveSum);
                lblNeverRecived.Text = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)).ToString();
                if (rec.CountNeverReceivedItemsByCateogryAndYear(storeId, categoryid,curYear) == 0)
                {
                    linkLabel10.Visible = false;
                }
                else 
                {
                    linkLabel10.Visible = true;
                }
                lblNeverIssued.Text = rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)).ToString();
                if (rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)) == 0)
                {
                    linkLabel21.Visible = false;
                }
                else
                {
                    linkLabel21.Visible = true;
                }
               // progressBar1.PerformStep();
                DataTable dtIss = iss.GetTopIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
               // groupIssued.Text = "Top 10 Most Issued Items";
                PopulateList(dtIss, listIssued);

                //Balance bal = new Balance();
                GeneralInfo info = new GeneralInfo();
                info.LoadAll();
                DataTable dtList = new DataTable();
                DataTable dtCons = new DataTable();
                string[] co = { "Ham", "Neh", "Mes", "Tek", "Hed", "Tah", "Tir", "Yek", "Meg", "Miz", "Gen", "Sen" };

                //foreach(string s in co)
                //{
                dtList.Columns.Add("Month");
                dtList.Columns.Add("Value");
                dtList.Columns[1].DataType = typeof(Int64);

                dtCons.Columns.Add("Month");
                dtCons.Columns.Add("Value");
                dtCons.Columns[1].DataType = typeof(Int64);

                int[] mon = { 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                double[] cos = new double[12];
                DataTable dtBal = new DataTable();
                Items recd = new Items();
               //time here
                int year = dtCurrent.Year;
                
                chartReceiveCost.Series.Clear();

                Series ser = new Series("Received Cost In Birr", ViewType.Line);
                ser.DataSource = dtList;
                ser.ArgumentScaleType = ScaleType.Qualitative;
                ser.ArgumentDataMember = "Month";
                ser.ValueScaleType = ScaleType.Numerical;
                
                ser.ValueDataMembers.AddRange(new string[] { "Value" });
                chartReceiveCost.Series.Add(ser);

                chartIssueCost.Series.Clear();

                Series serIss = new Series("Issued Cost In Birr", ViewType.Line);
                serIss.DataSource = dtCons;
                serIss.ArgumentScaleType = ScaleType.Qualitative;
                serIss.ArgumentDataMember = "Month";
                serIss.ValueScaleType = ScaleType.Numerical;

                serIss.ValueDataMembers.AddRange(new string[] { "Value" });
                chartIssueCost.Series.Add(serIss);

            }
            else
            {
                ReceiveDoc rec = new ReceiveDoc();
                DataTable dtRec = rec.GetTopReceivedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear);
                // groupRecSummary.Text = "Top 10 Most Received Items";
                PopulateList(dtRec, listReceiveSum);
                lblNeverRecived.Text = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue),  curYear).ToString();
                if (rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear) == 0)
                {
                    linkLabel10.Visible = false;
                }
                else
                {
                    linkLabel10.Visible = true;
                }
                DateTime lastRec = rec.GetLastReceivedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue),curYear);
                //time here
                TimeSpan tt = new TimeSpan();
                string noDays = "";
                tt = new TimeSpan(dtCurrent.Ticks - lastRec.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";

                lblLastReceived.Text = (tt.TotalDays < 30000) ? lastRec.ToString("MM dd,yyyy") : "Never";
                lblRecDays.Text = noDays;

                IssueDoc iss = new IssueDoc(); 
                DateTime lastIss = iss.GetLastIssuedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear);
              
                //progressBar1.PerformStep();
                tt = new TimeSpan(dtCurrent.Ticks - lastIss.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";
                lblLastIssued.Text = (tt.TotalDays < 30000) ? lastIss.ToString("MM dd,yyyy") : "Never";
                lblIssuedDays.Text = noDays;

                lblNeverIssued.Text = rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId,  Convert.ToInt32(lkCategory.EditValue), curYear).ToString();
                if (rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue),  curYear) == 0)
                {
                    linkLabel21.Visible = false;
                }
                else
                {
                    linkLabel21.Visible = true;
                }
                // progressBar1.PerformStep();
                DataTable dtIss = iss.GetTopIssuedItemsByCategoryAndYear(storeId, categoryid, curYear);
                // groupIssued.Text = "Top 10 Most Issued Items";
                PopulateList(dtIss, listIssued);
            }
        }

        private void PopulateStockStatusByCategoryAndYearForAllStore()
        {
            // progressBar1.Visible = true;

            if (curMont != 0 && curYear != 0)
            {
                IssueDoc iss = new IssueDoc();
                ReceiveDoc rec = new ReceiveDoc();
                Balance blnc = new Balance();

                DataTable dtbl = blnc.GetSOHForAllStores(curMont, curYear);

                DateTime lastRec = rec.GetLastReceivedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                DateTime lastIss = iss.GetLastIssuedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                //time here
                TimeSpan tt = new TimeSpan();
                string noDays = "";
                tt = new TimeSpan(dtCurrent.Ticks - lastRec.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";

                lblLastReceived.Text = (tt.TotalDays < 30000) ? lastRec.ToString("MM dd,yyyy") : "Never";
                lblRecDays.Text = noDays;
                //progressBar1.PerformStep();
                tt = new TimeSpan(dtCurrent.Ticks - lastIss.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";
                lblLastIssued.Text = (tt.TotalDays < 30000) ? lastIss.ToString("MM dd,yyyy") : "Never";
                lblIssuedDays.Text = noDays;

                DataTable dtStockout;
                try
                {
                    dtStockout = (from m in dtbl.AsEnumerable()
                                  where m["Status"].ToString() == "Normal" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                  && ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                  select m).CopyToDataTable();
                }
                catch (InvalidOperationException)
                {
                    dtStockout = null;
                }

                PopulateList(dtStockout, listStatused);

                DataTable dtFreeStockOut;
                try
                {
                    dtFreeStockOut = (from m in dtbl.AsEnumerable()
                                      where m["Status"].ToString() == "Stock Out" && Convert.ToInt32(m["TypeID"]) == Convert.ToInt32(lkCategory.EditValue)
                                      && ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                      select m).CopyToDataTable();
                }
                catch (InvalidOperationException)
                {
                    dtFreeStockOut = null;
                }

                PopulateList(dtFreeStockOut, listStatused);

                DataTable dtRec = rec.GetTopReceivedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                // groupRecSummary.Text = "Top 10 Most Received Items";
                PopulateList(dtRec, listReceiveSum);
                lblNeverRecived.Text = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)).ToString();
                if (rec.CountNeverReceivedItemsByCateogryAndYear(storeId, categoryid, curYear) == 0)
                {
                    linkLabel10.Visible = false;
                }
                else
                {
                    linkLabel10.Visible = true;
                }
                lblNeverIssued.Text = rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)).ToString();
                if (lblNeverIssued.Text == "0")
                {
                    linkLabel21.Visible = false;
                }
                else
                {
                    linkLabel21.Visible = true;
                }
                // progressBar1.PerformStep();
                DataTable dtIss = iss.GetTopIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                // groupIssued.Text = "Top 10 Most Issued Items";
                PopulateList(dtIss, listIssued);

                //Balance bal = new Balance();
                GeneralInfo info = new GeneralInfo();
                info.LoadAll();
                DataTable dtList = new DataTable();
                DataTable dtCons = new DataTable();
                string[] co = { "Ham", "Neh", "Mes", "Tek", "Hed", "Tah", "Tir", "Yek", "Meg", "Miz", "Gen", "Sen" };

                //foreach(string s in co)
                //{
                dtList.Columns.Add("Month");
                dtList.Columns.Add("Value");
                dtList.Columns[1].DataType = typeof(Int64);

                dtCons.Columns.Add("Month");
                dtCons.Columns.Add("Value");
                dtCons.Columns[1].DataType = typeof(Int64);

                int[] mon = { 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                double[] cos = new double[12];
                DataTable dtBal = new DataTable();
                Items recd = new Items();
                //time here
                int year = dtCurrent.Year;

                chartReceiveCost.Series.Clear();

                Series ser = new Series("Received Cost In Birr", ViewType.Line);
                ser.DataSource = dtList;
                ser.ArgumentScaleType = ScaleType.Qualitative;
                ser.ArgumentDataMember = "Month";
                ser.ValueScaleType = ScaleType.Numerical;

                ser.ValueDataMembers.AddRange(new string[] { "Value" });
                chartReceiveCost.Series.Add(ser);

                chartIssueCost.Series.Clear();

                Series serIss = new Series("Issued Cost In Birr", ViewType.Line);
                serIss.DataSource = dtCons;
                serIss.ArgumentScaleType = ScaleType.Qualitative;
                serIss.ArgumentDataMember = "Month";
                serIss.ValueScaleType = ScaleType.Numerical;

                serIss.ValueDataMembers.AddRange(new string[] { "Value" });
                chartIssueCost.Series.Add(serIss);

            }
            else
            {
                ReceiveDoc rec = new ReceiveDoc();
                DataTable dtRec = rec.GetTopReceivedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear);
                // groupRecSummary.Text = "Top 10 Most Received Items";
                PopulateList(dtRec, listReceiveSum);
                lblNeverRecived.Text = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear).ToString();
                if (rec.CountNeverReceivedItemsByCateogryAndYear(storeId, categoryid, curYear) == 0)
                {
                    linkLabel10.Visible = false;
                }
                else
                {
                    linkLabel10.Visible = true;
                }
                DateTime lastRec = rec.GetLastReceivedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear);
                 //time here
                TimeSpan tt = new TimeSpan();
                string noDays = "";
                tt = new TimeSpan(dtCurrent.Ticks - lastRec.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";

                lblLastReceived.Text = (tt.TotalDays < 30000) ? lastRec.ToString("MM dd,yyyy") : "Never";
                lblRecDays.Text = noDays;

                IssueDoc iss = new IssueDoc(); 
               DateTime lastIss = iss.GetLastIssuedDateByCategoryAndYear(storeId, categoryid,curYear);
                
                tt = new TimeSpan(dtCurrent.Ticks - lastIss.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";
                lblLastIssued.Text = (tt.TotalDays < 30000) ? lastIss.ToString("MM dd,yyyy") : "Never";
                lblIssuedDays.Text = noDays;

               lblNeverIssued.Text = rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, categoryid, curYear).ToString();
                if (lblNeverIssued.Text == "0")
                {
                    linkLabel21.Visible = false;
                }
                else
                {
                    linkLabel21.Visible = true;
                }
                // progressBar1.PerformStep();
                DataTable dtIss = iss.GetTopIssuedItemsByCategoryAndYear(storeId, categoryid, curYear);
                // groupIssued.Text = "Top 10 Most Issued Items";
                PopulateList(dtIss, listIssued);


            }
        }

        private void PopulateStockStatusByYear()
        {
           // progressBar1.Visible = true;

            if (curMont != 0 && curYear != 0)
            {
                IssueDoc iss = new IssueDoc();
                ReceiveDoc rec = new ReceiveDoc();
                Balance blnc = new Balance();

                DataTable dtbl = blnc.GetSOH(storeId, curMont, curYear);

                DateTime lastRec = rec.GetLastReceivedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                DateTime lastIss = iss.GetLastIssuedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
               //time here
                TimeSpan tt = new TimeSpan();
                string noDays = "";
                tt = new TimeSpan(dtCurrent.Ticks - lastRec.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";

                lblLastReceived.Text = (tt.TotalDays < 30000) ? lastRec.ToString("MM dd,yyyy") : "Never";
                lblRecDays.Text = noDays;
                //progressBar1.PerformStep();
                tt = new TimeSpan(dtCurrent.Ticks - lastIss.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";
                lblLastIssued.Text = (tt.TotalDays < 30000) ? lastIss.ToString("MM dd,yyyy") : "Never";
                lblIssuedDays.Text = noDays;

                DataTable dtAllItems;
                try
                {
                    dtAllItems = (from m in dtbl.AsEnumerable()
                                            where ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                            select m).CopyToDataTable();
                }
                catch (InvalidOperationException)
                {
                    dtAllItems = null;
                }

                PopulateList(dtAllItems, listStatused);

                DataTable dtRec = rec.GetTopReceivedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
               // groupRecSummary.Text = "Top 10 Most Received Items";
                PopulateList(dtRec, listReceiveSum);
                lblNeverRecived.Text = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)).ToString();
                if (rec.CountNeverReceivedItemsByCateogryAndYear(storeId,categoryid, curYear) == 0)
                {
                    linkLabel10.Visible = false;
                }
                else 
                {
                    linkLabel10.Visible = true;
                }
                lblNeverIssued.Text = rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)).ToString();
                if (rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)) == 0)
                {
                    linkLabel21.Visible = false;
                }
                else
                {
                    linkLabel21.Visible = true;
                }
               // progressBar1.PerformStep();
                DataTable dtIss = iss.GetTopIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
               // groupIssued.Text = "Top 10 Most Issued Items";
                PopulateList(dtIss, listIssued);

                //Balance bal = new Balance();
                GeneralInfo info = new GeneralInfo();
                info.LoadAll();
                DataTable dtList = new DataTable();
                DataTable dtCons = new DataTable();
                string[] co = { "Ham", "Neh", "Mes", "Tek", "Hed", "Tah", "Tir", "Yek", "Meg", "Miz", "Gen", "Sen" };

                //foreach(string s in co)
                //{
                dtList.Columns.Add("Month");
                dtList.Columns.Add("Value");
                dtList.Columns[1].DataType = typeof(Int64);

                dtCons.Columns.Add("Month");
                dtCons.Columns.Add("Value");
                dtCons.Columns[1].DataType = typeof(Int64);

                int[] mon = { 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                double[] cos = new double[12];
                DataTable dtBal = new DataTable();
                Items recd = new Items();
               //time here
                int year = dtCurrent.Year;
                
                chartReceiveCost.Series.Clear();

                Series ser = new Series("Received Cost In Birr", ViewType.Line);
                ser.DataSource = dtList;
                ser.ArgumentScaleType = ScaleType.Qualitative;
                ser.ArgumentDataMember = "Month";
                ser.ValueScaleType = ScaleType.Numerical;
                
                ser.ValueDataMembers.AddRange(new string[] { "Value" });
                chartReceiveCost.Series.Add(ser);

                chartIssueCost.Series.Clear();

                Series serIss = new Series("Issued Cost In Birr", ViewType.Line);
                serIss.DataSource = dtCons;
                serIss.ArgumentScaleType = ScaleType.Qualitative;
                serIss.ArgumentDataMember = "Month";
                serIss.ValueScaleType = ScaleType.Numerical;

                serIss.ValueDataMembers.AddRange(new string[] { "Value" });
                chartIssueCost.Series.Add(serIss);

            }
            else
            {
                ReceiveDoc rec = new ReceiveDoc();
                DataTable dtRec = rec.GetTopReceivedItemsByCategoryAndYear(storeId, categoryid, curYear);
                // groupRecSummary.Text = "Top 10 Most Received Items";
                PopulateList(dtRec, listReceiveSum);
                lblNeverRecived.Text = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, categoryid,curYear).ToString();
                if (rec.CountNeverReceivedItemsByCateogryAndYear(storeId,categoryid, curYear) == 0)
                {
                    linkLabel10.Visible = false;
                }
                else
                {
                    linkLabel10.Visible = true;
                }
                DateTime lastRec = rec.GetLastReceivedDateByCategoryAndYear(storeId, categoryid,  curYear);
                 //time here
                TimeSpan tt = new TimeSpan();
                string noDays = "";
                tt = new TimeSpan(dtCurrent.Ticks - lastRec.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";

                lblLastReceived.Text = (tt.TotalDays < 30000) ? lastRec.ToString("MM dd,yyyy") : "Never";
                lblRecDays.Text = noDays;

                lblNeverIssued.Text = rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, categoryid, curYear).ToString();
                if (rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, categoryid, curYear) == 0)
                {
                    linkLabel21.Visible = false;
                }
                else
                {
                    linkLabel21.Visible = true;
                }
            }
        }

        private void PopulateStockStatusByYearForAllStore()
        {
            // progressBar1.Visible = true;

            if (curMont != 0 && curYear != 0)
            {
                IssueDoc iss = new IssueDoc();
                ReceiveDoc rec = new ReceiveDoc();
                Balance blnc = new Balance();

                DataTable dtbl = blnc.GetSOHForAllStores(curMont, curYear);

                DateTime lastRec = rec.GetLastReceivedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                DateTime lastIss = iss.GetLastIssuedDateByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                //time here
                TimeSpan tt = new TimeSpan();
                string noDays = "";
                tt = new TimeSpan(dtCurrent.Ticks - lastRec.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";

                lblLastReceived.Text = (tt.TotalDays < 30000) ? lastRec.ToString("MM dd,yyyy") : "Never";
                lblRecDays.Text = noDays;
                //progressBar1.PerformStep();
                tt = new TimeSpan(dtCurrent.Ticks - lastIss.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";
                lblLastIssued.Text = (tt.TotalDays < 30000) ? lastIss.ToString("MM dd,yyyy") : "Never";
                lblIssuedDays.Text = noDays;

                DataTable dtAllItems;
                try
                {
                    dtAllItems = (from m in dtbl.AsEnumerable()
                                  where ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                  select m).CopyToDataTable();
                }
                catch (InvalidOperationException)
                {
                    dtAllItems = null;
                }

                PopulateList(dtAllItems, listStatused);

                DataTable dtRec = rec.GetTopReceivedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                // groupRecSummary.Text = "Top 10 Most Received Items";
                PopulateList(dtRec, listReceiveSum);
                lblNeverRecived.Text = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)).ToString();
                if (rec.CountNeverReceivedItemsByCateogryAndYear(storeId,categoryid, curYear) == 0)
                {
                    linkLabel10.Visible = false;
                }
                else
                {
                    linkLabel10.Visible = true;
                }
                lblNeverIssued.Text = rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue)).ToString();
                if (lblNeverIssued.Text == "0")
                {
                    linkLabel21.Visible = false;
                }
                else
                {
                    linkLabel21.Visible = true;
                }
                // progressBar1.PerformStep();
                DataTable dtIss = iss.GetTopIssuedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboYear.EditValue));
                // groupIssued.Text = "Top 10 Most Issued Items";
                PopulateList(dtIss, listIssued);

                //Balance bal = new Balance();
                GeneralInfo info = new GeneralInfo();
                info.LoadAll();
                DataTable dtList = new DataTable();
                DataTable dtCons = new DataTable();
                string[] co = { "Ham", "Neh", "Mes", "Tek", "Hed", "Tah", "Tir", "Yek", "Meg", "Miz", "Gen", "Sen" };

                //foreach(string s in co)
                //{
                dtList.Columns.Add("Month");
                dtList.Columns.Add("Value");
                dtList.Columns[1].DataType = typeof(Int64);

                dtCons.Columns.Add("Month");
                dtCons.Columns.Add("Value");
                dtCons.Columns[1].DataType = typeof(Int64);

                int[] mon = { 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                double[] cos = new double[12];
                DataTable dtBal = new DataTable();
                Items recd = new Items();
                //time here
                int year = dtCurrent.Year;

                chartReceiveCost.Series.Clear();

                Series ser = new Series("Received Cost In Birr", ViewType.Line);
                ser.DataSource = dtList;
                ser.ArgumentScaleType = ScaleType.Qualitative;
                ser.ArgumentDataMember = "Month";
                ser.ValueScaleType = ScaleType.Numerical;

                ser.ValueDataMembers.AddRange(new string[] { "Value" });
                chartReceiveCost.Series.Add(ser);

                chartIssueCost.Series.Clear();

                Series serIss = new Series("Issued Cost In Birr", ViewType.Line);
                serIss.DataSource = dtCons;
                serIss.ArgumentScaleType = ScaleType.Qualitative;
                serIss.ArgumentDataMember = "Month";
                serIss.ValueScaleType = ScaleType.Numerical;

                serIss.ValueDataMembers.AddRange(new string[] { "Value" });
                chartIssueCost.Series.Add(serIss);

            }
            else
            {
                ReceiveDoc rec = new ReceiveDoc();
                     lblNeverRecived.Text = rec.CountNeverReceivedItemsByCateogryAndYear(storeId, categoryid, curYear).ToString();
                if (rec.CountNeverReceivedItemsByCateogryAndYear(storeId,categoryid, curYear) == 0)
                {
                    linkLabel10.Visible = false;
                }
                else
                {
                    linkLabel10.Visible = true;
                }
                DateTime lastRec = rec.GetLastReceivedDateByCategoryAndYear(storeId, categoryid,curYear);
                TimeSpan tt = new TimeSpan();
                string noDays = "";
                tt = new TimeSpan(dtCurrent.Ticks - lastRec.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";

                lblLastReceived.Text = (tt.TotalDays < 30000) ? lastRec.ToString("MM dd,yyyy") : "Never";
                lblRecDays.Text = noDays;

                IssueDoc iss = new IssueDoc();
                DateTime lastIss = iss.GetLastIssuedDateByCategoryAndYear(storeId, categoryid, curYear);
                 
                tt = new TimeSpan(dtCurrent.Ticks - lastIss.Ticks);
                noDays = (tt.TotalDays < 30000) ? tt.TotalDays.ToString() + " Days" : "Never";
                lblLastIssued.Text = (tt.TotalDays < 30000) ? lastIss.ToString("MM dd,yyyy") : "Never";
                lblIssuedDays.Text = noDays;
          
                lblNeverIssued.Text = rec.CountReceivedNotIssuedItemsByCategoryAndYear(storeId,categoryid,curYear).ToString();
                if (lblNeverIssued.Text == "0")
                {
                    linkLabel21.Visible = false;
                }
                else
                {
                    linkLabel21.Visible = true;
                }
                // progressBar1.PerformStep();
                DataTable dtIss = iss.GetTopIssuedItemsByCategoryAndYear(storeId, categoryid, curYear);
                // groupIssued.Text = "Top 10 Most Issued Items";
                PopulateList(dtIss, listIssued);

            }
        }

        private void GenerateStockStatusPieChart(Object[] obj)
        {
            chartPie.Series.Clear();
            
            DataTable dtList = new DataTable();
            dtList.Columns.Add("Type");
            dtList.Columns.Add("Value");
            dtList.Columns[1].DataType = typeof(Int64);
            object[] oo = {"Stock In",obj[0]};
            dtList.Rows.Add(oo);

            object[] oo2 = { "Stock Out", obj[1] };
            dtList.Rows.Add(oo2);

            //object[] oo6 = { "Below Min", obj[5] };
            //dtList.Rows.Add(oo6);

            object[] oo3 = { "OverStock", obj[2] };
            dtList.Rows.Add(oo3);

            object[] oo4 = { "Near EOP", obj[3] };
            dtList.Rows.Add(oo4);

            object[] oo5 = { "Below EOP", obj[4] };
            dtList.Rows.Add(oo5);

            

            Series ser = new Series("pie", ViewType.Pie3D);
            ser.DataSource = dtList;
            
            ser.ArgumentScaleType = ScaleType.Qualitative;
            ser.ArgumentDataMember = "Type";
            ser.ValueScaleType = ScaleType.Numerical;
            ser.ValueDataMembers.AddRange(new string[] { "Value" });
            ser.PointOptions.PointView = PointView.Argument;
            ser.LegendText = "Key";
           
            ser.PointOptions.ValueNumericOptions.Format =  NumericFormat.Percent;
            ((PieSeriesLabel)ser.Label).Position = PieSeriesLabelPosition.TwoColumns;
            ((PiePointOptions)ser.PointOptions).PointView = PointView.ArgumentAndValues;
            ser.PointOptions.ValueNumericOptions.Precision = 0;
            chartPie.Series.Add(ser);
            //chartPie.Size = new System.Drawing.Size(500, 300);
        }

        private void PopulateInventory()
        {
            Items itm = new Items();
            lblAll.Text = itm.CountAllItems().ToString();
            if (itm.CountAllItems() == 0)
            {
                listAll.Visible = false;
                listAll.Text = "";
            }
            lblNotEDL.Text = itm.CountEDLItems().ToString();
            if (itm.CountEDLItems() == 0)
            {
                listNotEDL.Visible = false;
                listNotEDL.Text = "";
            }
            lblFree.Text = itm.CountFreeItems().ToString();
            if (itm.CountFreeItems() == 0)
            {
                listFree.Visible = false;
                listFree.Text = "";
            }
            lblRefrigerated.Text = itm.CountRefrigeratedItems().ToString();
            if (itm.CountRefrigeratedItems() == 0)
            {
                listRefrigerated.Visible = false;
                listRefrigerated.Text = "";
            }
            lblPediatric.Text = itm.CountPediatricItems().ToString();
            if (itm.CountPediatricItems() == 0)
            {
                listPediatric.Visible = false;
                listPediatric.Text = "";
            }
        }

        private void listAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Items itm = new Items();
            DataTable dtItm = itm.GetAllItems(1);
            //groupList.Text = "All Items";
            PopulateList(dtItm,lstDetail);
        }

        private void PopulateList(DataTable dtItm,GridControl lst)
        {
            lst.DataSource = dtItm;
            if (Convert.ToInt32(lkCategory.EditValue) != 0)
            {
                gridView2.ActiveFilterString = String.Format("TypeID ={0}", Convert.ToInt32(lkCategory.EditValue));
            }
        }

        private void listNotEDL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Items itm = new Items();
            DataTable dtItm = itm.GetEDLItems();
           // groupList.Text = "Items Not in EDL";
            PopulateList(dtItm,lstDetail);
        }

        private void listFree_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Items itm = new Items();
            if (itm.GetFreeItems().Rows.Count != 0)
            {
                DataTable dtItm = itm.GetFreeItems();
                // groupList.Text = "Free Items";
                PopulateList(dtItm, lstDetail);
            }
            else
            { 
                //nothing happenes
            }
        }

        private void listRefrigerated_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Items itm = new Items();
            DataTable dtItm = itm.GetRefrigeratedItems();
           // groupList.Text = "Refrigerated Items";
            PopulateList(dtItm,lstDetail);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void listPediatric_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Items itm = new Items();
            DataTable dtItm = itm.GetPediatricItems();
           // groupList.Text = "Pediatric Items";
            PopulateList(dtItm,lstDetail);
        }

        private void listInStock_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Balance bal = new Balance();
            DataTable stockout = bal.GetStockIn(storeId, curMont, curYear , Convert.ToInt32(lkCategory.EditValue));
            //StatusGroup.Text = "Stocked In Items";
            DataTable dtStockout = (from m in stockout.AsEnumerable()
                                    where  ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true )
                                    select m).CopyToDataTable();
            PopulateList(dtStockout, listStatused);
        }

        private void listStockOut_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Balance bal = new Balance();
            DataTable stockout = bal.GetStockOut(storeId, curMont, curYear, Convert.ToInt32(lkCategory.EditValue));
            DataTable dtStockout = (from m in stockout.AsEnumerable()
                                    where  ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                    select m).CopyToDataTable();
                //((ckExclude.Checked) ? bal.GetReceivedStockOut(storeId, curMont, curYear) :);
           // StatusGroup.Text = "Stocked Out Items";
            PopulateList(dtStockout, listStatused);           
        }

        private void listNearEOP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //if (lblNearEOP.Text)
            Balance bal = new Balance();
            DataTable stockout = bal.GetNearEOP(storeId, curMont, curYear, Convert.ToInt32(lkCategory.EditValue));
           // StatusGroup.Text = "Near EOP Items";
            DataTable dtStockout = (from m in stockout.AsEnumerable()
                                    where ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                    select m).CopyToDataTable();
            PopulateList(stockout, listStatused);  
        }

        private void listOverstock_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Balance bal = new Balance();
            DataTable stockout = bal.GetOverStock(storeId, curMont, curYear, Convert.ToInt32(lkCategory.EditValue));
            //StatusGroup.Text = "Over Stocked Items";
            DataTable dtStockout = (from m in stockout.AsEnumerable()
                                    where  ((ckExclude.Checked == true) ? Convert.ToInt32(m["EverReceived"]) == 1 : true)
                                    select m).CopyToDataTable();
            PopulateList(stockout, listStatused);  
        }

        private void cboStores_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((lkCategory.EditValue != null) && (cboYear.EditValue != null))
            {

                curYear = (cboYear.EditValue != DBNull.Value)?Convert.ToInt32(cboYear.EditValue):0; 

                storeId = (cboStores.SelectedValue != null) ? Convert.ToInt32(cboStores.SelectedValue) : 1;

                if (storeId == 0)
                {
                    if (Convert.ToInt32(lkCategory.EditValue) == 0)
                    {
                        PopulateSStatusByYearForAllStore();
                        PopulateStockStatusByYearForAllStore();
                    }
                    else
                    {
                        PopulateSStatusByCategoryAndYearForAllStore();
                        PopulateStockStatusByCategoryAndYearForAllStore();
                    }
                }
                else
                {
                    if (Convert.ToInt32(lkCategory.EditValue) == 0)
                    {
                        PopulateSStatusByYear();
                        PopulateStockStatusByYear();
                    }
                    else
                    {
                        PopulateSStatusByCategoryAndYear();
                        PopulateStockStatusByCategoryAndYear();
                    }
                }
            }

            ManageLabelLinksStatus();
        }

        private void tabPage8_Click(object sender, EventArgs e)
        {

        }

        private void listDetail_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (listDetail.SelectedItems[0].Tag != null)
            //{
            //    dtDate.Value = DateTime.Now;
            //    dtDate.CustomFormat = "MM/dd/yyyy";
            //    DateTime dtCur = Convert.ToDateTime(dtDate.Text);
            //    int month = dtCur.Month;
            //    int year = dtCur.Year;

            //    int itemId = Convert.ToInt32(listDetail.SelectedItems[0].Tag);
            //    ItemDetailReport con = new ItemDetailReport(itemId, Convert.ToInt32(cboStores.SelectedValue), year,0);
            //    con.ShowDialog();
            //}
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Balance bal = new Balance();
            DataTable dtFreeStockOut =  bal.GetFreeItemsStockOut(storeId,curMont,curYear ,Convert.ToInt32(lkCategory.EditValue));
           // groupStatusTrend.Text = "Stocked out Free Items ";
            PopulateList(dtFreeStockOut, listStatusTrend);
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Balance bal = new Balance();
            DataTable dtVitalStockOut = bal.GetVitalItemsStockOut(storeId, curMont, curYear);
            //groupStatusTrend.Text = "Stocked out Vital Items ";
            PopulateList(dtVitalStockOut, listStatusTrend);
        }

        private void PopulateStatusTrend()
        { 
            
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReceiveDoc rec = new ReceiveDoc();
            DataTable dtRec = rec.GetTopReceivedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear);
            //groupRecSummary.Text = "Top 10 Most Received Items";
            PopulateList(dtRec, listReceiveSum);
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReceiveDoc rec = new ReceiveDoc();
            DataTable dtRec = rec.GetLeastReceivedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear);
            //groupRecSummary.Text = "Top 10 Least Received Items";
            PopulateList(dtRec, listReceiveSum);
        }

        private void linkLabel17_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IssueDoc iss = new IssueDoc();
            categoryid = (lkCategory.EditValue != DBNull.Value) ? Convert.ToInt32(lkCategory.EditValue) : 0;
            DataTable dtIss = iss.GetTopIssuedItemsByCategoryAndYear(storeId, categoryid, curYear);
            //groupIssued.Text = "Top 10 Most Issued Items";
            PopulateList(dtIss, listIssued);
        }

        private void linkLabel16_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IssueDoc iss = new IssueDoc();
            categoryid = (lkCategory.EditValue != DBNull.Value) ? Convert.ToInt32(lkCategory.EditValue) : 0;
            DataTable dtIss = iss.GetLeastIssuedItemsByCategoryAndYear(storeId, categoryid, curYear);
            //groupIssued.Text = "Top 10 Least Issued Items";
            PopulateList(dtIss, listIssued);
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReceiveDoc rec = new ReceiveDoc();
            DataTable dtRec = rec.GetNeverReceivedItemsByCategoryAndYear(storeId, Convert.ToInt32(lkCategory.EditValue), curYear);
            //groupRecSummary.Text = "Never Received Items";
            PopulateList(dtRec, listReceiveSum);
        }

        private void listStatused_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (listStatused.SelectedItems[0].Tag != null)
            //{
            //    dtDate.Value = DateTime.Now;
            //    dtDate.CustomFormat = "MM/dd/yyyy";
            //    DateTime dtCur = Convert.ToDateTime(dtDate.Text);
            //    int month = dtCur.Month;
            //    int year = dtCur.Year;

            //    int itemId = Convert.ToInt32(listStatused.SelectedItems[0].Tag);
            //    ItemDetailReport con = new ItemDetailReport(itemId, Convert.ToInt32(cboStores.SelectedValue), year,0);
            //    con.ShowDialog();
            //}
        }

        private void listStatusTrend_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (listStatused.SelectedItems[0].Tag != null)
            //{
            //    dtDate.Value = DateTime.Now;
            //    dtDate.CustomFormat = "MM/dd/yyyy";
            //    DateTime dtCur = Convert.ToDateTime(dtDate.Text);
            //    int month = dtCur.Month;
            //    int year = dtCur.Year;

            //    int itemId = Convert.ToInt32(listStatused.SelectedItems[0].Tag);
            //    ItemDetailReport con = new ItemDetailReport(itemId, Convert.ToInt32(cboStores.SelectedValue), year,0);
            //    con.ShowDialog();
            //}
        }

        private void listReceiveSum_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (listReceiveSum.SelectedItems[0].Tag != null)
            //{
            //    dtDate.Value = DateTime.Now;
            //    dtDate.CustomFormat = "MM/dd/yyyy";
            //    DateTime dtCur = Convert.ToDateTime(dtDate.Text);
            //    int month = dtCur.Month;
            //    int year = dtCur.Year;

            //    int itemId = Convert.ToInt32(listReceiveSum.SelectedItems[0].Tag);
            //    ItemDetailReport con = new ItemDetailReport(itemId, Convert.ToInt32(cboStores.SelectedValue), year,0);
            //    con.ShowDialog();
            //}
        }

        private void listIssued_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (listIssued.SelectedItems[0].Tag != null)
            //{
            //    dtDate.Value = DateTime.Now;
            //    dtDate.CustomFormat = "MM/dd/yyyy";
            //    DateTime dtCur = Convert.ToDateTime(dtDate.Text);
            //    int month = dtCur.Month;
            //    int year = dtCur.Year;

            //    int itemId = Convert.ToInt32(listIssued.SelectedItems[0].Tag);
            //    ItemDetailReport con = new ItemDetailReport(itemId, Convert.ToInt32(cboStores.SelectedValue), year,0);
            //    con.ShowDialog();
            //}
        }

        private void linkLabel19_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Balance bal = new Balance();
            //DataTable dtFreeStockOut = bal.GetFreeItemsStockOut(storeId, curMont, curYear);
            //groupList.Text = "Stocked out Free Items ";
            //PopulateList(dtFreeStockOut, listDetail);
        }

        private void linkLabel18_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Balance bal = new Balance();
            DataTable dtVitalStockOut = bal.GetVitalItemsStockOut(storeId, curMont, curYear);
            //groupList.Text = "Stocked out Vital Items ";
            PopulateList(dtVitalStockOut, lstDetail);
        }

        private void linkLabel20_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Items itm = new Items();
            Programs prog = new Programs();            
            prog.GetProgramByName("Family Planning");
            DataTable dtItm = itm.GetItemsByProgram(prog.ID);
            //groupList.Text = "ECLS Items ";
            PopulateList(dtItm, lstDetail);
        }

        private void linkLabel21_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReceiveDoc rec = new ReceiveDoc();
            categoryid = (lkCategory.EditValue != DBNull.Value) ? Convert.ToInt32(lkCategory.EditValue) : 0;
            DataTable dtRec = rec.GetReceivedNotIssuedItemsByCategoryAndYear(storeId,categoryid , curYear);
            //groupIssued.Text = "Received But Never Issued Items";
            PopulateList(dtRec, listIssued);
        }

        private void ckExclude_CheckedChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lkCategory.EditValue) == 0)
            {
                PopulateSStatusByYear();
                PopulateStockStatusByYear();
            }
            else
            {
                PopulateSStatusByCategoryAndYear();
                PopulateStockStatusByCategoryAndYear();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            GeneralInfo info = new GeneralInfo();
            info.LoadAll();
            dtDate.Value = DateTime.Now;
            dtDate.CustomFormat = "MM/dd/yyyy";
            string[] header = { info.HospitalName, " Summery Report ", cboStores.Text, " Date: " + dtDate.Text };

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/SummaryReport" + (new Random()).Next(1, 1000) + ".xls";
            if (tabControl1.SelectedTabPage == tabPage8)
            {
                XlsExportOptions opts2 = new XlsExportOptions();
                opts2.SheetName = "Receive Summary";
                listReceiveSum.ExportToXls(filePath, opts2);
                System.Diagnostics.Process.Start(filePath);
            }
            else if (tabControl1.SelectedTabPage == tabPage9)
            {
                XlsExportOptions opts3 = new XlsExportOptions();
                opts3.SheetName = "Issue Summary";
                listIssued.ExportToXls(filePath, opts3);
                System.Diagnostics.Process.Start(filePath);
            }
            else
            {
                XlsExportOptions opts1 = new XlsExportOptions();
                opts1.SheetName = "Stock Status Summary";
                listStatused.ExportToXls(filePath, opts1);
                System.Diagnostics.Process.Start(filePath);
            }
        }

        private void linkLabel22_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Balance bal = new Balance();
            DataTable dtECLSStockOut = bal.GetECLSItemsStockOut(storeId, curMont, curYear);
           // groupList.Text = "Stocked out ECLS Items ";
            PopulateList(dtECLSStockOut, lstDetail);
        }

        private void linkLabel23_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Balance bal = new Balance();
            DataTable stockout = bal.GetBelowEOP(storeId, curMont, curYear ,Convert.ToInt32(lkCategory.EditValue));
            //StatusGroup.Text = "Below EOP Items";
            PopulateList(stockout, listStatused);

        }

        private void linkLabel24_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Balance bal = new Balance();
            DataTable stockout = bal.GetBelowMin(storeId, curMont, curYear);
            //StatusGroup.Text = "Below Min Items";
            PopulateList(stockout, listStatused);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void listDetail_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tabControl1.SelectedTabPageIndex == 0)
                cboStores.Visible = false;
            else
                cboStores.Visible = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            chartPie.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Stretch;
            printableComponentLink1.CreateMarginalHeaderArea += Link_CreateMarginalHeaderArea;
            printableComponentLink1.CreateDocument(false);
            //printableComponentLink1.PrintingSystem.Document.AutoFitToPagesWidth = 1;
            printableComponentLink1.ShowPreview();
        }

        private void Link_CreateMarginalHeaderArea(object sender, CreateAreaEventArgs e)
        {
            var info = new GeneralInfo();
            info.LoadAll();
            cboYear.EditValue = (cboYear.EditValue != DBNull.Value) ? cboYear.EditValue : dtCurrent.Year;
            int year = Convert.ToInt32(cboYear.EditValue) ;
           
            int month = 10;
            int day = 30;
            if (year == dtCurrent.Year)
            {
                month = dtCurrent.Month;
                day = dtCurrent.Day;
            }

            DateTime startDate = EthiopianDate.EthiopianDate.EthiopianToGregorian(String.Format("{0}/{1}/{2}", 1, 11, year - 1));
            DateTime endDate = EthiopianDate.EthiopianDate.EthiopianToGregorian(String.Format("{0}/{1}/{2}", day, month, year));
            DateTime currentDate = EthiopianDate.EthiopianDate.EthiopianToGregorian(String.Format("{0}/{1}/{2}", dtCurrent.Day, dtCurrent.Month, dtCurrent.Year));

            string strStartDate = EthiopianDate.EthiopianDate.GregorianToEthiopian(startDate);
            string strEndDate = EthiopianDate.EthiopianDate.GregorianToEthiopian(endDate);
            string strCurrentDate = EthiopianDate.EthiopianDate.GregorianToEthiopian(currentDate);

            string[] header = { info.HospitalName,  "Store: " + cboStores.Text ,"From Start Date: " + strStartDate + " To End Date: " + strEndDate, "Printed Date: " + strCurrentDate };
            printableComponentLink1.Landscape = true;
            printableComponentLink1.PageHeaderFooter = header;

            TextBrick brick = e.Graph.DrawString(header[0], Color.DarkBlue, new RectangleF(0, 20, 400, 100), BorderSide.None);
            TextBrick brick1 = e.Graph.DrawString(header[1], Color.DarkBlue, new RectangleF(0, 40,400, 100), BorderSide.None);
            TextBrick brick2 = e.Graph.DrawString(header[2], Color.DarkBlue, new RectangleF(0, 60, 400, 100), BorderSide.None);
            TextBrick brick3 = e.Graph.DrawString(header[3], Color.DarkBlue, new RectangleF(0, 80, 400, 100), BorderSide.None);
            //TextBrick brick4 = e.Graph.DrawString(header[4], Color.DarkBlue, new RectangleF(0,80, 400, 100), BorderSide.None);
            //TextBrick brick5 = e.Graph.DrawString(header[5], Color.DarkBlue, new RectangleF(0, 80, 400, 100), BorderSide.None);
        }

        private void btnPrintGrid_Click(object sender, EventArgs e)
        {
            pcl.CreateMarginalHeaderArea += Link_CreateMarginalHeaderArea;
            pcl.CreateDocument();
            pcl.ShowPreview();
        }

        private void btnPrintReceiveSummaryGrid_Click(object sender, EventArgs e)
        {
            pclReceive.CreateMarginalHeaderArea += Link_CreateMarginalHeaderArea;
            pclReceive.CreateDocument();
            pclReceive.ShowPreview();
        }

        private void btnPrintIssueSummaryGrid_Click(object sender, EventArgs e)
        {
            pclIssue.CreateMarginalHeaderArea += Link_CreateMarginalHeaderArea;
            pclIssue.CreateDocument();
            pclIssue.ShowPreview();
        }
    }
}
