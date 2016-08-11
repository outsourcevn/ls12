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
        [Display(Name = "Tên và mô tả ngắn về sản phẩm")]
        public string ProductName { get; set; }
        [Display(Name="Giá bán")]
        [Required(ErrorMessage="Vui lòng nhập {0} sản phẩm")]
        public string ProductPrice { get; set; }
        [Display(Name="VAT")]
        public bool ProductVAT { get; set; }
        [Display(Name="Tình trạng sản phẩm")]
        [Required(ErrorMessage="Vui lòng nhập {0}")]
        public string ProductStatus { get; set; }
        [Display(Name = "Thể loại sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        public string ProductType { get; set; }
        [Display(Name="Cách thức giao hàng")]
        [Required(ErrorMessage="Vui lòng nhập {0}")]
        public string ProductMethod { get; set; }
        [Display(Name = "Bảo hành")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        public string ProductGuarantee { get; set; }
        [Display(Name = "Khuyến mại")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        public string ProductPromotion { get; set; }
        [Display(Name = "Ngày đăng")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{dd/MM/yy hh:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ProductDateCreate { get; set; }
        [Display(Name = "Ảnh đại diện sản phẩm")]
        public HttpPostedFileBase ProductAvatar { get; set; }
        public string ProductDescription { get; set; }
        public string ProductMore { get; set; }   
        [Required(ErrorMessage="Vui lòng chọn danh mục sản phẩm")]
        [Range(1, int.MaxValue, ErrorMessage = "Giá trị {0} phải là số")]
        public Nullable<int> CategoryId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn địa chỉ")]
        [Range(1, int.MaxValue, ErrorMessage = "Giá trị {0} phải là số")]
        public Nullable<int> LocalId { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
    }

    public class ProductImages
    {        
        public Nullable<long> ProductId { get; set; }
        public string UrlImage { get; set; }
    }

}