﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Route.DAL.Entities;
using Route.PL.Helper;
using Route.PL.Models;

namespace Route.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
			_signInManager = signInManager;
		}
        #region SIgnUp
        public IActionResult SignUp()
        {
            return View(new RegisterVM());
        }
        [HttpPost]
        public async Task<IActionResult> SignUpAsync(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    Email = registerVM.Email,
                    UserName = registerVM.Email.Split('@')[0],
                    IsAgree = registerVM.IsAgree,

                };
                var result = await _userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                    return RedirectToAction("SignIn");
                 
                foreach (var item in result.Errors)
                    ModelState.AddModelError("", item.Description);
            }
            return View(registerVM);
        }
		#endregion

		#region SignIn
		public async Task<IActionResult> SignIn()
		{
			return View(new LoginVM());
		}
        [HttpPost]
		public async Task<IActionResult> SignIn(LoginVM loginVM)
		{
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginVM.Email);

                if (user == null)
                    ModelState.AddModelError("", "Email Not Found");

                var passwordIsCorrect = await _userManager.CheckPasswordAsync(user, loginVM.Password);

                if (passwordIsCorrect)
                {
                    var loginSucceeded = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, false);
                    if(loginSucceeded.Succeeded)
                        return RedirectToAction("Index", "Home");
                }

                
            }
			return View(loginVM);
		}
        #endregion

        #region SignOut
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }
        #endregion

        #region ResetPassword

        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordVM());
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPasswordLink = Url.Action("ResetPassword", "Account", new { Email = model.Email, Token = token }, Request.Scheme);

                    var email = new Email()
                    {
                        Title = "Reset Password",
                        Body = resetPasswordLink,
                        To = model.Email
                    };

                    EmailSettings.SendEmail(email);
                    return RedirectToAction("CompleteForgotPassword");
                }
                ModelState.AddModelError("", "Invalid Email");
            }
            return View(model);
        }

        public IActionResult CompleteForgotPassword()
        {
            return View();
        }
        #endregion


        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordVM());
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user,model.Token, model.Password);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(SignIn));

                    foreach (var item in result.Errors)
                        ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }

    }
}
