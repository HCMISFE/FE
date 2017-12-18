using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraReports.UI;
using PharmInventory.HelperClasses;
using PharmInventory.Reports;

namespace PharmInventory.Forms.Reports
{
    public partial class vrfmainForm : Form
    {
        private int _storeID;
        private int _requestedquantity;
        private int _fromYear;
        private int _toYear;
        private int _fromMonth;
        private int _toMonth;
        private int _programID;
        private DataTable tblRRF;
        private DataTable _tblrrf;
        public vrfmainForm()
        {
            InitializeComponent();
        }

        private void VRFForm_Load(object sender, EventArgs e)
        {
            var type = new BLL.Type();
            var alltypes = type.GetAllCategory();
            categorybindingSource.DataSource = alltypes.DefaultView;

            var program = new Programs();
            var programs = program.GetSubPrograms();
            cboProgram.Properties.DataSource = programs;
            cboProgram.Properties.DisplayMember = "Name";
            cboProgram.Properties.ValueMember = "ID";

            PopulateTheMonthCombos(cboFromMonth);
            PopulateTheMonthCombos(cboToMonth);
            PopulateTheYearCombo(cboFromYear);
            PopulateTheYearCombo(cboToYear);
            var stor = new Stores();
            stor.GetActiveStores();
            cboStores.Properties.DataSource = stor.DefaultView;
            PopulateVRFs();

            WindowVisibility(false);

            var prog = new Programs();
            prog.GetSubPrograms();
            cboProgram.Properties.DataSource = prog.DefaultView;
            cboProgram.EditValue = 1000;
        }

        private static void PopulateTheYearCombo(ComboBoxEdit combo)
        {
            int[] years = new int[25];
            for (int i = 0; i < 25; i++)
            {
                years[i] = 2003 + i;
            }
            combo.Properties.Items.AddRange(years);
        }


        private static void PopulateTheMonthCombos(ComboBoxEdit combo)
        {
            int[] months = new int[12];
            for (int i = 0; i < 12; i++)
            {
                months[i] = i + 1;
            }
            combo.Properties.Items.AddRange(months);
        }

        private void PopulateVRFs()
        {
            VRF vrf = new VRF();
            grdRRF.DataSource = vrf.GetSavedVRFForDisplay();
            _tblrrf = vrf.GetSavedVRFForDisplay();
            grdRRF.RefreshDataSource();
        }

        private void WindowVisibility(bool rrfDetailVisible)
        {
            if (rrfDetailVisible)
            {
                lcRRFInformation.Visibility = LayoutVisibility.Always;
                lcRRFList.Visibility = LayoutVisibility.Never;
            }
            else
            {
                lcRRFInformation.Visibility = LayoutVisibility.Never;
                lcRRFList.Visibility = LayoutVisibility.Always;
            }

        }

        private void grdRRF_DoubleClick(object sender, EventArgs e)
        {
            DataRow dr = grdViewRRFList.GetFocusedDataRow();
            if (dr == null)
                return;
            int rrfID = Convert.ToInt32(dr["ID"]);
            ShowRRFDetailWindow(rrfID);
            WindowVisibility(true);
        }

        private void ShowRRFDetailWindow(int rrfID)
        {
            Cursor = Cursors.WaitCursor;
            RRF rrf = new RRF();
            rrf.LoadByPrimaryKey(rrfID);
            cboFromMonth.EditValue = rrf.FromMonth;
            cboFromYear.EditValue = rrf.FromYear;
            cboToMonth.EditValue = rrf.ToMonth;
            cboToYear.EditValue = rrf.ToYear;
            cboStores.EditValue = rrf.RRFType;
            PopulateList();
            Cursor = Cursors.Default;
        }

        private void PopulateList()
        {
             Items itm = new Items();
            _storeID = Convert.ToInt32(cboStores.EditValue);
            _programID = Convert.ToInt32(cboProgram.EditValue);


            _fromMonth = int.Parse(cboFromMonth.EditValue.ToString());
            _toMonth = int.Parse(cboToMonth.EditValue.ToString());
            _toYear = int.Parse(cboToYear.EditValue.ToString());
            _fromYear = int.Parse(cboFromYear.EditValue.ToString());
             tblRRF = itm.GetVRFReport(_storeID, _fromYear, _fromMonth, _toYear, _toMonth);
             gridItemsChoice.DataSource = tblRRF;
         //    ChooseGridView();
        }

        

        private void btnGenerateRRF_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            PopulateList();
            Cursor = Cursors.Default;
        }

