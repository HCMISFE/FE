using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace PharmInventory.Reports
{
    public partial class RRFReport : DevExpress.XtraReports.UI.XtraReport
    {
        public RRFReport()
        {
            InitializeComponent();
        }

        private void categoryName_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            BLL.Type catType = new BLL.Type();
            catType.LoadByPrimaryKey(Convert.ToInt16(xrTypeID.Text));
            categoryName.Text = catType.Name;
        }
    }
}
