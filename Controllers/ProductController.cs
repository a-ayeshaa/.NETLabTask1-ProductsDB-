using LabTask.DB;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LabTask.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product p)
        {
            var db = new Products_dbEntities();
            db.Products.Add(p);
            db.SaveChanges();
            return RedirectToAction("AddProduct");
        }

        [HttpGet]
        public ActionResult ShowProduct()
        {
            var db = new Products_dbEntities();
            var products = db.Products.ToList();
            return View(products);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var db = new Products_dbEntities();
            var item = (from p in db.Products
                        where p.Id == id
                        select p).SingleOrDefault();
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(Product p)
        {
            var db = new Products_dbEntities();
            var item = (from pr in db.Products
                        where pr.Id == p.Id
                        select pr).SingleOrDefault();
            item.Name = p.Name;
            item.Price= p.Price;
            item.Qty= p.Qty;
            db.SaveChanges();
            return RedirectToAction("ShowProduct");

        }

        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            var db = new Products_dbEntities();
            var item = (from p in db.Products
                        where p.Id == id
                        select p).SingleOrDefault();
            db.Products.Remove(item);   
            db.SaveChanges();
            return RedirectToAction("ShowProduct");
        }

        [HttpGet]
        public ActionResult Product()
        {
            var db = new Products_dbEntities();
            var products = db.Products.ToList();
            return View(products);
        }
        [HttpPost]
        public ActionResult Product(Product p)
        {
            var db = new Products_dbEntities();
            var item = (from pr in db.Products
                        where pr.Id == p.Id
                        select pr).SingleOrDefault();
            item.Qty= item.Qty - p.Qty;
            db.SaveChanges();
            if (Session["cart"]==null)
            {
                var i = new List<Product>();
                i.Add(new Product()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Qty = p.Qty
                });
                string json = new JavaScriptSerializer().Serialize(i);
                Session["cart"] = json;
            }
            else
            {
                var i = new JavaScriptSerializer().Deserialize<List<Product>>(Session["cart"].ToString());
                i.Add(new Product()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Qty = p.Qty
                });
                string json = new JavaScriptSerializer().Serialize(i);
                Session["cart"] = json;
            }
            return RedirectToAction("Cart");
        }

        public ActionResult Cart()
        {
            if (Session["cart"]==null)
            {
                return View();
            }
            var i = new JavaScriptSerializer().Deserialize<List<Product>>(Session["cart"].ToString());
            return View(i);
        }

        public ActionResult ConfirmOrder()
        {
            var items = new JavaScriptSerializer().Deserialize<List<Product>>(Session["cart"].ToString());
            var total = 0.00f;
            foreach (var item in items)
            {
                total += (float)item.Qty * (float)item.Price;
            }

            var db = new Products_dbEntities();
            db.Orders.Add(new Order()
            {
                Name = "Username",
                TotalPrice = total
            });
            db.SaveChanges();

            return RedirectToAction("Orders");
        }

        public ActionResult Orders()
        {
            var d = new Products_dbEntities();
            var orders = d.Orders.ToList();
            return View(orders);
        }
    }
}