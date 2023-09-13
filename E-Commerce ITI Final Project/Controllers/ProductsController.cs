using E_Commerce_ITI_Final_Project.Data;
using E_Commerce_ITI_Final_Project.DTOs;
using E_Commerce_ITI_Final_Project.Entities;
using E_Commerce_ITI_Final_Project.Helpers;
using E_Commerce_ITI_Final_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_ITI_Final_Project.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly StoreContext storeContext;
        private readonly UserManager<Account> userManager;

        public ProductsController(IWebHostEnvironment environment, StoreContext storeContext, UserManager<Account> userManager)
        {
            this.environment = environment;
            this.storeContext = storeContext;
            this.userManager = userManager;
        }

        #region Seller Products
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Index()
        {
            var seller = await userManager.GetUserAsync(User);
            var products = storeContext.Products.Include("Category").Include("Seller").Where(P => P.SellerId == seller.Id).ToList();
            return View(products);
        }
        #endregion

        #region Add Product
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Add(ProductViewModel P)
        {
            ViewData["Categories"] = storeContext.Categories.ToList();
            return View(P);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddProduct(ProductViewModel P)
        {
            if (ModelState.IsValid)
            {
                var currentSellerId = (await userManager.GetUserAsync(User)).Id;
                await storeContext.Products.AddAsync(new Product()
                {
                    Name = P.Name,
                    Price = P.Price,
                    Quantity = P.Quantity,
                    Description = P.Description,
                    SellerId = currentSellerId,
                    CategoryId = P.CategoryId,
                    AddedOn = DateTime.Now
                });

                await storeContext.SaveChangesAsync();
                var product = await storeContext.Products.FirstOrDefaultAsync(Pd => Pd.Name == P.Name && Pd.SellerId == currentSellerId);

                if (P.Picture is not null)
                {
                    var url = await FileManager.UploadImageAsync(environment, P.Picture, "Products", $"P{product.Id}_S{currentSellerId}");
                    if (url.Contains($"P{product.Id}_S{currentSellerId}"))
                    {
                        product.Picture = url;
                    }
                    else
                    {
                        product.Picture = "/files/images/Products/default.png";
                    }
                }
                else
                {
                    product.Picture = "/files/images/Products/default.png";
                }
                storeContext.Products.Update(product);
                await storeContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View("AddProduct", P);
        }
        #endregion

        #region Edit Product
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Edit(int Id)
        {
            ViewData["Categories"] = storeContext.Categories.ToList();
            Product P = storeContext.Products.Find(Id);
            if (P == null)
                return RedirectToAction("Index");
            var Pr = new ProductViewModel()
            {
                Id = P.Id,
                Name = P.Name,
                Price = P.Price,
                Quantity = P.Quantity,
                Description = P.Description,
                CategoryId = P.CategoryId,
                SellerId = P.SellerId,
                AddedOn = P.AddedOn
            };
            return View(Pr);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> EditProduct(int Id, ProductViewModel P)
        {
            if (ModelState.IsValid)
            {
                var currentSellerId = (await userManager.GetUserAsync(User)).Id;
                var product = await storeContext.Products.FirstOrDefaultAsync(Pd => Pd.Id == Id && Pd.SellerId == currentSellerId);

                product.Name = P.Name;
                product.Price = P.Price;
                product.Quantity = P.Quantity;
                product.Description = P.Description;
                product.CategoryId = P.CategoryId;

                if (P.Picture is not null)
                {
                    var url = await FileManager.UploadImageAsync(environment, P.Picture, "Products", $"P{product.Id}_S{currentSellerId}");
                    if (url.Contains($"P{product.Id}_S{currentSellerId}"))
                    {
                        product.Picture = url;
                    }
                }
                storeContext.Products.Update(product);
                await storeContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View("AddProduct", P);
        }
        #endregion

        #region Delete Product
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Delete(int Id)
        {
            Product product = storeContext.Products.Find(Id);
            if (product is null)
                return RedirectToAction("Index");
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            Product product = storeContext.Products.Find(Id);
            if (product is null)
                return RedirectToAction("Index");

            storeContext.Remove(product);
            await storeContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Add to Cart

        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddToCart(int Id)
        {
            var account = await userManager.GetUserAsync(User);
            var user = (AppUser)await userManager.FindByEmailAsync(account.Email);
            if (user is null)
                return NotFound();

            var product = await storeContext.Products.FirstOrDefaultAsync(P => P.Id == Id);
            if (product is null)
                return NotFound();

            if (user.Orders is not null)
            {
                var OnCarts = storeContext.Orders.Include("OrderProducts").ToList();//.FirstOrDefault(O => O.Status == OrderStatus.OnCart && O.UserId == user.Id);
                var OnCart = OnCarts.FirstOrDefault(O => O.GetStatus() == OrderStatus.OnCart && O.UserId == user.Id);
                if (OnCart is null)
                {
                    OnCart = new Order() { UserId = user.Id };
                    storeContext.Orders.Add(OnCart);
                    await storeContext.SaveChangesAsync();
                }
                var OrderProduct = storeContext.OrderProducts.FirstOrDefault(OP => OP.ProductId == product.Id && OP.OrderId == OnCart.Id);
                if (OrderProduct is not null)
                {
                    OrderProduct.Quantity++;
                    storeContext.Update(OrderProduct);
                }
                else
                {
                    OrderProduct = new OrderProduct()
                    {
                        OrderId = OnCart.Id,
                        ProductId = product.Id,
                        Quantity = 1
                    };
                    storeContext.OrderProducts.Add(OrderProduct);
                }
                await storeContext.SaveChangesAsync();
            }
            TempData["added"] = "Product Added Successfully";
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Show Cart

        [Authorize(Roles = "User")]
        public async Task<IActionResult> ShowCart()
        {
            var user = (AppUser)await userManager.GetUserAsync(User);

            var onCartOrder = storeContext.Orders.Include("OrderProducts.Product.Category").Include("OrderProducts.Product.Seller").ToList().FirstOrDefault(O => O.GetStatus() == OrderStatus.OnCart && O.UserId == user.Id);

            return View(onCartOrder);
        }

        #endregion

        #region Update Cart

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> UpdateCart(Order cart)
        {
            var user = (AppUser)await userManager.GetUserAsync(User);
            var OnCart = storeContext.Orders.Include("OrderProducts.Product.Category").Include("OrderProducts.Product.Seller").FirstOrDefault(O => O.GetStatus() == OrderStatus.OnCart && O.UserId == user.Id);

            OnCart.OrderProducts = cart.OrderProducts;
            storeContext.Update(OnCart);
            await storeContext.SaveChangesAsync();

            return RedirectToAction("ShowCart");
        }

        #endregion

        #region Increase and Decrease Product from Cart

        [Authorize(Roles = "User")]
        public async Task<IActionResult> DecreaseQ(int Id)
        {
            var user = (AppUser)await userManager.GetUserAsync(User);
            if (user is null)
                return RedirectToAction("ShowCart");

            var OnCart = storeContext.Orders.Include("OrderProducts").ToList().FirstOrDefault(O => O.GetStatus() == OrderStatus.OnCart && O.UserId == user.Id);
            if (OnCart is null)
                return RedirectToAction("ShowCart");

            var orderProduct = storeContext.OrderProducts.FirstOrDefault(OP => OP.ProductId == Id && OP.OrderId == OnCart.Id);
            if (orderProduct is null)
                return RedirectToAction("ShowCart");

            if (orderProduct.Quantity > 1)
            {
                orderProduct.Quantity--;
                storeContext.Update(orderProduct);
            }
            else
            {
                storeContext.Remove(orderProduct);
            }
            storeContext.SaveChanges();

            return RedirectToAction("ShowCart");
        }


        [Authorize(Roles = "User")]
        public async Task<IActionResult> IncreaseQ(int Id)
        {
            var user = (AppUser)await userManager.GetUserAsync(User);
            if (user is null)
                return RedirectToAction("ShowCart");

            var OnCart = storeContext.Orders.Include("OrderProducts").ToList().FirstOrDefault(O => O.GetStatus() == OrderStatus.OnCart && O.UserId == user.Id);
            if (OnCart is null)
                return RedirectToAction("ShowCart");

            var orderProduct = storeContext.OrderProducts.FirstOrDefault(OP => OP.ProductId == Id && OP.OrderId == OnCart.Id);
            if (orderProduct is null)
                return RedirectToAction("ShowCart");

            orderProduct.Quantity++;
            storeContext.Update(orderProduct);
            storeContext.SaveChanges();

            return RedirectToAction("ShowCart");
        }
        #endregion

        #region Confirm Order

        [Authorize(Roles = "User")]
        public async Task<IActionResult> ConfirmOrder()
        {
            var user = (AppUser)await userManager.GetUserAsync(User);
            if (user is null)
                return RedirectToAction("ShowCart");

            var cart = storeContext.Orders.Include("OrderProducts").ToList().FirstOrDefault(O => O.GetStatus() == OrderStatus.OnCart && O.UserId == user.Id);
            if (cart is null)
                return RedirectToAction("ShowCart");

            if (cart.OrderProducts.Count == 0)
            {
                storeContext.Remove(cart);
                await storeContext.SaveChangesAsync();
                return RedirectToAction("ShowCart");
            }

            var orderProducts = cart.OrderProducts.ToList();
            for (int i = 0; i < orderProducts.Count; i++)
            {
                orderProducts[i].Status = OrderStatus.Purchased;
            }

            storeContext.Update(cart);
            await storeContext.SaveChangesAsync();
            TempData["added"] = "Order Confirmed Successfully";
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Manage Orders by Sellers

        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ManageOrders()
        {
            var seller = (Seller)await userManager.GetUserAsync(User);
            if (seller is null)
                return NotFound();

            var orders = storeContext.OrderProducts
                .Include("Product.Category")
                .Include("Product.Seller")
                .Include("Order.User")
                .Where(OP => OP.Product.SellerId == seller.Id
                   && (OP.Status == OrderStatus.Purchased || OP.Status == OrderStatus.Shipped))
                .ToList();

            return View(orders);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> ShipProduct(ConfirmOrderProductDto model)
        {
            var seller = (Seller)await userManager.GetUserAsync(User);
            if (seller is null)
                return NotFound();

            var productOrder = storeContext.OrderProducts.FirstOrDefault(OP => OP.OrderId == model.OrderId
                                                                        && OP.Product.SellerId == seller.Id
                                                                        && OP.ProductId == model.ProductId
                                                                        && OP.Status == OrderStatus.Purchased);

            if (productOrder is null)
                ViewData["response"] = "the order of this product was not found!";
            else
            {
                ViewData["confirmed"] = "the order of this product was confirmed!";
                productOrder.Status = OrderStatus.Shipped;
                var product = storeContext.Products.FirstOrDefault(P => P.Id == model.ProductId);
                if (product is null)
                    ViewData["response"] = "the product was not found!";
                else if (product.Quantity < productOrder.Quantity)
                    ViewData["response"] = "the product quantity is less than the order quantity!";
                else
                {
                    product.Quantity -= productOrder.Quantity;
                    storeContext.Update(product);
                    storeContext.Update(productOrder);
                    await storeContext.SaveChangesAsync();
                }
            }

            return RedirectToAction("ManageOrders");
        }

        #endregion

        #region Manage Orders by User
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyOrders()
        {
            var user = (AppUser)await userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

            var orders = storeContext.OrderProducts
                .Include("Product.Category")
                .Include("Product.Seller")
                .Include("Order.User")
                .Where(OP => OP.Order.UserId == user.Id
                   && (OP.Status == OrderStatus.Purchased || OP.Status == OrderStatus.Shipped))
                .ToList();

            return View(orders);
        }
        #endregion
    }
}
