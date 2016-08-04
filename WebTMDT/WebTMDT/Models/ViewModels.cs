using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTMDT.Models
{
    // The ViewModel is now a hirearchical model, where each item has a list of children.
    public class CatViewModel
    {
        public int CatId {get; set;}
        public string CatName { get; set; }
        public IEnumerable<CatViewModel> ChildrenCat { get; set; }
    }

    public class DanhMucCon
    {
        public int Id { get; set; }
        public string Name { get; set; } 
    }

    public class LocalViewModel
    {
        public int LocalId { get; set; }
        public string LocalName { get; set; }
        public IEnumerable<LocalViewModel> ChildrenLocal { get; set; }
    }
}