        private void btnNewRRF_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var ethiopianDate = new EthiopianDate.EthiopianDate();
            int currentMonth = ethiopianDate.Month;
            int currentYear = ethiopianDate.Year;
            int startingMonth = GetStartingMonth(currentMonth);
            int startingYear = GetStartingYear(currentMonth, currentYear);
            cboFromMonth.EditValue = startingMonth;
            cboFromYear.EditValue = startingYear;
            SetEndingMonthAndYear(startingMonth, startingYear);
            cboStores.ItemIndex = 1;
            WindowVisibility(true);
            Cursor = Cursors.Default;
        }

        private static int GetStartingMonth(int currentMonth)
        {
            int startingMonth;
            if (currentMonth > 2)
            {
                startingMonth = currentMonth - 2;
            }
            else
            {
                startingMonth = 12 - currentMonth - 1;
            }

            return startingMonth;
        }

        private static int GetStartingYear(int currentMonth, int currentYear)
        {
            if (currentMonth <= 2)
            {
                return currentYear - 1;
            }
            return currentYear;
        }

        private void SetEndingMonthAndYear(int startingMonth, int startingYear)
        {
            if (startingMonth <= 11)
            {
                cboToMonth.EditValue = startingMonth;
                cboToYear.EditValue = startingYear;
            }

            else
            //If the starting month is the 12th month. (The period will be from Nehassie of last year - Meskerem of the next year)
            {
                cboToMonth.EditValue = 1;
                cboToYear.EditValue = startingYear;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            WindowVisibility(false);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //if (VisibilitySetting.HandleUnits == 3) chkCalculateInPacks.Enabled = false;
            //chkCalculateInPacks.Checked = false;
            if (
                XtraMessageBox.Show("Are you sure you want to save and print the VRF?", "Confirm",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            SaveVRF();
            var ginfo = new GeneralInfo();
            ginfo.LoadAll();
            var ethioDateFrom = new EthiopianDate.EthiopianDate(_fromYear, _fromMonth, 1);
            var vrfReport = new VRFReport
            {
                FacilityName = { Text = ginfo.HospitalName },
                Period =
                {
                    Text =
                        string.Format("{0}, {1}", ethioDateFrom.GetMonthName(),
                                      ethioDateFrom.Year)
                },
                ProgramName = { Text = cboProgram.Text },
                categoryName = { Text = lkCategory.Text },
                RegionZoneWoreda = {Text = ginfo.DSUpdateFrequency},
                ResponsiblePerson = { Text = ginfo.HospitalContact },
                TelephoneNumber = { Text = ginfo.Telephone},
                MonthsToSupply = { Text = ginfo.RRFStatusUpdateFrequency },
                Births = { Text = ginfo.RRFStatusFirstUpdateAfter },
                SurvivingInfants = { Text = ginfo.ScmsWSUserName} ,
                RequestDate = { Text =  DateTime.Now.ToShortDateString()}

            };

            var tbl1 = ((DataView)gridItemChoiceView.DataSource).Table;
            tbl1.TableName = "DataTable11";
            var dtset = new DataSet();
            dtset.Tables.Add(tbl1.Copy());
            vrfReport.DataSource = dtset;
            if (Convert.ToInt32(cboProgram.EditValue) == 0 && Convert.ToInt32(lkCategory.EditValue) == 0)
            {
                vrfReport.ShowPreviewDialog();
            }
            else if (Convert.ToInt32(lkCategory.EditValue) != 0)
            {
                vrfReport.FilterString = String.Format("ProgramID={0} and TypeID ={1}", Convert.ToInt32(cboProgram.EditValue), Convert.ToInt32(lkCategory.EditValue));
                vrfReport.ShowPreviewDialog();
            }
            else
            {
                vrfReport.FilterString = String.Format("ProgramID={0}", Convert.ToInt32(cboProgram.EditValue));
                vrfReport.ShowPreviewDialog();
            }

        }

        private int SaveVRF()
        {
            var vrf = new VRF();
            if (vrf.VRFExists(_storeID, _fromYear, _fromMonth, _toYear, _toMonth))
            {
                if (XtraMessageBox.Show("VRF Exists on disk, are you sure you want to replace it?", "RRF Save",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return -1;
            }
            var rrfID = vrf.AddNewVRF(_storeID, _fromYear, _fromMonth, _toYear, _toMonth, true);
            var dtbl1 = new DataTable();
            if (gridItemChoiceView.DataSource != null) 
                dtbl1 = ((DataView)gridItemChoiceView.DataSource).Table;
            foreach (DataRow dr in dtbl1.Rows)
            {
                var itemID = Convert.ToInt32(dr["ID"]);
                var requestedqty = Convert.ToInt32(dr["QuantityOrdered"]);
                var storeID = int.Parse(cboStores.EditValue.ToString());
                var doses = Convert.ToInt32(dr["Doses"]);
                var wasteFactor = Convert.ToDecimal(dr["WasteFactor"]);
                var targetcoverage = Convert.ToInt32(dr["TargetCoverage"]);
                var vacigiven = Convert.ToInt32(dr["VaccinationGiven"]);
                var remark = Convert.ToString(dr["Remark"]);

              
                var rrfDetail = new VRFDetail();
                rrfDetail.AddNewVRFDetail(rrfID, storeID, itemID, requestedqty,doses ,wasteFactor,targetcoverage,vacigiven,remark);
            }

            return vrf.ID;
        }

        private void chkCalculateInPacks_CheckedChanged(object sender, EventArgs e)
        {
            //ChooseGridView();
        }

        private void gridItemChoiceView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "QuantityOrdered")
                if (Convert.ToDecimal(e.Value) <= 0) e.DisplayText = "0";
        }

        private void lkCategory_EditValueChanged(object sender, EventArgs e)
        {
            if (cboProgram.EditValue == null)
            {
                gridItemChoiceView.ActiveFilterString = String.Format("TypeID={0}", Convert.ToInt32(lkCategory.EditValue));
            }
            else if (cboProgram.EditValue != null)
            {
                gridItemChoiceView.ActiveFilterString = String.Format("ProgramID={1} and TypeID={0}", Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboProgram.EditValue));
            }
        }

        private void cboProgram_EditValueChanged(object sender, EventArgs e)
        {
            if (lkCategory.EditValue == null)
            {
                gridItemChoiceView.ActiveFilterString = String.Format("ProgramID={0}",
                                                                      Convert.ToInt32(cboProgram.EditValue));
            }
            else if (lkCategory.EditValue != null)
            {
                gridItemChoiceView.ActiveFilterString = String.Format("TypeID={0} and ProgramID ={1}", Convert.ToInt32(lkCategory.EditValue), Convert.ToInt32(cboProgram.EditValue));
            }
        }

        private void cboFromYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFromMonth.EditValue == null || cboFromYear.EditValue == null) return;
            SetEndingMonthAndYear(Convert.ToInt32(cboFromMonth.EditValue), Convert.ToInt32(cboFromYear.EditValue));
        }

        private void cboFromMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFromMonth.EditValue == null || cboFromYear.EditValue == null) return;
            SetEndingMonthAndYear(Convert.ToInt32(cboFromMonth.EditValue), Convert.ToInt32(cboFromYear.EditValue));
        }

        Dictionary<int,decimal> _customQuantity = new Dictionary<int, decimal>();

        //private void gridItemChoiceView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        //{
        //    //int rowIndex = e.ListSourceRowIndex;
        //    //decimal reqirment = Convert.ToDecimal(gridItemChoiceView.GetListSourceRowCellValue(rowIndex, "RequirmentforNextSupply"));
        //    //decimal begBal = Convert.ToDecimal(gridItemChoiceView.GetListSourceRowCellValue(rowIndex, "BeginingBalance"));
        //    //decimal received = Convert.ToDecimal(gridItemChoiceView.GetListSourceRowCellValue(rowIndex, "Received"));
        //    //decimal issued = Convert.ToDecimal(gridItemChoiceView.GetListSourceRowCellValue(rowIndex, "Issued"));
        //    //decimal lossadj = Convert.ToDecimal(gridItemChoiceView.GetListSourceRowCellValue(rowIndex, "LossAdj"));
        //    //decimal currBal = begBal + received - issued + lossadj;
        //    //decimal qtyOrdered = reqirment - currBal;
        //    //if (e.Column.FieldName != "QuantityOrdered") return;
        //    //if (e.IsGetData)
        //    //{
        //    //    if (!_customQuantity.ContainsKey(rowIndex))
        //    //        _customQuantity.Add(rowIndex, qtyOrdered);
        //    //    e.Value = _customQuantity[rowIndex];
        //    //}
        //    //if (e.IsSetData)
        //    //{
        //    //    _customQuantity[rowIndex] = Convert.ToDecimal(e.Value);
        //    //}
        //}

       


    }
}
