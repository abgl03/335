using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Linq.Dynamic;

namespace WebApplication1.Controllers {
    public class SqlController : Controller {
        public SqlController() {
            Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo("en-NZ"); // ru-RU
        }

        // GET: Sql
        public string Index() {
            return "<h1>MVC Demo</h1>";
        }

        // GET: Sql
        public ActionResult Index2() {
            ViewBag.text = "MVC Demo";
            ViewBag.count = 3;
            return View();
        }

        // GET: WebGrid?page=1&rowsPerPage=3&sort=OrderID&sortDir=ASC
        public ActionResult WebGrid(int page = 1, int rowsPerPage = 3, string sortCol = "OrderID", string sortDir = "ASC") {
            List<MyModel> res;
            int count;
            string sql;
            
            using (var nwd = new NorthwindEntities()) {
                var _res = nwd.MyOrders
                    .OrderBy(sortCol+" "+sortDir)
                    .Skip((page - 1) * rowsPerPage)
                    .Take(rowsPerPage)
                    .Select(o => new MyModel {
                        OrderID = o.OrderID,
                        Description = o.Description,
                        Price = o.Price,
                        Items = o.Items,
                        CustomerID = o.CustomerID,
                        Name = o.MyCustomer.Name,
                        Age = o.MyCustomer.Age,
                    });

                res = _res.ToList();
                count = nwd.MyOrders.Count();
            }

            ViewBag.sortCol = sortCol;
            ViewBag.rowsPerPage = rowsPerPage;
            ViewBag.count = count;
            return View(res);
        }
    }


    public class MyModel {
        [Key]
        public string OrderID { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Items { get; set; }
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
    }
}