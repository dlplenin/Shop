using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Data;
using Shop.Web.Data.Entities;
using Shop.Web.Helpers;
using Shop.Web.Models;

namespace Shop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly IUserHelper userHelper;

        public ProductsController(IProductRepository productRepository, IUserHelper userHelper)
        {
            this.productRepository = productRepository;
            this.userHelper = userHelper;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(productRepository.GetAll());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,LastPurchase,LastSale,IsAvailabe,Stock,ImageFile")] ProductViewModel productView)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (productView.ImageFile != null && productView.ImageFile.Length > 0)
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), 
                        "wwwroot\\images\\Products", 
                        productView.ImageFile.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await productView.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/Products/{productView.ImageFile.FileName}";
                }


                var product = ToProduct(productView, path);

                // TODO: Pending to change to: this.User.Identity.Name
                product.User = await this.userHelper.GetUserByEmailAsync("dp@gmail.com");
                await productRepository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(productView);
        }

        private Product ToProduct(ProductViewModel productView, string path)
        {
            return new Product
            {
                Id = productView.Id,
                ImageUrl = path,
                IsAvailabe = productView.IsAvailabe,
                LastPurchase = productView.LastPurchase,
                LastSale = productView.LastSale,
                Name = productView.Name,
                Price = productView.Price,
                Stock = productView.Stock,
                User = productView.User

            };
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,ImageUrl,LastPurchase,LastSale,IsAvailabe,Stock")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Pending to change to: this.User.Identity.Name
                    product.User = await this.userHelper.GetUserByEmailAsync("dp@gmail.com");
                    await productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await productRepository.ExistAsync(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            await productRepository.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }
    }
}
