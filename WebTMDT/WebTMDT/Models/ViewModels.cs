using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

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

    public class ProductViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [Display(Name = "Tên đăng nhập")]
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
        public Nullable<bool> ProductVAT { get; set; }
        public string ProductStatus { get; set; }
        public string ProductType { get; set; }
        public string ProductMethod { get; set; }
        public string ProductGuarantee { get; set; }
        public string ProductPromotion { get; set; }
        public Nullable<System.DateTime> ProductDateCreate { get; set; }
        public string ProductAvatar { get; set; }
        public string ProductDescription { get; set; }
        public string ProductMore { get; set; }       
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> LocalId { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
    }

    public class ProductImages
    {        
        public Nullable<long> ProductId { get; set; }
        public string UrlImage { get; set; }
    }

}