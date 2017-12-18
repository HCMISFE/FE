using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using DevExpress.XtraPrinting;

namespace PharmInventory.Forms.SummaryReports
{
    public partial class ActivityLogReports : Form
    {
        private DataTable dtRec,dtiss;
        private ReceiveDoc rec = new ReceiveDoc();
        private IssueDoc iss = new IssueDoc();
        bool isIssue = false; 

        public ActivityLogReports()
        {
            InitializeComponent();
        }

        private void ActivityLogReports_Load(object sender, EventArgs e)
        {
            Stores stor = new Stores();
            DataTable dtStor = stor.GetActiveStores();
            DataRow dtStorRow = dtStor.NewRow();
            dtStorRow["ID"] = "0";
            dtStorRow["StoreName"] = "All Stores";
            dtStor.Rows.InsertAt(dtStorRow, 0);

            cboStores.Properties.DataSource = dtStor;

            DateTime dateFrom = dtFrom.Value;
            DateTime dateTo = dtTo.Value;

            Supplier sp = new Supplier();
            DataTable dtsupp = sp.GetActiveSuppliers();
            DataRow dtrsupp = dtsupp.NewRow();
            dtrsupp["ID"] = "0";
            dtrsupp["CompanyName"] = "All Suppliers";
            dtsupp.Rows.InsertAt(dtrsupp, 0);
            lkSupplier.Properties.DataSource = dtsupp;

            ReceivingUnits ru = new ReceivingUnits();
            DataTable dtru = ru.GetAllApplicableDU();
            DataRow dtrru = dtru.NewRow();
            dtrru["ID"] = "0";
            dtrru["Name"] = "All Receiving Units";
            dtru.Rows.InsertAt(dtrru, 0);
            lklocation.Properties.DataSource = dtru;

            dtRec = rec.GetAllReceiveByDateRange(dateFrom, dateTo);
            dtiss  = iss.GetIssuedByDateRange(dateFrom, dateTo);
            gridReceives.DataSource = dtRec;
            gridIssues.DataSource = dtiss;
            grdViewIssued.ActiveFilterString = String.Format("Quantity<>0");
            grdViewIssued.Columns["InternalDrugCode"].Visible = grdViewReceive.Columns["InternalDrugCode"].Visible = false;
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            RefreshFilter();
        }

        private void cboStores_EditValueChanged(object sender, EventArgs e)
        {
            RefreshFilter();
        }
        private void RefreshFilter()
        {
            if ((cboStores.Text == "All Stores") || (cboStores.EditValue== null))
            {
                dtRec = rec.GetAllReceiveByDateRange(dtFrom.Value, dtTo.Value);
                dtiss = iss.GetIssuedByDateRange(dtFrom.Value, dtTo.Value);
            }
            else
            {
                dtRec = rec.GetAllReceiveByStoreDateRange(Convert.ToInt32(cboStores.EditValue), dtFrom.Value, dtTo.Value);
                dtiss = iss.GetTransactionByDateRange(Convert.ToInt32(cboStores.EditValue), dtFrom.Value, dtTo.Value);
            }
                
            gridReceives.DataSource = dtRec;
            gridIssues.DataSource = dtiss;
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            RefreshFilter();
        }

        private void lklocation_EditValueChanged(object sender, EventArgs e)
        {
            if (lklocation.Text != "All Receiving Units")
                grdViewIssued.ActiveFilterString = string.Format("[IssuedTo]='{0}'", lklocation.Text);
            else
                grdViewIssued.ActiveFilterString = "";
        }

        private void chkIntDrugCode_CheckedChanged(object sender, EventArgs e)
        { 
            grdViewIssued.Columns["InternalDrugCode"].Visible= grdViewReceive.Columns["InternalDrugCode"].Visible = Convert.ToBoolean(chkIntDrugCode.EditValue);
         }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            isIssue = true;
            printableComponentLink2.CreateMarginalHeaderArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalHeaderArea);
            printableComponentLink2.CreateMarginalFooterArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalFooterArea);
            printableComponentLink2.CreateDocument();
            printableComponentLink2.ShowPreview();
        }

        private void printableComponentLink1_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
        {
            PageInfoBrick pib = new PageInfoBrick();
            pib.Format = "Page {0}/{1}";

            RectangleF r = RectangleF.Empty;
            r.Height = 20;

            pib = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pib.Format, Color.Black, r, BorderSide.None);
            PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, "Print Date " + DateTime.Now.ToShortDateString() + " G.C",
                                  Color.Black, r, BorderSide.None);
            brick.Alignment = BrickAlignment.Far;

        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            isIssue = false;
            printableComponentLink1.CreateMarginalHeaderArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalHeaderArea);
            printableComponentLink1.CreateMarginalFooterArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalFooterArea);
            printableComponentLink1.CreateDocument();
            printableComponentLink1.ShowPreview();
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
            var headCenter = "Activity Log Report";
            TextBrick brickcenter = e.Graph.DrawString(headCenter, Color.Navy, new RectangleF(0, 20, 1000, 120),
                                              DevExpress.XtraPrinting.BorderSide.None);
            brickcenter.Font = new Font("Tahoma", 13);
            brickcenter.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Center);

            var comHeader = info.HospitalName + "\nStore: " + cboStores.Text;
            string header = (!isIssue) ? comHeader + "\nReceived From " + dtFrom.Text + " To " + dtTo.Text + " E.C \nSupplier: " + lkSupplier.Text :
                     comHeader+ "\nIssued From " + dtFrom.Text + " To " + dtTo.Text + " E.C \nIssue Location: " + lklocation.Text;

            printableComponentLink1.Landscape = true;
            printableComponentLink1.PageHeaderFooter = header; 
             
            TextBrick brick = e.Graph.DrawString(header, Color.Navy, new RectangleF(0, 40, 1000, 120),
                                                DevExpress.XtraPrinting.BorderSide.None);
            brick.Font = new Font("Tahoma", 12);
            brick.StringFormat = new DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Near);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            string fileName = MainWindow.GetNewFileName("xls");
            gridReceives.ExportToXls(fileName);
            MainWindow.OpenInExcel(fileName);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = MainWindow.GetNewFileName("xls");
            gridIssues.ExportToXls(fileName);
            MainWindow.OpenInExcel(fileName);
        }

        private void lkSupplier_EditValueChanged(object sender, EventArgs e)
        {
            if (lkSupplier.Text != "All Suppliers")
                grdViewReceive.ActiveFilterString = string.Format("[CompanyName]='{0}'", lkSupplier.Text);
            else
                grdViewReceive.ActiveFilterString = "";
        }
    }
}
