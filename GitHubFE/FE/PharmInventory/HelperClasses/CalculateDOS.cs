using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using PharmInventory.ViewModels;
using StockoutIndexBuilder.DAL;

namespace PharmInventory.HelperClasses
{
    public class CalculateDOS
    {
        private readonly StockoutRepository _stockoutRepository = new StockoutRepository();
        private readonly StoreRepository _storerepository = new StoreRepository();
        private readonly ReceiveDocRepository receiveDocRepository = new ReceiveDocRepository();

        List<ItemViewModel> _dataSource = new List<ItemViewModel>();

        private Boolean IsCalculatedToday()
        {
            int count = _stockoutRepository.GetAll().Where(dos => dos.LastIndexedTime.Value.Date == DateTime.Today.Date).Select(dos => dos.ItemID).Count();
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void BuildDOS(bool forceCalc = false)
        {
            if ((IsCalculatedToday()) && (forceCalc == false))
            {
                return;
            }

            _stockoutRepository.DeleteAll();

            var allstores = _storerepository.AllStores();
            foreach (var store in allstores)
            {        
                var allItems = receiveDocRepository.RecievedItems().Where(x=>x.StoreID == store.ID).Select(m => m.Item).Distinct().ToList();
                var itemsList = ItemViewModelCollection.Create(allItems);

                foreach (var item in itemsList)
                {
                    StockoutIndexBuilder.Builder.BuildIndex(item.ItemId, store.ID);
                    item.Indexed = true;
                }
            }
        }
    }
}
