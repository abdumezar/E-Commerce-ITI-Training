using E_Commerce_ITI_Final_Project.Entities;
using E_Commerce_ITI_Final_Project.Helpers;
using E_Commerce_ITI_Final_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_ITI_Final_Project.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<Account> userManager;
        private readonly SignInManager<Account> signInManager;
        private readonly IWebHostEnvironment environment;

        public AccountController(UserManager<Account> userManager, SignInManager<Account> signInManager, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.environment = environment;
        }

        #region Register
        public async Task<IActionResult> Register(RegisterViewModel R)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is not null)
                return Redirect("/Home/Index");
            return View(R);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterForm(RegisterViewModel R)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result;

                if (R.isSeller)
                {
                    var account = new Seller
                    {
                        FName = R.FName,
                        LName = R.LName,
                        UserName = R.Email.Split('@')[0],
                        Email = R.Email,
                        Address = R.Address,
                        PhoneNumber = R.PhoneNumber,
                        Gender = R.Gender
                    };
                    result = await userManager.CreateAsync(account, R.Password);
                }
                else
                {
                    var account = new AppUser
                    {
                        FName = R.FName,
                        LName = R.LName,
                        UserName = R.Email.Split('@')[0],
                        Email = R.Email,
                        Address = R.Address,
                        PhoneNumber = R.PhoneNumber,
                        Gender = R.Gender
                    };
                    result = await userManager.CreateAsync(account, R.Password);
                }

                if (result.Succeeded)
                {
                    if (R.isSeller)
                        await userManager.AddToRoleAsync(await userManager.FindByEmailAsync(R.Email), "Seller");
                    else
                        await userManager.AddToRoleAsync(await userManager.FindByEmailAsync(R.Email), "User");
                    return RedirectToAction("Login");
                }

                foreach (var item in result.Errors)
                    ModelState.AddModelError(string.Empty, item.Description);

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Register", R);
        }
        #endregion

        #region Login
        public async Task<IActionResult> Login(LoginViewModel L)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is not null)
            {
                var role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                if (role == "Seller")
                    return Redirect("/Products/Index");
                else
                    return Redirect("/Home/Index");
            }
            return View(L);
        }

        [HttpPost]
        public async Task<IActionResult> LoginForm(LoginViewModel L)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(L.Email);
                if (user is not null)
                {
                    var flag = await userManager.CheckPasswordAsync(user, L.Password);
                    if (flag)
                    {
                        await signInManager.PasswordSignInAsync(user, L.Password, L.RememberMe, false);
                        var role = userManager.GetRolesAsync(user).Result.FirstOrDefault();

                        if (role == "Seller")
                            return Redirect("/Products/Index");
                        else
                            return Redirect("/Home/Index");
                    }
                    ViewData["errors"] = "Invalid Password!";
                    return View("Login", L);
                }
                ViewData["errors"] = "Email is not exist!";
                return View("Login", L);
            }
            return View("Login", L);
        }
        #endregion

        #region Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region Profile

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            return View(user);
        }

        #region Update Profile

        [Authorize]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await userManager.GetUserAsync(User);

            var mapped = new RegisterViewModel()
            {
                FName = user.FName,
                LName = user.LName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Gender = user.Gender,
                isSeller = await userManager.IsInRoleAsync(user, "Seller")
            };

            return View(mapped);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(RegisterViewModel U)
        {
            var user = await userManager.GetUserAsync(User);
            user.FName = U.FName;
            user.LName = U.LName;
            user.Address = U.Address;
            user.PhoneNumber = U.PhoneNumber;
            user.Email = U.Email;
            user.Gender = U.Gender;

            if (U.ProfilePic is not null)
            {
                var url = await FileManager.UploadImageAsync(environment, U.ProfilePic, "ProfilePics", $"ProfilePic_{user.Id}");
                if (url.Contains($"ProfilePic_{user.Id}"))
                    user.ProfilePicture = url;
            }
            else
            {
                user.ProfilePicture = "";
            }
            await userManager.UpdateAsync(user);
            await signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Profile", user);
        }

        #endregion

        #endregion

        #region Helpers

        //private async Task<string> UploadImageAsync(IFormFile file, string FileName)
        //{
        //    try
        //    {
        //        // Check if the file is not null and the content length is greater than 0
        //        if (file != null && file.Length > 0)
        //        {
        //            // Validate the file type
        //            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        //            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        //            if (!allowedExtensions.Contains(fileExtension))
        //            {
        //                return "Only image files (jpg, jpeg, png, gif) are allowed.";
        //            }

        //            // Validate file size (e.g., limit to 5MB)
        //            var maxFileSize = 5 * 1024 * 1024; // 5MB
        //            if (file.Length > maxFileSize)
        //            {
        //                return "The image file size exceeds the allowed limit.";
        //            }

        //            // Create a more meaningful filename using the original filename and timestamp;
        //            var fileName = $"{FileName}.{Path.GetFileName(file.ContentType)}";

        //            // Determine the storage path (you can use configuration settings for this)
        //            var storagePath = Path.Combine(environment.WebRootPath, "files", "images", fileName);

        //            // if image exists [replace it]
        //            var imagesPath = Path.Combine(environment.WebRootPath, "files", "images");
        //            var imageNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        //            string[] filesToDelete = Directory.GetFiles(imagesPath, $"{imageNameWithoutExtension}.*");

        //            foreach (string filePath in filesToDelete)
        //            {
        //                System.IO.File.Delete(filePath);
        //            }

        //            // If you choose cloud-based storage, use the respective SDK to upload the file here
        //            // Save the file to the specified path
        //            using (var fs = new FileStream(storagePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(fs);
        //            }

        //            //Process the image(e.g., resize the image)
        //            using (var image = Image.Load(storagePath))
        //            {
        //                // Resize the image to a specific width while preserving the aspect ratio
        //                var maxWidth = 800;
        //                if (image.Width > maxWidth)
        //                {
        //                    var scaleFactor = (double)maxWidth / image.Width;
        //                    var newHeight = (int)(image.Height * scaleFactor);
        //                    image.Mutate(x => x.Resize(maxWidth, newHeight));
        //                }

        //                // Save the processed image back to the same file path
        //                image.Save(storagePath);
        //            }

        //            // Return the URL of the processed image
        //            var imageUrl = Url.Content($"~/files/images/{fileName}");

        //            return imageUrl;
        //        }

        //        return "No file uploaded";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error uploading the file";
        //    }
        //}

        #endregion
    }
}
