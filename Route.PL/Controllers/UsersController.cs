using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Route.DAL.Entities;
using System.Data;

namespace Route.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<IActionResult> Index(string searchValue = "")
        {
            List<ApplicationUser> users;
            if (string.IsNullOrEmpty(searchValue))
                users = await _userManager.Users.ToListAsync();
            else
                users = await _userManager.Users
                                          .Where(user => user.Email.Trim().ToLower().Contains(searchValue.Trim().ToLower()))
                                          .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if(id is null)
                return NotFound();
            
            var user = await _userManager.FindByIdAsync(id);

            if(user is null) 
                return NotFound();

            return View(viewName, user);
        }

        public async Task<IActionResult> Update(string id)
        {
            return await Details(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(string id, ApplicationUser applicationUser)
        {
            if (id is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    user.UserName = applicationUser.UserName;
                    user.NormalizedUserName = applicationUser.UserName.ToUpper();

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                        return RedirectToAction("Index");

                    foreach (var item in result.Errors)
                        ModelState.AddModelError("", item.Description);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            return View(applicationUser);
        }
        public async Task<IActionResult> Delete(string id, ApplicationUser applicationUser)
        {
            if (id is null)
                return NotFound();

            try
            {
                var user = await _userManager.FindByIdAsync(id);

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var item in result.Errors)
                    ModelState.AddModelError("", item.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
            return View(applicationUser);
        }

    }
}
