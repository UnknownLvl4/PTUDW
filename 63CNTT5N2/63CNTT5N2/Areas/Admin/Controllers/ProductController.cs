using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.Model;
using MyClass.DAO;
using _63CNTT5N2.Library;
using System.IO;

namespace _63CNTT5N2.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        ProductsDAO productsDAO = new ProductsDAO();
        CategoriesDAO categoriesDAO = new CategoriesDAO();
        SuppliersDAO suppliersDAO = new SuppliersDAO();
        /////////////////////////////////////////////////////////////
        // GET: Admin/Product/Index
        public ActionResult Index()
        {
            return View(productsDAO.getList("Index"));
        }
        /////////////////////////////////////////////////////////////
        // GET: Admin/Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            return View(products);
        }
        /////////////////////////////////////////////////////////////
        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatId - truy van tu bang Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");//sai SupplierID - truy van bang Suppliers
            //dung de lua chon tu danh sach droplist nhu bang Categories: ParentID va Supplier: ParentID
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Products products)
        {
            if (ModelState.IsValid)
            {
                //xu ly thong tin tu dong cho mot so truong
                //Xu ly tu dong cho: CreateAt
                products.CreateAt = DateTime.Now;
                //Xu ly tu dong cho: UpdateAt
                products.UpdateAt = DateTime.Now;
                //Xu ly tu dong cho: CreateBy
                products.CreateBy = Convert.ToInt32(Session["UserId"]);
                //Xu ly tu dong cho: UpdateBy
                products.UpdateBy = Convert.ToInt32(Session["UserId"]);
                //Xu ly tu dong cho: Slug
                products.Slug = XString.Str_Slug(products.Name);

                //xu ly cho phan upload hinh anh
                var img = Request.Files["img"];//lay thong tin file
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;//abc-def-1.jpg
                        //upload hinh
                        string PathDir = "~/Public/img/product/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //luu vao DB
                productsDAO.Insert(products);
                //thong bao thanh cong
                TempData["message"] = new XMessage("success", "Thêm sản phẩm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatId - truy van tu bang Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");//sai SupplierID - truy van bang Suppliers
            return View(products);
        }
        /////////////////////////////////////////////////////////////
        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatId - truy van tu bang Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");//sai SupplierID - truy van bang Suppliers
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Products products)
        {
            if (ModelState.IsValid)
            {
                //xu ly tu dong cho cac truong: CreateAt/By, UpdateAt/By, Slug, OrderBy

                //Xu ly tu dong cho: UpdateAt
                products.UpdateAt = DateTime.Now;
                //Xu ly tu dong cho: Slug
                products.Slug = XString.Str_Slug(products.Name);

                //truoc khi cap nhat lai anh moi thi xoa anh cu
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "~/Public/img/product/";
                if (img.ContentLength != 0 && products.Img != null)//ton tai mot logo cua NCC tu truoc
                {
                    //xoa anh cu
                    string DelPath = Path.Combine(Server.MapPath(PathDir), products.Img);
                    System.IO.File.Delete(DelPath);
                }
                //upload anh moi cua NCC
                //xu ly cho phan upload hinh anh

                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;//abc-def-1.jpg
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //cap nhat mau tin vao DB
                productsDAO.Update(products);
                //thong bao thanh cong
                TempData["message"] = new XMessage("success", "Cập nhật sản phẩm thành công");
                return RedirectToAction("Index");
            }
            return View(products);
        }
        /////////////////////////////////////////////////////////////
        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = productsDAO.getRow(id);
            productsDAO.Delete(products);
            //thong bao thanh cong
            TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
            return RedirectToAction("Trash");
        }

        ///////////////////////////////////////////////////////////////////
        // GET: Admin/Products/Status/5
        public ActionResult Status(int? id)
        {//khong lien quan den hinh anh
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }

            //tim row co id == id cua Nha CC can thay doi Status
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //kiem tra trang thai cua status, neu hien tai la 1 ->2 va nguoc lai
            products.Status = (products.Status == 1) ? 2 : 1;
            //cap nhat gia tri cho UpdateAt
            products.UpdateAt = DateTime.Now;
            //cap nhat lai DB
            productsDAO.Update(products);
            //thong bao thanh cong
            TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
            //tra ket qua ve Index
            return RedirectToAction("Index");
        }

        ///////////////////////////////////////////////////////////////////
        /// MoveTrash
        // GET: Admin/Supplier/MoveTrash/5
        public ActionResult MoveTrash(int? id)
        {//chua lien quan den hinh anh
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Xóa mẩu tin thất bại");
                return RedirectToAction("Index");
            }

            //tim row co id == id cua loai SP can thay doi Status
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Xóa mẩu tin thất bại");
                return RedirectToAction("Index");
            }
            //trang thai cua status = 0
            products.Status = 0;
            //cap nhat gia tri cho UpdateAt
            products.UpdateAt = DateTime.Now;

            //cap nhat lai DB
            productsDAO.Update(products);
            //thong bao thanh cong
            TempData["message"] = new XMessage("success", "Mẩu tin được chuyển vào thùng rác");
            //tra ket qua ve Index
            return RedirectToAction("Index");
        }

        ///////////////////////////////////////////////////////////////////
        // GET: Admin/Products/Trash
        public ActionResult Trash()
        {
            return View(productsDAO.getList("Trash"));//chi hien thi cac dong co status 0
        }

        ///////////////////////////////////////////////////////////////////
        /// Recover: Khong lien quan den hinh anh
        // GET: Admin/Products/Recover/5
        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Phục hồi mẩu tin thất bại");
                return RedirectToAction("Index");
            }
            //tim row co id == id cua loai SP can thay doi Status
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Phục hồi mẩu tin thất bại");
                return RedirectToAction("Index");
            }
            //trang thai cua status = 2
            products.Status = 2;//truoc recover=0
            //cap nhat gia tri cho UpdateAt
            products.UpdateAt = DateTime.Now;

            //cap nhat lai DB
            productsDAO.Update(products);
            //thong bao thanh cong
            TempData["message"] = new XMessage("success", "Phục hồi mẩu tin thành công");
            //tra ket qua ve Index
            return RedirectToAction("Trash");//action trong SupllierDAO
        }
    }
}
