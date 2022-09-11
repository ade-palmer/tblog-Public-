using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TBlog.Core.Entities;
using TBlog.Core.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBlog.Core.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public async Task<IActionResult> AccountDetails()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userResult = new UserProfileViewModel()
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return View(userResult);
        }


        [HttpPost]
        public async Task<IActionResult> AccountDetails(UserProfileViewModel userProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            //TODO: Do i need to allow the option to reset the username (email address) as well and force logout

            var user = await _userManager.GetUserAsync(User);
            if (userProfileViewModel.Email != user.Email) user.Email = userProfileViewModel.Email;
            if (userProfileViewModel.FirstName != user.FirstName) user.FirstName = userProfileViewModel.FirstName;
            if (userProfileViewModel.LastName != user.LastName) user.LastName = userProfileViewModel.LastName;

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (updateUserResult.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);

                TempData["StatusMessage"] = "Your profile has been updated.";
                return RedirectToAction("AccountDetails");
            }

            // Captures if Email already in use
            foreach (var error in updateUserResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword);

            if (changePasswordResult.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user); // Is this required as we are forcing a new log in.
                await _signInManager.SignOutAsync();
                TempData["StatusMessage"] = "Your password has been changed. Please log in with your new password";
                
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
    }
}
