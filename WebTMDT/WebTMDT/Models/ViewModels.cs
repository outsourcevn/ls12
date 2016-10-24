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
        public int CatId { get; set; }
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

    public class DanhMuc
    {
        public int CatId { get; set; }
        public string CatName { get; set; }
        public int? ParentId { get; set; }
        public IList<DanhMuc> DanhMucs { get; set; }
        public DanhMuc()
        {
            DanhMucs = new List<DanhMuc>();
        }
    }

    public class DanhMucEdit
    {
        public int CatId { get; set; }
        [Display(Name = "Tên menu")]
        [Required(ErrorMessage = "Phải nhập tên menu")]
        public string CatName { get; set; }
        [Display(Name = "Menu cha")]
        public int? ParentId { get; set; }
    }

    public class ProductViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [Display(Name = "Tên và mô tả ngắn về sản phẩm")]
        public string ProductName { get; set; }
        [Display(Name = "Giá bán")]
        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Giá bán phải là số.")]
        [Required(ErrorMessage = "Vui lòng nhập giá bán sản phẩm")]
        public string ProductPrice { get; set; }
        [Display(Name = "VAT")]
        public bool ProductVAT { get; set; }
        [Display(Name = "Tình trạng sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập tình trạng sản phẩm")]
        public string ProductStatus { get; set; }
        [Display(Name = "Thể loại sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập thể loại")]
        public string ProductType { get; set; }
        [Display(Name = "Cách thức giao hàng")]        
        public string ProductMethod { get; set; }
        [Display(Name = "Bảo hành")]
        //[Required(ErrorMessage = "Vui lòng nhập {0}")]
        public string ProductGuarantee { get; set; }
        [Display(Name = "Khuyến mại")]
        //[Required(ErrorMessage = "Vui lòng nhập {0}")]
        public string ProductPromotion { get; set; }        
        [Display(Name = "Ảnh đại diện sản phẩm")]
        [Required(ErrorMessage = "Vui lòng chọn ảnh đại diện sản phẩm")]
        public string ProductAvatar { get; set; }
        public string ProductDescription { get; set; }
        public string ProductMore { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn danh mục sản phẩm")]
        [Range(1, int.MaxValue, ErrorMessage = "Giá trị {0} phải là số")]
        public Nullable<int> SubCatId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn địa chỉ")]
        public Nullable<int> LocalId { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
        public Nullable<int> ParentCatId { get; set; }
        public Nullable<int> CategoryId { get; set; }
    }

    public class ProductEditViewModel
    {
        public long ProductId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [Display(Name = "Tên và mô tả ngắn về sản phẩm")]
        public string ProductName { get; set; }
        [Display(Name = "Giá bán")]
        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Giá bán phải là số.")]
        [Required(ErrorMessage="Vui lòng nhập giá bán sản phẩm")]
        public string ProductPrice { get; set; }
        [Display(Name = "VAT")]
        public bool ProductVAT { get; set; }
        [Display(Name = "Tình trạng sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập tình trạng sản phẩm")]
        public string ProductStatus { get; set; }
        [Display(Name = "Thể loại sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập thể loại sản phẩm")]
        public string ProductType { get; set; }
        [Display(Name = "Cách thức giao hàng")]        
        public string ProductMethod { get; set; }
        [Display(Name = "Bảo hành")]
        //[Required(ErrorMessage = "Vui lòng nhập {0}")]
        public string ProductGuarantee { get; set; }
        [Display(Name = "Khuyến mại")]
        //[Required(ErrorMessage = "Vui lòng nhập {0}")]
        public string ProductPromotion { get; set; }
        //[Display(Name = "Ngày đăng")]
        //[Required(ErrorMessage = "Vui lòng nhập {0}")]
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        //public Nullable<System.DateTime> ProductDateCreate { get; set; }
        [Display(Name = "Ảnh đại diện sản phẩm")]
        [Required(ErrorMessage = "Vui lòng chọn ảnh đại diện sản phẩm")]
        public string ProductAvatar { get; set; }
        public string ProductDescription { get; set; }
        public string ProductMore { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn danh mục sản phẩm")]
        [Range(1, int.MaxValue, ErrorMessage = "Giá trị {0} phải là số")]
        public Nullable<int> SubCatId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn địa chỉ")]
        [Range(1, int.MaxValue, ErrorMessage = "Giá trị {0} phải là số")]
        public Nullable<int> LocalId { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
        public Nullable<int> ParentCatId { get; set; }
        public Nullable<int> CategoryId { get; set; }
    }

    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [Display(Name = "Họ và tên")]
        public string TenNguoiBan { get; set; }
    }

    public class ProductImages
    {
        public Nullable<long> ProductId { get; set; }
        public string UrlImage { get; set; }
    }

    public class ProductStatus
    {
        public string ProductStatusName { get; set; }
    }

    public class ProductType
    {
        public string ProductTypeName { get; set; }
    }

    public class UrlImages
    {
        public string hinh1 { get; set; }
        public string hinh2 { get; set; }
        public string hinh3 { get; set; }
    }

    public class SeachQueryString
    {
        public string ParentId { get; set; }
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public string LocalId { get; set; }
        public string TheLoai { get; set; }
        public string TinhTrang { get; set; }
        public string NgayDang { get; set; }
        public string GiaBan { get; set; }
    }

    public class ProductShow
    {
        public long SanPhamId { get; set; }
        public string TenSp { get; set; }
        public string slugTenSp { get; set; }
        public string TinhTrangSp { get; set; }
        public DateTime? NgayDang { get; set; }
        public string TheLoai { get; set; }
        public long? GiaBan { get; set; }
        public string TenNguoiBan { get; set; }
        public string SDTNguoiBan { get; set; }
        public string AnhSanPham { get; set; }
        public int? LocalId { get; set; }
        public string GianHang { get; set; }
        public string SlugCat { get; set; }
        public string SlugSubCat { get; set; }
        public string SlugGianHang { get; set; }
        public int? SubCatId { get; set; }
        public int? CatId { get; set; }
        public int? ParentId { get; set; }
        public string DiaDiem { get; set; }
    }

    public class EntityProduct
    {
        public long SanPhamId { get; set; }
        public string TenSp { get; set; }
        public string TinhTrangSp { get; set; }
        public DateTime? NgayDang { get; set; }
        public string TheLoai { get; set; }
        public long? GiaBan { get; set; }
        public string TenNguoiBan { get; set; }
        public string SoDienThoaiNgBan { get; set; }
        public string AnhSanPham { get; set; }
        public string DiaDiem { get; set; }
        public string SlugCat { get; set; }
        public string slugTenSp { get; set; }
        public int? SubCatId { get; set; }
    }

    public class sortOrder
    {
        public string TypeSort { get; set; }
        public string NameSort { get; set; }
    }

}