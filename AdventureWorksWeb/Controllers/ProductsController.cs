using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdventureWorksWeb.Models.Entitites;
using Ninject;
using AdventureWorksWeb.Models.ViewModels;
using AdventureWorksWeb.Models;
using System.Web.UI;
using Rebus.Bus;
using Common.Events;

namespace AdventureWorksWeb.Controllers
{
    public class ProductsController : Controller
    {
        private AdventureWorks _context;
        private IKernel _kernel;
        private IBus _bus;

        public ProductsController(IKernel kernel, AdventureWorks context, IBus bus)
        {
            _kernel = kernel;
            _context = context;
            _bus = bus;
        }

        // GET: Products
        public async Task<ActionResult> Index(string term, string filter, int page = 0)
        {
            var paging = new PagingInfo()
            {
                CurrentPage = page
            };
            var model = await ProductsViewModel.LoadAsync(_kernel, term, filter, paging);

            return View(model);
        }

        public async Task<ActionResult> Suggest(string term)
        {
            var model = await ProductSuggestionViewModel.LoadAsync(_kernel, term);

            return this.Json(model.Products.Select(x => x.Name), JsonRequestBehavior.AllowGet);
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.ProductCategoryID = new SelectList(_context.ProductCategories, "ProductCategoryID", "Name");
            ViewBag.ProductModelID = new SelectList(_context.ProductModels, "ProductModelID", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductID,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,ProductCategoryID,ProductModelID")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProductCategoryID = new SelectList(_context.ProductCategories, "ProductCategoryID", "Name", product.ProductCategoryID);
            ViewBag.ProductModelID = new SelectList(_context.ProductModels, "ProductModelID", "Name", product.ProductModelID);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductCategoryID = new SelectList(_context.ProductCategories, "ProductCategoryID", "Name", product.ProductCategoryID);
            ViewBag.ProductModelID = new SelectList(_context.ProductModels, "ProductModelID", "Name", product.ProductModelID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind(Include = "ProductID,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,ProductCategoryID,ProductModelID")] Product product)
        {
            if (ModelState.IsValid)
            {
                var original = await _context.Products.SingleAsync(x => x.ProductID == id);

                this.UpdateModel(original);

                await _context.SaveChangesAsync();

                var message = new ProductUpdated()
                {
                    ProductID = id.ToString(),
                    Name = product.Name
                };

                await _bus.Publish(message);

                return RedirectToAction("Index");
            }
            ViewBag.ProductCategoryID = new SelectList(_context.ProductCategories, "ProductCategoryID", "Name", product.ProductCategoryID);
            ViewBag.ProductModelID = new SelectList(_context.ProductModels, "ProductModelID", "Name", product.ProductModelID);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
