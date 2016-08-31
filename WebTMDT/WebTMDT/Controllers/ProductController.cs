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
using System.ComponentModel;

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
            ViewBag.ProductType = new List<ProductType>() {
                new ProductType() { ProductTypeName = "Hàng chính hãng" },
                new ProductType() { ProductTypeName = "Hàng xách tay" },
                new ProductType() { ProductTypeName = "Hàng lỗi" },
                new ProductType() { ProductTypeName = "Hàng xuất khẩu" },
                new ProductType() { ProductTypeName = "Hàng khác" },
            };
            ViewBag.ProductStatus = new List<ProductStatus>() { 
                new ProductStatus() { ProductStatusName = "Mới 100%" },
                new ProductStatus() { ProductStatusName = "Mới 90%" },
                new ProductStatus() { ProductStatusName = "Mới 80%" },
                new ProductStatus() { ProductStatusName = "Hàng like new" },
                new ProductStatus() { ProductStatusName = "Hàng cũ" },
                new ProductStatus() { ProductStatusName = "Hàng thanh lý" },
                new ProductStatus() { ProductStatusName = "Hàng cho không" },
                new ProductStatus() { ProductStatusName = "Hàng khác" }
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
                _product.F10 = DateTime.Now;
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
        private string _fullPath_ = "";
        private string _fileName_ = "";
        SprightlySoftAWS.S3.CalculateHash MyCalculateHash;
        SprightlySoftAWS.S3.Upload MyUpload;
        System.ComponentModel.BackgroundWorker UploadBackgroundWorker;
        System.ComponentModel.BackgroundWorker CalculateHashBackgroundWorker;
        [HttpPost]
        public ActionResult Upload()
        {
            string relativeUrl = string.Empty;
            var file = Request.Files[0];
            if (!Configs.IsImage(file))
            {
                return Json(relativeUrl, JsonRequestBehavior.AllowGet); 
            }
            var fileName = String.Format("{0}.jpg", Guid.NewGuid().ToString()); 
            string path = Server.MapPath("~/Content/Images/Products/")+fileName;
            relativeUrl = "/Content/Images/Products/" + fileName;
            file.SaveAs(path);
            _fullPath_ = path;
            _fileName_ = fileName;
            init();
            //FileInfo fileInfo = new FileInfo(Server.MapPath("~/Content/Images/Products/" + fileName));
            //if (fileInfo.Exists)
            //{
            //    fileName = string.Format("{0:dd_MM_yyyy_hh_mm_ss_tt}_" + fileName, DateTime.Now);
            //    string strpath = Path.Combine(path, fileName);
            //    file.SaveAs(strpath);
            //    relativeUrl = "/Content/Images/Products/" + fileName;
            //}
            //else
            //{
            //    string strpath = Path.Combine(path, fileName);
            //    file.SaveAs(strpath);
            //    relativeUrl = "/Content/Images/Products/" + fileName;
            //}
            return Json(relativeUrl, JsonRequestBehavior.AllowGet);
        }
        public async Task init()
        {
            MyCalculateHash = new SprightlySoftAWS.S3.CalculateHash();
            //yCalculateHash.ProgressChangedEvent += MyCalculateHash_ProgressChangedEvent;

            MyUpload = new SprightlySoftAWS.S3.Upload();
            //MyUpload.ProgressChangedEvent += MyUpload_ProgressChangedEvent;

            CalculateHashBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            CalculateHashBackgroundWorker.DoWork += CalculateHashBackgroundWorker_DoWork;
            CalculateHashBackgroundWorker.RunWorkerCompleted += CalculateHashBackgroundWorker_RunWorkerCompleted;

            UploadBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            UploadBackgroundWorker.DoWork += UploadBackgroundWorker_DoWork;
            UploadBackgroundWorker.RunWorkerCompleted += UploadBackgroundWorker_RunWorkerCompleted;
            //Application.DoEvents();
            StreamWriter SW = new StreamWriter(HttpContext.Server.MapPath("../") + "init.txt");
            SW.WriteLine("public void init()");
            SW.Close();
            //Run the hash calculation in a BackgroundWorker process.  Calculating the hash of a
            //large file will take a while.  Running the process in a BackgroundWorker will prevent
            //the form from locking up.

            //Use a hash table to pass parameters to the function in the BackgroundWorker.
            Task task = new Task(ProcessDataAsync);
            task.Start();
            task.Wait();

        }
        public async void ProcessDataAsync()
        {
            // Start the HandleFile method.
            Task<string> task = ok();
            string x = await task;

        }
        public async Task<string> ok()
        {
            System.Collections.Hashtable CalculateHashHashTable = new System.Collections.Hashtable();
            CalculateHashHashTable.Add("LocalFileName", _fullPath_);
            CalculateHashBackgroundWorker.RunWorkerAsync(CalculateHashHashTable);
            StreamWriter SW = new StreamWriter(HttpContext.Server.MapPath("../") + "ok.txt");
            SW.WriteLine("public async Task<string> ok()");
            SW.Close();
            return "ok";
        }
        private void CalculateHashBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Call the CalculateMD5FromFile function and set the result.  When the function is complete
            //the RunWorkerCompleted event will fire.  Use the LocalFileName value from the passed hash table.
            System.Collections.Hashtable CalculateHashHashTable = e.Argument as System.Collections.Hashtable;
            e.Result = MyCalculateHash.CalculateMD5FromFile(CalculateHashHashTable["LocalFileName"].ToString());
        }
        private void CalculateHashBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //If the CalculateMD5FromFile function was successful upload the file.
            if (MyCalculateHash.ErrorNumber == 0)
            {
                StreamWriter SW = new StreamWriter(HttpContext.Server.MapPath("../") + "startupload1.txt");
                SW.WriteLine("0k");
                SW.Close();

                //Set the extra request headers to send with the upload
                Dictionary<String, String> ExtraRequestHeaders = new Dictionary<String, String>();

                ExtraRequestHeaders.Add("Content-Type", "image/jpeg");
                //ExtraRequestHeaders.Add("x-amz-acl", "public-read");


                //Use the MD5 hash that was calculated previously.
                ExtraRequestHeaders.Add("Content-MD5", e.Result.ToString());



                String RequestURL;
                RequestURL = MyUpload.BuildS3RequestURL(true, "s3.amazonaws.com", "bananhso", _fileName_, "");

                String RequestMethod = "PUT";

                ExtraRequestHeaders.Add("x-amz-date", DateTime.UtcNow.ToString("r"));

                String AuthorizationValue;
                AuthorizationValue = MyUpload.GetS3AuthorizationValue(RequestURL, RequestMethod, ExtraRequestHeaders, "AKIAIR2TUTKM6EM5Q6WQ", "Uc5myRRoncvKFGXrL9gzaK5YwHYh6OXAUqZal4Tu");
                ExtraRequestHeaders.Add("Authorization", AuthorizationValue);
                SW = new StreamWriter(HttpContext.Server.MapPath("../") + "startupload2.txt");
                SW.WriteLine(_fullPath_);
                SW.Close();
                //Create a hash table of of parameters to sent to the upload function.
                System.Collections.Hashtable UploadHashTable = new System.Collections.Hashtable();
                UploadHashTable.Add("RequestURL", RequestURL);
                UploadHashTable.Add("RequestMethod", RequestMethod);
                UploadHashTable.Add("ExtraRequestHeaders", ExtraRequestHeaders);
                UploadHashTable.Add("LocalFileName", _fullPath_);

                //Run the UploadFile call in a BackgroundWorker to prevent the Window from freezing.
                try
                {
                    UploadBackgroundWorker.RunWorkerAsync(UploadHashTable);
                }
                catch (Exception notup)
                {
                    _fullPath_ = "";
                }
                SW = new StreamWriter(HttpContext.Server.MapPath("../") + "CalculateHashBackgroundWorker_RunWorkerCompleted.txt");
                SW.WriteLine("private void CalculateHashBackgroundWorker_RunWorkerCompleted");
                SW.Close();
            }
            else
            {
                _fullPath_ = "";
            }
        }
        private void UploadBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Run the UploadFile call.
            System.Collections.Hashtable UploadHashTable = e.Argument as System.Collections.Hashtable;
            e.Result = MyUpload.UploadFile(UploadHashTable["RequestURL"].ToString(), UploadHashTable["RequestMethod"].ToString(), UploadHashTable["ExtraRequestHeaders"] as Dictionary<String, String>, UploadHashTable["LocalFileName"].ToString());
        }
        private void UploadBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //System.Diagnostics.Debug.Print("");
            //System.Diagnostics.Debug.Print(MyUpload.LogData);
            //System.Diagnostics.Debug.Print("");

            //EnableDisableEnd();

            if (Convert.ToBoolean(e.Result) == true)
            {
                //MessageBox.Show("Upload complete.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                StreamWriter SW = new StreamWriter(HttpContext.Server.MapPath("../") + "uploadok.txt");
                SW.WriteLine("uploadok");
                SW.Close();
                //try
                //{

                //    if (System.IO.File.Exists(_fullPath_))
                //    {
                //        System.IO.File.Delete(_fullPath_);
                //    }
                //}
                //catch (Exception ex2) { }
                _fullPath_ = "";
            }
            else
            {
                _fullPath_ = "";
                //Show the error message.
                String ResponseMessage;

                if (MyUpload.ResponseString == "")
                {
                    ResponseMessage = MyUpload.ErrorDescription;
                }
                else
                {
                    System.Xml.XmlDocument XmlDoc = new System.Xml.XmlDocument();
                    XmlDoc.LoadXml(MyUpload.ResponseString);

                    System.Xml.XmlNode XmlNode;
                    XmlNode = XmlDoc.SelectSingleNode("/Error/Message");

                    ResponseMessage = XmlNode.InnerText;
                }
                StreamWriter SW = new StreamWriter(HttpContext.Server.MapPath("../") + "upload.txt");
                SW.WriteLine(ResponseMessage);
                SW.Close();
                //MessageBox.Show(ResponseMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        public ActionResult Search(string inputsearch, string f5, string f6, string f3, string f10, string f15, string f16, string f17, string f18, int? pg)
        {         
            var _p = db.Products.Select(p=>p);           

            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(inputsearch))
            {
                _p = _p.Where(o => o.F2.ToLower().Contains(inputsearch));
            }

            if (f18 == null) f18 = ""; if (f17 == null) f17 = ""; if (f15 == null) f15 = ""; if (f16 == null) f16 = ""; if (f3 == null) f3 = ""; if (f5 == null) f5 = ""; if (f6 == null) f6 = ""; if (f10 == null) f10 = "";
            if (f18 != null && f18 != "")
            {
                int _id = Convert.ToInt32(f18);
                _p = _p.Where(o => o.F18 == _id);
                var _cat = db.Categories.Where(o => o.F1 == _id).FirstOrDefault();
                ViewBag.f18n = _cat.F2;
            }

            if (f17 != null && f17 != "")
            {
                int _id = Convert.ToInt32(f17);
                _p = _p.Where(o => o.F17 == _id);
                var _cat = db.Categories.Where(o => o.F1 == _id).FirstOrDefault();
                ViewBag.f17n = _cat.F2;
            }

            if (f15 != null && f15 != "")
            {
                int _id = Convert.ToInt32(f15);
                _p = _p.Where(o => o.F15 == _id);
                var _cat = db.Categories.Where(o => o.F1 == _id).FirstOrDefault();
                ViewBag.f15n = _cat.F2;
            }

            if (f16 != null && f16 != "")
            {
                int _id = Convert.ToInt32(f16);
                _p = _p.Where(o => o.F16 == _id);
                var _local = db.Locals.Where(o => o.F1 == _id).FirstOrDefault();
                ViewBag.f16n = _local.F2;
            }

            if (f5 != null && f5 != "")
            {
                _p = _p.Where(o => o.F5 == f5);
            }

            if (f6 != null && f6 != "")
            {
                _p = _p.Where(o => o.F6 == f6);
            }

            if (f10 != null && f10 == "")
            {
                _p = _p.OrderByDescending(o => o.F10);
            }
            else if (f10 != null && f10 == "desc")
            {
                _p = _p.OrderByDescending(o => o.F10);
            }
            else if(f10 != null && f10 == "asc")
            {
                _p = _p.OrderBy(o => o.F10);
            }

            if (f3 != null &&  f3 != "")
            {               
                Configs.TwoNumber _x;
                switch (f3)
                {
                    case "desc":
                        _p = _p.OrderByDescending(x => x.F3);
                        break;
                    case "asc":
                        _p = _p.OrderBy(x => x.F3);
                        break;
                    case "1-3":
                        _x = Configs.GetNumber("1-3");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    case "3-5":
                        _x = Configs.GetNumber("3-5");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    case "5-7":
                        _x = Configs.GetNumber("5-7");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    case "7-9":
                        _x = Configs.GetNumber("7-9");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    case "9-11":
                        _x = Configs.GetNumber("9-11");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    case "11-13":
                        _x = Configs.GetNumber("11-13");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    case "13-15":
                        _x = Configs.GetNumber("13-15");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    case "15-17":
                        _x = Configs.GetNumber("15-17");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    case "17-19":
                        _x = Configs.GetNumber("17-19");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    case "19-20":
                        _x = Configs.GetNumber("19-20");
                        _p = _p.Where(x => x.F3 >= _x.Number1 && x.F3 <= _x.Number2);
                        break;
                    default: 
                        _x = Configs.GetNumber("0-1");
                        _p = _p.Where(x => x.F3 <= _x.Number2);
                        break;
                }
            }

            var Cat = db.Categories.ToList();
            ViewBag.Category = Cat;
            var LocalData = db.Locals.ToList();
            ViewBag.LocalData = LocalData;
            ViewBag.ProductTheLoai = new List<ProductType>() {
                new ProductType() { ProductTypeName = "Hàng chính hãng" },
                new ProductType() { ProductTypeName = "Hàng xách tay" },
                new ProductType() { ProductTypeName = "Hàng lỗi" },
                new ProductType() { ProductTypeName = "Hàng xuất khẩu" },
                new ProductType() { ProductTypeName = "Hàng khác" },
            };
            ViewBag.ProductTrangThai = new List<ProductStatus>() { 
                new ProductStatus() { ProductStatusName = "Mới 100%" },
                new ProductStatus() { ProductStatusName = "Mới 90%" },
                new ProductStatus() { ProductStatusName = "Mới 80%" },
                new ProductStatus() { ProductStatusName = "Hàng like new" },
                new ProductStatus() { ProductStatusName = "Hàng cũ" },
                new ProductStatus() { ProductStatusName = "Hàng thanh lý" },
                new ProductStatus() { ProductStatusName = "Hàng cho không" },
                new ProductStatus() { ProductStatusName = "Hàng khác" }
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

            ViewBag.inputsearch = inputsearch;
            ViewBag.f18 = f18;
            ViewBag.f17 = f17;
            ViewBag.f15 = f15;
            ViewBag.f16 = f16;
            ViewBag.f3 = f3;
            ViewBag.f10 = f10;
            ViewBag.f5 = f5;
            ViewBag.f6 = f6;
            //ViewBag.f5 = f5.Replace("%20", " ");
            //ViewBag.f6 = f6.Replace("%20", " ");


            List<ProductShow> _lstProducts = new List<ProductShow>();
            foreach (var p in _p)
            {
                var item = new ProductShow()
                {
                    SanPhamId = p.F1,
                    TenSp = p.F2,
                    TinhTrangSp = p.F5,
                    TheLoai = p.F6,
                    GiaBan = p.F3,
                    NgayDang = p.F10,
                    TenNguoiBan = p.AspNetUser.TenNguoiBan,
                    SDTNguoiBan = p.AspNetUser.PhoneNumber,
                    AnhSanPham = p.F11,
                    LocalId = p.F16,
                    SlugCat = Configs.unicodeToNoMark(p.Category.Category2.F2),
                    SlugSubCat = Configs.unicodeToNoMark(p.Category.F2),
                    slugTenSp = Configs.unicodeToNoMark(p.F2 != null ? p.F2 : "no-title"),
                    GianHang = p.AspNetUser.UserName,
                    SlugGianHang = Configs.unicodeToNoMark(p.AspNetUser.TenNguoiBan != null ? p.AspNetUser.TenNguoiBan : "no-title"),
                    SubCatId = p.F15,
                    CatId = p.F17,
                    ParentId = p.F18,
                    DiaDiem = p.Local.F2
                };
                _lstProducts.Add(item);
            }

            return View(_lstProducts.ToPagedList(pageNumber, pageSize));
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
            ViewBag.ProductType = new List<ProductType>() {
                new ProductType() { ProductTypeName = "Hàng chính hãng" },
                new ProductType() { ProductTypeName = "Hàng xách tay" },
                new ProductType() { ProductTypeName = "Hàng lỗi" },
                new ProductType() { ProductTypeName = "Hàng xuất khẩu" },
                new ProductType() { ProductTypeName = "Hàng khác" },
            };
            ViewBag.ProductStatus = new List<ProductStatus>() { 
                new ProductStatus() { ProductStatusName = "Mới 100%" },
                new ProductStatus() { ProductStatusName = "Mới 90%" },
                new ProductStatus() { ProductStatusName = "Mới 80%" },
                new ProductStatus() { ProductStatusName = "Hàng like new" },
                new ProductStatus() { ProductStatusName = "Hàng cũ" },
                new ProductStatus() { ProductStatusName = "Hàng thanh lý" },
                new ProductStatus() { ProductStatusName = "Hàng cho không" },
                new ProductStatus() { ProductStatusName = "Hàng khác" }
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
                    _product.F10 = DateTime.Now;
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
                            DiaDiem = item.Local.F2,
                            SlugCat = Configs.unicodeToNoMark(item.Category.Category2.F2),
                            slugTenSp = Configs.unicodeToNoMark(item.F2 != null ? item.F2 : "no-title"),
                            SubCatId = item.F15
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