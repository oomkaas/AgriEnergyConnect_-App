using AgriEnergy.Models;
using AgriEnergy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AgriEnergy.Controllers
{
    //[Route("/Admin/[controller]/{action=Index}/{id}")]
    [Authorize(Roles ="Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 10;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            IQueryable<Product> query = context.Products;

            //search functionality
            if (search != null)
            {
                query=query.Where(p=>p.Name.Contains(search) ||p.Category.Contains(search));
            }

            //sort functionality
            string[] validColumns = { "Id", "Name", "Category", "price", "ProdDate" };
            string[] validOrderBy = { "desc", "asc" };

            if (!validColumns.Contains(column))
            {
                column = "Id";
            }

            if (!validOrderBy.Contains(orderBy))
            {
                orderBy = "desc";
            }

            if (column == "Name")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }

            else if (column == "Category")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }

            else if (column == "price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.price);
                }
            }

            else if (column == "ProdDate")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.ProdDate);
                }
                else
                {
                    query = query.OrderByDescending(p => p.ProdDate);
                }
            }

            else 
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            //query =query.OrderByDescending(p => p.Id);
            //pagination functionality
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products = query.ToList();
            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

            ViewData["Search"] = search ??"";

            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if(!ModelState.IsValid)
            {
                return View(productDto);
            }

            //save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            //    string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);
            //using (var stream = new FileStream(imageFullPath, FileMode.Create))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            //save the new product in the database
            Product product = new Product() 
            { 
                Name = productDto.Name,
                Category = productDto.Category,
                price = productDto.price,
                ImageFileName = newFileName,
                ProdDate = DateTime.Now,
            };

            context.Products.Add(product);
            context.SaveChanges();


            return RedirectToAction("Index", "Products");
        }

        public IActionResult Edit(int id) 
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            //create productDto from product
            var productDto = new ProductDto()
            {
                Name = product.Name,
                Category = product.Category,
                price = product.price,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["ProdDate"] = product.ProdDate.ToString("MM/dd/yyyy");

            return View(productDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if(!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["ProdDate"] = product.ProdDate.ToString("MM/dd/yyyy");

                return View(productDto);
            }

            //update the image file if we have a new image file
            string newFileName = product.ImageFileName;
            if(productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

                string imageFullPath = environment.WebRootPath + "/products" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                //delete the old image
                string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(imageFullPath);
            }

            //update the product in the database
            product.Name = productDto.Name;
            product.Category = productDto.Category;
            product.price = productDto.price;
            product.ImageFileName = newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("index", "products");
            }

            string imageFullPath = environment.WebRootPath + "/products" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Products.Remove(product);
            context.SaveChanges();

            return RedirectToAction("index", "Products");

        }
    }
}
