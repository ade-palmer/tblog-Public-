using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TBlog.Core.Entities;
using TBlog.Core.Models.ViewModels;
using TBlog.Core.Services;

namespace TBlog.Core.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View(); 
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, true);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl ?? "/");
                }

                if (result.IsLockedOut)
                {
                    TempData["StatusMessage"] = "Error: Your account is locked out. Please try again in 5 minutes or use the 'Forgot your Password' option to choose another password";
                }
                else
                {
                    TempData["StatusMessage"] = "Error: Please ensure your username and password are correct and that you have verified your account via the confirmation email";
                }
            }

            return View(loginViewModel);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            var user = new ApplicationUser
            {
                UserName = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                Email = registerViewModel.Email,
                EnrollmentDateTime = registerViewModel.Enrollment
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(registerViewModel.Email, "Welcome to T-Blog",
                        //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                        $"<p>Hi {registerViewModel.FirstName}</p>" +
                        $"<p>Thank you for taking an interest in our site and deciding to register. Once registration is complete you will be able to add comments to any of the posts.</p>" +
                        $"<p>Click on the Activate Now! button to be redirected to your account login page.</p>",
                        $"<table border=\"0\" cellspacing=\"0\" cellpadding=\"10\" style=\"border:2px solid #3b464e;\"><tr><td align=\"center\" style=\"color:#3b464e; font-size:16px; font-family: Arial, sans-serif;\">" +
                        $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style=\"color:#3b464e; text-decoration:none; font-size:16px; font-family: Arial, sans-serif;\">ACTIVATE NOW!</a>" +
                        $"</td></tr></table>");

                TempData["StatusMessage"] = "Verification email sent. Please check your email and complete the verification before logging in.";

                return RedirectToAction("Login", "Account");

                //await _signInManager.SignInAsync(user, isPersistent: false);
                //return RedirectToAction("Index", "Blog");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("index", "Blog");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Thank you for validating your account. You can now log in.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }
        }


        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                TempData["StatusMessage"] = "Please check your email and complete the password reset process before logging in.";
                return RedirectToAction("Login", "Account");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { code = code }, protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(forgotPasswordViewModel.Email, "T-Blog password reset",
                        $"<p>You recently requested to reset your password for the T-Blog website. Click the button below to reset it.</p>" +
                        $"<p>If you did not request a password reset, please ignore this email or reply to let us know. This password reset is only valid for the next 24 hours.</p>",
                        $"<table border=\"0\" cellspacing=\"0\" cellpadding=\"10\" style=\"border:2px solid #3b464e;\"><tr><td align=\"center\" style=\"color:#3b464e; font-size:16px; font-family: Arial, sans-serif;\">" +
                        $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style=\"color:#3b464e; text-decoration:none; font-size:16px; font-family: Arial, sans-serif;\">RESET PASSWORD</a>" +
                        $"</td></tr></table>");

            TempData["StatusMessage"] = "Please check your email and complete the password reset process before logging in.";
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                var passwordResetModel = new ResetPasswordViewModel
                {
                    Code = code
                };
                return View(passwordResetModel);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("Login", "Account");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Code, resetPasswordViewModel.Password);
            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Your password has been reset and you can log in.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Blog");
        }
    }
}
