using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTMDT.Models;
using PagedList;
using System.Threading.Tasks;
using WebTMDT.Helpers;

namespace WebTMDT.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private langson12Entities db = new langson12Entities();
        // GET: Admin
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
            return View();
        }

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Administrator")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }  

         // /admin/products/list
        public ActionResult ProductsList(int? page, string searchQuery)
        {            
            var products = db.Products.Select(item => new EntityProduct()
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
            }).ToList();


            if (products.Count > 0)
            {
                if (!String.IsNullOrEmpty(searchQuery))
                {
                    ViewBag.searchQuery = searchQuery.ToLower();
                    products = products.Where(s => s.TenSp.ToLower().Contains(searchQuery.ToLower())).ToList();
                }

                products = products.OrderByDescending(x => x.NgayDang).ToList();
                // Phan trang o day ne
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                var onePageOfProducts = products.ToPagedList(pageNumber, pageSize);
                ViewBag.ProductLists = onePageOfProducts;
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
           
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                ViewBag.ProductNotFound = "Sản phẩm không tồn tại hoặc đã bị xóa.";
                return View();
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
                CategoryId = product.F17,
                ParentCatId = product.F18,
                SubCatId = product.F15,
                LocalId = product.F16,
                ProductImages = product.ImageProducts.Select(x => new ProductImages() { ProductId = x.F1, UrlImage = x.F3 }).ToList()
            };

            var Cat = db.Categories.ToList();
            ViewBag.Category = Cat;
            var LocalData = db.Locals.ToList();
            ViewBag.LocalData = LocalData;

            ViewBag.ProductType = Configs.CreateListProductType();
            ViewBag.ProductStatus = Configs.CreateListProductStatus();

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
                    //!string.IsNullOrWhiteSpace(product.ProductPrice) ? product.ProductPrice.Replace(",", "") : null;
                    string strGiaBanSp = product.ProductPrice != null ? product.ProductPrice.Replace(",", "").ToString() : null;
                    _product.F3 = !string.IsNullOrEmpty(strGiaBanSp) ? Convert.ToInt32(strGiaBanSp) : (int?)null;
                    _product.F4 = product.ProductVAT;
                    _product.F5 = product.ProductStatus ?? null;
                    _product.F6 = product.ProductType ?? null;
                    _product.F7 = product.ProductMethod ?? null;
                    _product.F8 = product.ProductGuarantee ?? null;
                    _product.F9 = product.ProductPromotion ?? null;
                    _product.F10 = DateTime.Now;
                    _product.F11 = product.ProductAvatar ?? "/Content/Images/no-image-available.png";
                    _product.F12 = product.ProductDescription ?? null;
                    _product.F13 = null;
                    _product.F15 = product.SubCatId ?? null;
                    var _subcat = db.Categories.Where(x => x.F1 == product.SubCatId).FirstOrDefault();
                    _product.F17 = _subcat.Category2.F1;
                    _product.F18 = _subcat.Category2.Category2.F1;
                    _product.F16 = product.LocalId ?? null;
                    db.Entry(_product).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    var _images = _product.ImageProducts;
                    if (_images.Count == 3)
                    {
                        ImageProduct imageproduct = (ImageProduct)_images.ElementAt(0);
                        if (imageproduct != null)
                        {
                            imageproduct.F3 = hinh_0 ?? null;
                            db.Entry(imageproduct).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        ImageProduct imageproduct1 = (ImageProduct)_images.ElementAt(1);
                        if (imageproduct1 != null)
                        {
                            imageproduct1.F3 = hinh_1 ?? null;
                            db.Entry(imageproduct1).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        ImageProduct imageproduct2 = (ImageProduct)_images.ElementAt(2);
                        if (imageproduct2 != null)
                        {
                            imageproduct2.F3 = hinh_2 ?? null;
                            db.Entry(imageproduct2).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    if (_images.Count == 2)
                    {
                        ImageProduct imageproduct = (ImageProduct)_images.ElementAt(0);
                        if (imageproduct != null)
                        {
                            imageproduct.F3 = hinh_0 ?? null;
                            db.Entry(imageproduct).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        ImageProduct imageproduct1 = (ImageProduct)_images.ElementAt(1);
                        if (imageproduct1 != null)
                        {
                            imageproduct1.F3 = hinh_1 ?? null;
                            db.Entry(imageproduct1).State = System.Data.Entity.EntityState.Modified;
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
                            db.Entry(imageproduct).State = System.Data.Entity.EntityState.Modified;
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
                return RedirectToRoute("AdminProductEdit");
            }

            return View(product);
        }


        public ActionResult Delete(long? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction("NotFound", "Home");
            }
            Product _product = db.Products.Find(id);
            if (_product == null)
            {
                ViewBag.ProductNotFound = "Sản phẩm không tồn tại hoặc đã bị xóa.";
                return View();
            }
            //db.Products.Remove(_product);
            //db.SaveChanges();
            return View(_product);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long? id)
        {
            Product _product = await db.Products.FindAsync(id);
            if (_product == null)
            {
                ViewBag.ProductNotFound = "Sản phẩm không tồn tại hoặc đã bị xóa.";
                return View();
            }

            if (_product.ImageProducts.Count > 0)
            {
                var _limg = _product.ImageProducts.ToList();
                foreach (var item in _limg)
                {
                    db.ImageProducts.Remove(item);
                    await db.SaveChangesAsync();
                }
            }
            db.Products.Remove(_product);
            await db.SaveChangesAsync();
            return RedirectToRoute("AdminProductsList");
        }

        //public ActionResult AddMenu()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddMenu(DanhMucEdit model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (model.ParentId == null)
        //        {
        //            Category _cat = new Category();
        //            _cat.F2 = model.CatName ?? null;
        //            _cat.F3 = null;
        //            db.Categories.Add(_cat);
        //            db.SaveChanges();
        //        }
        //    }
        //    return View();
        //}

        //public ActionResult Menu()
        //{
        //   List<DanhMuc> data = db.Categories.Select(x=>new DanhMuc() {
        //        CatId = x.F1,
        //        CatName = x.F2,
        //        ParentId = x.F3
        //   }).ToList();

        //   var presidents = data.Where(x => x.ParentId == null).FirstOrDefault();
        //   SetChildren(presidents, data);


        //   return View(presidents);
        //}

        //private void SetChildren(DanhMuc model, List<DanhMuc> danhmuc)
        //{
        //    var childs = danhmuc.Where(x => x.ParentId == model.CatId).ToList();
        //    if (childs.Count > 0)
        //    {
        //        foreach (var child in childs)
        //        {
        //            SetChildren(child, danhmuc);
        //            model.DanhMucs.Add(child);
        //        }
        //    }
        //}

    }
}