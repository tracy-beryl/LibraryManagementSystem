using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels
{
    public class InventoryIndexViewModel
    {
        public string SearchTerm { get; set; }
        public string StatusFilter { get; set; }

        public List<InventoryListItemViewModel> Items { get; set; } = new List<InventoryListItemViewModel>();
    }
}