using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTMDT.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using WebTMDT.Helpers;
using PagedList;
using System.Net;
using System.Data.Entity;
using System.Globalization;

namespace WebTMDT.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        /// <summary>
        /// Application DB context
        /// </summary>
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> UserManager { get; set; }

        private langson12Entities db = new langson12Entities();

        // GET: Product
        public ActionResult Index()
        {
            var Cat = db.Categories.ToList();
            ViewBag.Category = Cat;
            var LocalData = db.Locals.ToList();
            ViewBag.LocalData = LocalData;

            ViewBag.ProductStatus = new List<ProductStatus>() { 
                new ProductStatus() { ProductStatusName = "Mới 100%" },
                new ProductStatus() { ProductStatusName = "Mới 90%" },
                new ProductStatus() { ProductStatusName = "Mới 80%" },
                new ProductStatus() { ProductStatusName = "Hàng like new" },
                new ProductStatus() { ProductStatusName = "Hàng cũ" },
                new ProductStatus() { ProductStatusName = "hàng thanh lý" }
            };

            ViewBag.ProductType = new List<ProductType>() {
                new ProductType() { ProductTypeName = "Hàng chính hãng" },
                new ProductType() { ProductTypeName = "Hàng xách tay" },
                new ProductType() { ProductTypeName = "Hàng nội địa" },
                new ProductType() { ProductTypeName = "Hàng trung quốc" },
                new ProductType() { ProductTypeName = "Hàng Sê cần hen" },
            };

            


            return View();
        }



        // POST: /Product/AddNewProduct
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewProduct(ProductViewModel model, UrlImages image)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            //this.ApplicationDbContext = new ApplicationDbContext();
            //this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
            string userId = User.Identity.GetUserId();
            //var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!string.IsNullOrEmpty(userId))
            {               
                Product _product = new Product();
                _product.F2 = model.ProductName ?? null;
                string strGiaBanSp = model.ProductPrice != null ? model.ProductPrice.Replace(",", "").ToString() : null;
                _product.F3 = !string.IsNullOrEmpty(strGiaBanSp) ? Convert.ToInt32(strGiaBanSp) : (int?)null;
                _product.F4 = model.ProductVAT;
                _product.F5 = model.ProductStatus ?? null;
                _product.F6 = model.ProductType ?? null;
                _product.F7 = model.ProductMethod ?? null;
                _product.F8 = model.ProductGuarantee ?? null;
                _product.F9 = model.ProductPromotion ?? null;
                _product.F10 = DateTime.UtcNow;
                _product.F11 = model.ProductAvatar ?? null;
                _product.F12 = model.ProductDescription ?? null;
                _product.F13 = null;
                _product.F14 = userId;
                _product.F15 = model.SubCatId ?? null;
                _product.F16 = model.LocalId ?? null;
                var _subcat = db.Categories.Where(x => x.F1 == model.SubCatId).FirstOrDefault();
                _product.F17 = _subcat.Category2.F1;
                _product.F18 = _subcat.Category2.Category2.F1;
                db.Products.Add(_product);
                await db.SaveChangesAsync();
                long _productId = _product.F1;

                var listImage = new List<string> { image.hinh1, image.hinh2, image.hinh3 };
                if (listImage != null)
                {
                    var xx = from a in listImage where !string.IsNullOrWhiteSpace(a) select a;
                    foreach (var item in xx)
                    {
                        ImageProduct _imageProduct = new ImageProduct();
                        _imageProduct.F2 = _productId;
                        _imageProduct.F3 = item.ToString();
                        db.ImageProducts.Add(_imageProduct);
                        await db.SaveChangesAsync();
                    }
                }
                TempData["SubCatId"] = model.SubCatId;
                TempData["LocalId"] = model.LocalId;
                
                TempData["SubCatName"] = _subcat.F2;
                TempData["CatName"] = _subcat.Category2.F2;
                TempData["LocalName"] = db.Locals.Where(x => x.F1 == model.LocalId).FirstOrDefault().F2;
                TempData["Message"] = "Đăng sản phẩm thành công.";
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult Upload()
        {
            string relativeUrl = string.Empty;
            var file = Request.Files[0];
            if (!Configs.IsImage(file))
            {
                return Json(relativeUrl, JsonRequestBehavior.AllowGet); ;
            }
            var fileName = Path.GetFileName(file.FileName);
            string path = Server.MapPath("~/Content/Images/Products/");
            FileInfo fileInfo = new FileInfo(Server.MapPath("~/Content/Images/Products/" + fileName));
            if (fileInfo.Exists)
            {
                fileName = string.Format("{0:dd_MM_yyyy_hh_mm_ss_tt}_" + fileName, DateTime.Now);
                string strpath = Path.Combine(path, fileName);
                file.SaveAs(strpath);
                relativeUrl = "/Content/Images/Products/" + fileName;
            }
            else
            {
                string strpath = Path.Combine(path, fileName);
                file.SaveAs(strpath);
                relativeUrl = "/Content/Images/Products/" + fileName;
            }
            return Json(relativeUrl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase ProductImages)
        {
            List<string> Images = new List<string>();
            if (ProductImages != null)
            {
                if (!Configs.IsImage(ProductImages))
                {
                    return Json(Images, JsonRequestBehavior.AllowGet);
                }
            }


            if (ProductImages != null)
            {
                if (ProductImages.ContentLength > 0)
                {
                    string relativeUrl = string.Empty;
                    var fileName = Path.GetFileName(ProductImages.FileName);
                    string path = Server.MapPath("~/Content/Images/Products/");
                    FileInfo fileInfo = new FileInfo(Server.MapPath("~/Content/Images/Products/" + fileName));
                    if (fileInfo.Exists)
                    {
                        fileName = string.Format("{0:dd_MM_yyyy_hh_mm_ss_tt}_" + fileName, DateTime.Now);
                        string strpath = Path.Combine(path, fileName);
                        ProductImages.SaveAs(strpath);
                        relativeUrl = "/Content/Images/Products/" + fileName;
                    }
                    else
                    {
                        var strpath = Path.Combine(path, fileName);
                        ProductImages.SaveAs(strpath);
                        relativeUrl = "/Content/Images/Products/" + fileName;
                    }
                    Images.Add(relativeUrl);
                }
            }
            return Json(Images, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UploadMultiFile(IEnumerable<HttpPostedFileBase> ProductImages)
        {
            List<string> Images = new List<string>();
            if (ProductImages != null)
            {
                foreach (var item in ProductImages)
                {
                    if (!Configs.IsImage(item))
                    {
                        return Json(Images, JsonRequestBehavior.AllowGet);
                    }
                }
            }


            if (ProductImages != null)
            {
                foreach (var file in ProductImages)
                {
                    if (file.ContentLength > 0)
                    {
                        string relativeUrl = string.Empty;
                        var fileName = Path.GetFileName(file.FileName);
                        string path = Server.MapPath("~/Content/Images/Products/");
                        FileInfo fileInfo = new FileInfo(Server.MapPath("~/Content/Images/Products/" + fileName));
                        if (fileInfo.Exists)
                        {
                            fileName = string.Format("{0:dd_MM_yyyy_hh_mm_ss_tt}_" + fileName, DateTime.Now);
                            string strpath = Path.Combine(path, fileName);
                            file.SaveAs(strpath);
                            relativeUrl = "/Content/Images/Products/" + fileName;
                        }
                        else
                        {
                            var strpath = Path.Combine(path, fileName);
                            file.SaveAs(strpath);
                            relativeUrl = "/Content/Images/Products/" + fileName;
                        }
                        Images.Add(relativeUrl);
                    }
                }
            }
            return Json(Images, JsonRequestBehavior.AllowGet);
        }


        //private List<int?> catsChildList = new List<int?>();

        // GET: /Product/search
        [AllowAnonymous]
        public ActionResult Search(SeachQueryString search, int? page, string inputsearch)
        {            
            var Cat = db.Categories.ToList();
            ViewBag.Category = Cat;
            var LocalData = db.Locals.ToList();
            ViewBag.LocalData = LocalData;
            ViewBag.ProductTheLoai = new List<ProductType>() {
                new ProductType() { ProductTypeName = "Hàng chính hãng" },
                new ProductType() { ProductTypeName = "Hàng xách tay" },
                new ProductType() { ProductTypeName = "Hàng nội địa" },
                new ProductType() { ProductTypeName = "Hàng trung quốc" },
                new ProductType() { ProductTypeName = "Hàng Sê cần hen" },
            };
            ViewBag.ProductTrangThai = new List<ProductStatus>() { 
                new ProductStatus() { ProductStatusName = "Mới 100%" },
                new ProductStatus() { ProductStatusName = "Mới 90%" },
                new ProductStatus() { ProductStatusName = "Mới 80%" },
                new ProductStatus() { ProductStatusName = "Hàng like new" },
                new ProductStatus() { ProductStatusName = "Hàng cũ" },
                new ProductStatus() { ProductStatusName = "hàng thanh lý" }
            };

          
            ViewBag.ProductNgayDang = new List<sortOrder>() {
                new sortOrder() { TypeSort = "desc", NameSort = "Giảm dần" },
                new sortOrder() { TypeSort = "asc", NameSort = "Tăng dần" }
            };
            List<sortOrder> ListGiaBan = new List<sortOrder>() {
                new sortOrder() { TypeSort = "desc", NameSort = "Giảm dần" },
                new sortOrder() { TypeSort = "asc", NameSort = "Tăng dần" },
                new sortOrder() { TypeSort = "0-1", NameSort = "Giá dưới 1 triệu" }
            };
            List<sortOrder> myList = Configs.KhoangGia();
            var _ConcatListGiaBan = ListGiaBan.Concat(myList).AsEnumerable();
            ViewBag.ProductGiaBan = _ConcatListGiaBan;
           
            List<ProductShow> lstProduct = new List<ProductShow>();

            if (!string.IsNullOrWhiteSpace(inputsearch))
            {
                var _products = db.Products.Where(p => p.F2.ToLower().Contains(inputsearch)).Select(p=>p);
                foreach (var product in _products)
                {
                    var _p = new ProductShow()
                    {
                        SanPhamId = product.F1,
                        TenSp = product.F2,
                        TinhTrangSp = product.F5,
                        TheLoai = product.F6,
                        GiaBan = product.F3,
                        NgayDang = product.F10,
                        TenNguoiBan = product.AspNetUser.TenNguoiBan,
                        SoDienThoaiNgBan = product.AspNetUser.PhoneNumber,
                        AnhSanPham = product.F11,
                        LocalId = product.F16,
                        DanhMucCha = Configs.unicodeToNoMark(product.Category.Category2.F2),
                        DanhMucCon = Configs.unicodeToNoMark(product.Category.F2),
                        slugTenSp = Configs.unicodeToNoMark(product.F2 != null ? product.F2 : "no-title"),
                        gianhang = product.AspNetUser.UserName,
                        SlugGianHang = Configs.unicodeToNoMark(product.AspNetUser.TenNguoiBan != null ? product.AspNetUser.TenNguoiBan : "no-title")
                    };
                    lstProduct.Add(_p);
                }
            }

            // !p && !c && sc
            if (!string.IsNullOrWhiteSpace(search.ParentId) && !string.IsNullOrWhiteSpace(search.CategoryId) && string.IsNullOrWhiteSpace(search.SubCategoryId))
            {
                int _idParent = int.Parse(search.ParentId);
                int _idCat = int.Parse(search.CategoryId);
                var _dataCatParent = db.Categories.Where(x => x.F1 == _idCat && x.F3 == _idParent).FirstOrDefault();
                if (_dataCatParent != null)
                {
                    ViewBag.ParentCatIdFirst = _dataCatParent.Category2.F1;
                    ViewBag.ParentCatNameFirst = _dataCatParent.Category2.F2;
                    ViewBag.CatIdFirst = _dataCatParent.F1;
                    ViewBag.CatNameFirst = _dataCatParent.F2;
                    var cats = (from c in db.Categories where c.F3 == _idCat select new { c.F1 }).ToList();
                    List<int?> catsChildList = cats.Select(x => (int?)x.F1).ToList();
                    var products = (from p in db.Products
                                    group p by p.F1 into g
                                    where g.Any(x => catsChildList.Contains(x.F15))
                                    select g);

                    foreach (var item in products)
                    {
                        foreach (var product in item)
                        {
                            var p = new ProductShow()
                            {
                                SanPhamId = product.F1,
                                TenSp = product.F2,
                                TinhTrangSp = product.F5,
                                TheLoai = product.F6,
                                GiaBan = product.F3,
                                NgayDang = product.F10,
                                TenNguoiBan = product.AspNetUser.TenNguoiBan,
                                SoDienThoaiNgBan = product.AspNetUser.PhoneNumber,
                                AnhSanPham = product.F11,
                                LocalId = product.F16,
                                DanhMucCha = Configs.unicodeToNoMark(product.Category.Category2.F2),
                                DanhMucCon = Configs.unicodeToNoMark(product.Category.F2),
                                slugTenSp = Configs.unicodeToNoMark(product.F2 != null ? product.F2 : ""),
                                gianhang = product.AspNetUser.UserName,
                                SlugGianHang = Configs.unicodeToNoMark(product.AspNetUser.TenNguoiBan != null ? product.AspNetUser.TenNguoiBan : "no-title")

                            };
                            lstProduct.Add(p);
                        }
                    }
                }
            }
            // !p && c && sc
            if (!string.IsNullOrWhiteSpace(search.ParentId) && string.IsNullOrWhiteSpace(search.CategoryId) && string.IsNullOrWhiteSpace(search.SubCategoryId))
            {
                int _idParent;
                if (int.TryParse(search.ParentId, out _idParent))
                {
                    var _dataCatParent = db.Categories.Where(x => x.F1 == _idParent).FirstOrDefault();
                    ViewBag.ParentCatIdFirst = _dataCatParent.F1;
                    ViewBag.ParentCatNameFirst = _dataCatParent.F2;
                    //IQueryable<Product> products;
                    var catsInParentCat = (from c in db.Categories where c.F3 == _idParent select new { c.F1 }).ToList();
                    foreach (var cat in catsInParentCat)
                    {
                        var cats = (from cc in db.Categories where cc.F3 == cat.F1 select new { cc.F1 }).ToList();
                        List<int?> catsChildList = cats.Select(x => (int?)x.F1).ToList();
                        var _products = (from p in db.Products
                                         group p by p.F1 into g
                                         where g.Any(x => catsChildList.Contains(x.F15))
                                         select g);
                        foreach (var item in _products)
                        {
                            foreach (var product in item)
                            {
                                var p = new ProductShow()
                                {
                                    SanPhamId = product.F1,
                                    TenSp = product.F2,
                                    TinhTrangSp = product.F5,
                                    TheLoai = product.F6,
                                    GiaBan = product.F3,
                                    NgayDang = product.F10,
                                    TenNguoiBan = product.AspNetUser.TenNguoiBan,
                                    SoDienThoaiNgBan = product.AspNetUser.PhoneNumber,
                                    AnhSanPham = product.F11,
                                    LocalId = product.F16,
                                    DanhMucCha = Configs.unicodeToNoMark(product.Category.Category2.F2),
                                    DanhMucCon = Configs.unicodeToNoMark(product.Category.F2),
                                    slugTenSp = Configs.unicodeToNoMark(product.F2 != null ? product.F2 : "san-pham-khong-ten"),
                                    gianhang = product.AspNetUser.UserName,
                                    SlugGianHang = Configs.unicodeToNoMark(product.AspNetUser.TenNguoiBan != null ? product.AspNetUser.TenNguoiBan : "gian-hang-khong-ten")
                                
                                };
                                lstProduct.Add(p);
                            }
                        }

                    }                 

                }
            }

            // !p && !c && !sc
            if (!string.IsNullOrWhiteSpace(search.CategoryId) && !string.IsNullOrWhiteSpace(search.SubCategoryId) && !string.IsNullOrWhiteSpace(search.ParentId))
            {
                int _idCat = int.Parse(search.CategoryId);
                int _idSubCat = int.Parse(search.SubCategoryId);
                int _idPaCat = int.Parse(search.ParentId);
                var _parent = db.Categories.Where(x => x.F1 == _idPaCat).FirstOrDefault();
                ViewBag.ParentCatIdFirst = _parent.F1;
                ViewBag.ParentCatNameFirst = _parent.F2;
                var _cat = db.Categories.Where(x => x.F1 == _idCat && x.F3 == _idPaCat).FirstOrDefault();
                if (_cat != null)
                {
                    ViewBag.CatIdFirst = _cat.F1;
                    ViewBag.CatNameFirst = _cat.F2;
                    var _subcat = db.Categories.Where(x => x.F1 == _idSubCat && x.F3 == _idCat).FirstOrDefault();
                    if (_subcat != null)
                    {
                        ViewBag.SubCatIdFirst = _subcat.F1;
                        ViewBag.SubCatNameFirst = _subcat.F2;
                        // lay san pham voi subcatid
                        var _products = (from p in db.Products where p.F15 == _idSubCat select p);
                        foreach (var product in _products)
                        {
                            var p = new ProductShow()
                            {
                                SanPhamId = product.F1,
                                TenSp = product.F2,
                                TinhTrangSp = product.F5,
                                TheLoai = product.F6,
                                GiaBan = product.F3,
                                NgayDang = product.F10,
                                TenNguoiBan = product.AspNetUser.TenNguoiBan,
                                SoDienThoaiNgBan = product.AspNetUser.PhoneNumber,
                                AnhSanPham = product.F11,
                                LocalId = product.F16,
                                DanhMucCha = Configs.unicodeToNoMark(product.Category.Category2.F2),
                                DanhMucCon = Configs.unicodeToNoMark(product.Category.F2),
                                slugTenSp = Configs.unicodeToNoMark(product.F2 != null ? product.F2 : ""),
                                gianhang = product.AspNetUser.UserName,
                                SlugGianHang = Configs.unicodeToNoMark(product.AspNetUser.TenNguoiBan != null ? product.AspNetUser.TenNguoiBan : "no-title")
                                
                            };
                            lstProduct.Add(p);
                        }
                    }
                    else
                    {
                        ViewBag.SubCatIdFirst = null;
                        ViewBag.SubCatNameFirst = null;
                    }

                }
                else
                {
                    ViewBag.CatIdFirst = null;
                    ViewBag.CatNameFirst = null;
                }                
                

            }

            // p && c && !sc
            if (string.IsNullOrWhiteSpace(search.CategoryId) && string.IsNullOrWhiteSpace(search.ParentId) && !string.IsNullOrWhiteSpace(search.SubCategoryId))
            {
                int _id;
                if (int.TryParse(search.SubCategoryId, out _id))
                {
                    var _cat = db.Categories.Where(x => x.F1 == _id).FirstOrDefault();
                    ViewBag.SubCatIdFirst = _cat.F1;
                    ViewBag.SubCatNameFirst = _cat.F2;
                    // lay san pham voi subcatid
                    var _products = (from p in db.Products where p.F15 == _id select p);
                    foreach (var product in _products)
                    {
                        var p = new ProductShow()
                        {
                            SanPhamId = product.F1,
                            TenSp = product.F2,
                            TinhTrangSp = product.F5,
                            TheLoai = product.F6,
                            GiaBan = product.F3,
                            NgayDang = product.F10,
                            TenNguoiBan = product.AspNetUser.TenNguoiBan,
                            SoDienThoaiNgBan = product.AspNetUser.PhoneNumber,
                            AnhSanPham = product.F11,
                            LocalId = product.F16,
                            DanhMucCha = Configs.unicodeToNoMark(product.Category.Category2.F2),
                            DanhMucCon = Configs.unicodeToNoMark(product.Category.F2),
                            slugTenSp = Configs.unicodeToNoMark(product.F2 != null ? product.F2 : ""),
                            gianhang = product.AspNetUser.UserName,
                            SlugGianHang = Configs.unicodeToNoMark(product.AspNetUser.TenNguoiBan != null ? product.AspNetUser.TenNguoiBan : "no-title")
                                
                        };
                        lstProduct.Add(p);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(search.CategoryId) && string.IsNullOrWhiteSpace(search.SubCategoryId) && string.IsNullOrWhiteSpace(search.ParentId))
            {
                int _id;
                if (int.TryParse(search.CategoryId, out _id))
                {
                    var _dataCat = db.Categories.Where(x => x.F1 == _id).FirstOrDefault();
                    ViewBag.CatIdFirst = _dataCat.F1;
                    ViewBag.CatNameFirst = _dataCat.F2;
                    // lay da cac danh muc con cua danh muc cha co _id
                    var cats = (from c in db.Categories where c.F3 == _id select new { c.F1 }).ToList();
                    List<int?> catsChildList = cats.Select(x => (int?)x.F1).ToList();
                    var products = (from p in db.Products
                                    group p by p.F1 into g
                                    where g.Any(x => catsChildList.Contains(x.F15))
                                    select g);

                    foreach (var item in products)
                    {
                        foreach (var product in item)
                        {
                            var p = new ProductShow()
                               {
                                   SanPhamId = product.F1,
                                   TenSp = product.F2,
                                   TinhTrangSp = product.F5,
                                   TheLoai = product.F6,
                                   GiaBan = product.F3,
                                   NgayDang = product.F10,
                                   TenNguoiBan = product.AspNetUser.TenNguoiBan,
                                   SoDienThoaiNgBan = product.AspNetUser.PhoneNumber,
                                   AnhSanPham = product.F11,
                                   LocalId = product.F16,
                                   DanhMucCha = Configs.unicodeToNoMark(product.Category.Category2.F2),
                                   DanhMucCon = Configs.unicodeToNoMark(product.Category.F2),
                                   slugTenSp = Configs.unicodeToNoMark(product.F2 != null ? product.F2 : ""),
                                   gianhang = product.AspNetUser.UserName,
                                   SlugGianHang = Configs.unicodeToNoMark(product.AspNetUser.TenNguoiBan != null ? product.AspNetUser.TenNguoiBan : "no-title")
                                
                               };
                            lstProduct.Add(p);
                        }
                    }
                }

            }

            if (lstProduct.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(search.LocalId))
                {
                    int _idLocal;
                    if (int.TryParse(search.LocalId, out _idLocal))
                    {
                        var _local = db.Locals.Where(x => x.F1 == _idLocal).FirstOrDefault();
                        ViewBag.LocalId = _local.F1;
                        ViewBag.LocalName = _local.F2;
                        lstProduct = lstProduct.Where(x => x.LocalId == _idLocal).ToList();
                    };
                }
            }

            if (string.IsNullOrWhiteSpace(search.ParentId) && string.IsNullOrWhiteSpace(search.CategoryId) && string.IsNullOrWhiteSpace(search.SubCategoryId) && !string.IsNullOrWhiteSpace(search.LocalId))
            {
                int _idLocal;
                if (int.TryParse(search.LocalId, out _idLocal))
                {
                    var _local = db.Locals.Where(x => x.F1 == _idLocal).FirstOrDefault();
                    ViewBag.LocalId = _local.F1;
                    ViewBag.LocalName = _local.F2;
                    var _products = (from p in db.Products where p.F16 == _idLocal select p);
                    foreach (var product in _products)
                    {
                        var p = new ProductShow()
                        {
                            SanPhamId = product.F1,
                            TenSp = product.F2,
                            TinhTrangSp = product.F5,
                            TheLoai = product.F6,
                            GiaBan = product.F3,
                            NgayDang = product.F10,
                            TenNguoiBan = product.AspNetUser.TenNguoiBan,
                            SoDienThoaiNgBan = product.AspNetUser.PhoneNumber,
                            AnhSanPham = product.F11,
                            LocalId = product.F16,
                            DanhMucCha = Configs.unicodeToNoMark(product.Category.Category2.F2),
                            DanhMucCon = Configs.unicodeToNoMark(product.Category.F2),
                            slugTenSp = Configs.unicodeToNoMark(product.F2 != null ? product.F2 : ""),
                            gianhang = product.AspNetUser.UserName,
                            SlugGianHang = Configs.unicodeToNoMark(product.AspNetUser.TenNguoiBan != null ? product.AspNetUser.TenNguoiBan : "no-title")

                        };
                        lstProduct.Add(p);
                    }
                }
            }

            // Thêm điều kiện lọc khi lstProduct được tìm thấy
            if (!string.IsNullOrWhiteSpace(search.TheLoai) && !string.IsNullOrWhiteSpace(search.TinhTrang))
            {
                ViewBag.TheLoai = search.TheLoai;
                ViewBag.TinhTrang = search.TinhTrang;
                lstProduct = lstProduct.Where(x => x.TheLoai == search.TheLoai && x.TinhTrangSp == search.TinhTrang).ToList();
            }

            if (!string.IsNullOrWhiteSpace(search.TheLoai) && string.IsNullOrWhiteSpace(search.TinhTrang))
            {
                ViewBag.TinhTrang = null;
                ViewBag.TheLoai = search.TheLoai;
                string _strTheLoai = search.TheLoai;
                lstProduct = lstProduct.Where(x => x.TheLoai == _strTheLoai).ToList();
            }

            if (string.IsNullOrWhiteSpace(search.TheLoai) && !string.IsNullOrWhiteSpace(search.TinhTrang))
            {
                ViewBag.TheLoai = null;
                ViewBag.TinhTrang = search.TinhTrang;
                string _strTinhTrang = search.TinhTrang;
                lstProduct = lstProduct.Where(x => x.TinhTrangSp == _strTinhTrang).ToList();
            }

            lstProduct = lstProduct.OrderByDescending(x => x.NgayDang).ToList();

            //search.NgayDang = "desc";
            if (!string.IsNullOrWhiteSpace(search.NgayDang) && string.IsNullOrWhiteSpace(search.GiaBan))
            {
                ViewBag.GiaBan = null;
                ViewBag.NgayDang = search.NgayDang;
                switch (search.NgayDang)
                {
                    case "desc":
                        lstProduct = lstProduct.OrderByDescending(x => x.NgayDang).ToList();
                        break;
                    case "asc":
                        lstProduct = lstProduct.OrderBy(x => x.NgayDang).ToList();
                        break;
                    
                }
            }

            if (string.IsNullOrWhiteSpace(search.NgayDang) && !string.IsNullOrWhiteSpace(search.GiaBan))
            {
                ViewBag.NgayDang = null;
                ViewBag.GiaBan = search.GiaBan;
                Configs.TwoNumber _x;
                switch (search.GiaBan)
                {
                    case "desc":
                        lstProduct = lstProduct.OrderByDescending(x => x.GiaBan).ToList();
                        break;
                    case "asc":
                        lstProduct = lstProduct.OrderBy(x => x.GiaBan).ToList();
                        break;
                    case "1-3":
                        _x = Configs.GetNumber("1-3");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    case "3-5":
                        _x = Configs.GetNumber("3-5");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    case "5-7":
                        _x = Configs.GetNumber("5-7");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    case "7-9":
                        _x = Configs.GetNumber("7-9");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    case "9-11":
                        _x = Configs.GetNumber("9-11");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    case "11-13":
                        _x = Configs.GetNumber("11-13");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    case "13-15":
                        _x = Configs.GetNumber("13-15");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    case "15-17":
                        _x = Configs.GetNumber("15-17");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    case "17-19":
                        _x = Configs.GetNumber("17-19");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    case "19-20":
                        _x = Configs.GetNumber("19-20");
                        lstProduct = lstProduct.Where(x => x.GiaBan >= _x.Number1 && x.GiaBan <= _x.Number2).ToList();
                        break;
                    default: 
                        _x = Configs.GetNumber("0-1");
                        lstProduct = lstProduct.Where(x=>x.GiaBan <= _x.Number2).ToList();
                        break;
                }
            }

            if (lstProduct.Count > 0)
            {
                // Phan trang o day ne
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                var onePageOfProducts = lstProduct.ToPagedList(pageNumber, pageSize);
                ViewBag.Product = onePageOfProducts;
            }            

            
            return View();
        }

        // Product/ProductDetail/1
        [AllowAnonymous]
        public async Task<ActionResult> Detail(long? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction("NotFound", "Home");
            }

            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                ViewBag.ProductNotFound = "Người dùng chưa có sản phẩm nào";
            }

            product.F17 = product.F17 != null ? product.F17 += 1 : 1;
            await db.SaveChangesAsync();

            return View(product);
        }

        // user/product/list
        public async Task<ActionResult> ProductLists(int? page, string searchQuery)
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
            string userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            List<EntityProduct> lstProduct = new List<EntityProduct>();
            if (user != null)
            {
                userId = user.Id;
                var products = (from p in db.Products where p.F14 == userId select p);
                foreach (var item in products)
                {
                    var pr = new EntityProduct()
                           {
                               SanPhamId = item.F1,
                               TenSp = item.F2,
                               TinhTrangSp = item.F5,
                               TheLoai = item.F6,
                               GiaBan = item.F3,
                               NgayDang = item.F10,
                               TenNguoiBan = item.AspNetUser.TenNguoiBan,
                               SoDienThoaiNgBan = item.AspNetUser.PhoneNumber,
                               AnhSanPham = item.F11,
                           };
                    lstProduct.Add(pr);
                }



                if (lstProduct.Count > 0)
                {
                    if (!String.IsNullOrEmpty(searchQuery))
                    {
                        ViewBag.searchQuery = searchQuery.ToLower();
                        lstProduct = lstProduct.Where(s => s.TenSp.ToLower().Contains(searchQuery.ToLower())).ToList();
                    }

                    lstProduct.OrderByDescending(x => x.NgayDang).ToList();

                    // Phan trang o day ne
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    var onePageOfProducts = lstProduct.ToPagedList(pageNumber, pageSize);
                    ViewBag.ProductLists = onePageOfProducts;
                }    
                                
            }

            return View();
        }

        // GET: user/product/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction("NotFound", "Home");
            }
            var Cat = db.Categories.ToList();
            ViewBag.Category = Cat;
            var LocalData = db.Locals.ToList();
            ViewBag.LocalData = LocalData;

            ViewBag.ProductStatus = new List<ProductStatus>() { 
                new ProductStatus() { ProductStatusName = "Mới 100%" },
                new ProductStatus() { ProductStatusName = "Mới 90%" },
                new ProductStatus() { ProductStatusName = "Mới 80%" },
                new ProductStatus() { ProductStatusName = "Hàng like new" },
                new ProductStatus() { ProductStatusName = "Hàng cũ" },
                new ProductStatus() { ProductStatusName = "hàng thanh lý" }
            };

            ViewBag.ProductType = new List<ProductType>() {
                new ProductType() { ProductTypeName = "Hàng chính hãng" },
                new ProductType() { ProductTypeName = "Hàng xách tay" },
                new ProductType() { ProductTypeName = "Hàng nội địa" },
                new ProductType() { ProductTypeName = "Hàng trung quốc" },
                new ProductType() { ProductTypeName = "Hàng Sê cần hen" },
            };
           // this.ApplicationDbContext = new ApplicationDbContext();
           // this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
            string userId = User.Identity.GetUserId();
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                ViewBag.ProductNotFound = "Sản phẩm không tồn tại hoặc đã bị xóa.";
                return View();
            }
           
            if (product.F14 != userId)
            {
                ViewBag.ProductNotFound = "Bạn không có quyền sửa sản phẩm của người khác nhé.";
                return View(product);
            }
            var _products = new ProductEditViewModel()
            {
                ProductId = product.F1,
                ProductName = product.F2,                
                ProductPrice = product.F3.ToString(),
                ProductVAT = (bool)product.F4,
                ProductStatus = product.F5,
                ProductType = product.F6,
                ProductMethod = product.F7,
                ProductGuarantee = product.F8,
                ProductPromotion = product.F9,
                ProductAvatar = product.F11,
                ProductDescription = product.F12,
                CategoryId = product.Category.Category2.F1,
                SubCatId = product.Category.F1,
                LocalId = product.F16,
                ProductImages = product.ImageProducts.Select(x => new ProductImages() { ProductId = x.F1, UrlImage = x.F3 }).ToList()
            };

            ViewBag.CatName = db.Categories.Where(x => x.F1 == _products.CategoryId).FirstOrDefault().F2;
            ViewBag.SubCatName = db.Categories.Where(x => x.F1 == _products.SubCatId).FirstOrDefault().F2;
            ViewBag.LocalName = db.Locals.Where(x => x.F1 == _products.LocalId).FirstOrDefault().F2;
            //ViewBag.ItemsPt = new SelectList(ViewBag.ProductType, "ProductTypeName", "ProductTypeName", _products.ProductType);
            //ViewBag.ItemsPs = new SelectList(ViewBag.ProductStatus, "ProductStatusName", "ProductStatusName", _products.ProductStatus);
            return View(_products);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductEditViewModel product, string hinh_0, string hinh_1, string hinh_2)
        {    
            if (ModelState.IsValid)
            {
                var _product = db.Products.Find(product.ProductId);
                if (_product != null)
                {
                    _product.F2 = product.ProductName ?? null;
                    //!string.IsNullOrWhiteSpace(product.ProductPrice) ? product.ProductPrice.Replace(",","") : null;
                    string strGiaBanSp = product.ProductPrice != null ? product.ProductPrice.Replace(",", "").ToString() : null;
                    _product.F3 = !string.IsNullOrEmpty(strGiaBanSp) ? Convert.ToInt32(strGiaBanSp) : (int?)null;
                    _product.F4 = product.ProductVAT;
                    _product.F5 = product.ProductStatus ?? null;
                    _product.F6 = product.ProductType ?? null;
                    _product.F7 = product.ProductMethod ?? null;
                    _product.F8 = product.ProductGuarantee ?? null;
                    _product.F9 = product.ProductPromotion ?? null;
                    _product.F10 = DateTime.UtcNow;
                    _product.F11 = product.ProductAvatar ?? null;
                    _product.F12 = product.ProductDescription ?? null;
                    _product.F13 = null;
                    _product.F15 = product.SubCatId ?? null;
                    var _subcat = db.Categories.Where(x => x.F1 == product.SubCatId).FirstOrDefault();
                    _product.F17 = _subcat.Category2.F1;
                    _product.F18 = _subcat.Category2.Category2.F1;
                    _product.F16 = product.LocalId ?? null;
                    db.SaveChanges();
                    var _images = _product.ImageProducts;
                    if (_images.Count == 3)
                    {
                        ImageProduct imageproduct = (ImageProduct)_images.ElementAt(0);
                        if (imageproduct != null)
                        {
                            imageproduct.F3 = hinh_0 ?? null;
                            db.SaveChanges();
                        }
                        ImageProduct imageproduct1 = (ImageProduct)_images.ElementAt(1);
                        if (imageproduct1 != null)
                        {
                            imageproduct1.F3 = hinh_1 ?? null;
                            db.SaveChanges();
                        }
                        ImageProduct imageproduct2 = (ImageProduct)_images.ElementAt(2);
                        if (imageproduct2 != null)
                        {
                            imageproduct2.F3 = hinh_2 ?? null;
                            db.SaveChanges();
                        }
                    }

                    if (_images.Count == 2)
                    {
                        ImageProduct imageproduct = (ImageProduct)_images.ElementAt(0);
                        if (imageproduct != null)
                        {
                            imageproduct.F3 = hinh_0 ?? null;
                            db.SaveChanges();
                        }
                        ImageProduct imageproduct1 = (ImageProduct)_images.ElementAt(1);
                        if (imageproduct1 != null)
                        {
                            imageproduct1.F3 = hinh_1 ?? null;
                            db.SaveChanges();
                        }                        
                        if (!string.IsNullOrWhiteSpace(hinh_2))
                        {
                            ImageProduct _imageProduct = new ImageProduct();
                            _imageProduct.F2 = _product.F1;
                            _imageProduct.F3 = hinh_2;
                            db.ImageProducts.Add(_imageProduct);
                            db.SaveChanges();
                        }
                
                    }

                    if (_images.Count == 1)
                    {
                        ImageProduct imageproduct = (ImageProduct)_images.ElementAt(0);
                        if (imageproduct != null)
                        {
                            imageproduct.F3 = hinh_0 ?? null;
                            db.SaveChanges();
                        }
                        var listImage = new List<string> { hinh_1, hinh_2 };
                        if (listImage != null)
                        {
                            var xx = from a in listImage where !string.IsNullOrWhiteSpace(a) select a;
                            foreach (var item in xx)
                            {
                                ImageProduct _imageProduct = new ImageProduct();
                                _imageProduct.F2 = _product.F1;
                                _imageProduct.F3 = item.ToString();
                                db.ImageProducts.Add(_imageProduct);
                                db.SaveChanges();
                            }
                        }                        
                    }

                    if (_images.Count == 0)
                    {
                        var listImage = new List<string> { hinh_0, hinh_1, hinh_2 };
                        if (listImage != null)
                        {
                            var xx = from a in listImage where !string.IsNullOrWhiteSpace(a) select a;
                            foreach (var item in xx)
                            {
                                ImageProduct _imageProduct = new ImageProduct();
                                _imageProduct.F2 = _product.F1;
                                _imageProduct.F3 = item.ToString();
                                db.ImageProducts.Add(_imageProduct);
                                db.SaveChanges();
                            }
                        }
                        
                    }
                    
                }

                TempData["UpdateProduct"] = "Cập nhật sản phẩm <b>" + product.ProductName + "</b> thành công.";
                return RedirectToRoute("ProductEdit");
            }
            
            return View(product);
        }

        [AllowAnonymous]
        public async Task<ActionResult> GianHang(int? page, string username)
        {            
            if (!string.IsNullOrWhiteSpace(username))
            {
                this.ApplicationDbContext = new ApplicationDbContext();
                this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
                var user = await UserManager.FindByNameAsync(username);
                List<EntityProduct> lstProduct = new List<EntityProduct>();
                if (user != null)
                {
                    ViewBag.TenGianHang = user.TenNguoiBan;
                    var products = (from p in db.Products where p.F14 == user.Id select p);
                    foreach (var item in products)
                    {
                        var pr = new EntityProduct()
                        {
                            SanPhamId = item.F1,
                            TenSp = item.F2,
                            TinhTrangSp = item.F5,
                            TheLoai = item.F6,
                            GiaBan = item.F3,
                            NgayDang = item.F10,
                            TenNguoiBan = item.AspNetUser.TenNguoiBan,
                            SoDienThoaiNgBan = item.AspNetUser.PhoneNumber,
                            AnhSanPham = item.F11,
                        };
                        lstProduct.Add(pr);
                    }

                    if (lstProduct.Count > 0)
                    {
                        lstProduct = lstProduct.OrderByDescending(x => x.NgayDang).ToList();
                        // Phan trang o day ne
                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        var onePageOfProducts = lstProduct.ToPagedList(pageNumber, pageSize);
                        ViewBag.ProductLists = onePageOfProducts;
                    }

                }
            }           

            return View();
          
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddToLog(string inputsearch)
        {
            string x = string.Empty;
            if (!string.IsNullOrWhiteSpace(inputsearch))
            {
                var _log = db.Logs.Where(l => l.Keyword == inputsearch).FirstOrDefault();
                if (_log != null)
                {
                    _log.Keyword = inputsearch;
                    _log.Count += 1;
                    db.SaveChanges();
                    x = "Cập nhật thành công";                    
                }
                else
                {
                    Log _logNew = new Log();
                    _logNew.Keyword = inputsearch;
                    _logNew.Count = 1;
                    db.Logs.Add(_logNew);
                    db.SaveChanges();
                    x = "Thêm log thành công";                    
                }
            }
            return Json(x, JsonRequestBehavior.AllowGet);
        }


    }
}