using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BLL;

namespace PharmInventory.HelperClasses
{
    public static class Common
    {
        public static bool IsInventoryPeriod()
        {
            int currentMonth = EthiopianDate.EthiopianDate.Now.Month;
            if (currentMonth == 11)
            {
                YearEnd yearEnd = new YearEnd();
                if (!yearEnd.IsInventoryCompleted(EthiopianDate.EthiopianDate.Now.Year))
                {
                    MessageBox.Show("You are on inventory period and inventory is not completed for all Items Received and Issued this year!, so you cann't perform any transaction until inventory is done for all Items.", "HCMIS FE", MessageBoxButton.OK);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
