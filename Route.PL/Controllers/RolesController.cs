using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Route.DAL.Entities;
using Route.PL.Models;

namespace Route.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public async Task<IActionResult> Create()
        {
            return View(new ApplicationRole());
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRole role)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(role);

                if(result.Succeeded)
                    return RedirectToAction("Index");
                
                foreach (var item in result.Errors)
                    ModelState.AddModelError("", item.Description);
            }
            return View(role);
        }


        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return NotFound();

            var role = await _roleManager.FindByIdAsync(id);

            if (role is null)
                return NotFound();

            return View(viewName, role);
        }

        public async Task<IActionResult> Update(string id)
        {
            return await Details(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(string id, ApplicationRole applicationRole)
        {
            if (id is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(id);
                    role.Name = applicationRole.Name;
                    role.NormalizedName = applicationRole.Name.ToUpper();

                    var result = await _roleManager.UpdateAsync(role);

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
            return View(applicationRole);
        }
        public async Task<IActionResult> Delete(string id, ApplicationRole applicationRole)
        {
            if (id is null)
                return NotFound();

            try
            {
                var role = await _roleManager.FindByIdAsync(id);

                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var item in result.Errors)
                    ModelState.AddModelError("", item.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return View(applicationRole);
        }

        public async Task<IActionResult> AddOrRemoveUsers(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if(role is null)
                return BadRequest();

            ViewBag.RoleId = roleId;

            var users = new List<UserInRoleVM>();

            foreach (var item in await _userManager.Users.ToListAsync())
            {
                var user = new UserInRoleVM
                {
                    UserId = item.Id,
                    UserName = item.UserName
                };
                if (await _userManager.IsInRoleAsync(item, role.Name))
                    user.isSelected = true;
                else
                    user.isSelected = false;

                users.Add(user);
            }

            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(List<UserInRoleVM> users, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null)
                return BadRequest();

            if (ModelState.IsValid)
            {
                foreach (var item in users)
                {
                    var user = await _userManager.FindByIdAsync(item.UserId);
                    if(user != null)
                    {
                        if(item.isSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                            await _userManager.AddToRoleAsync(user, role.Name);
                        else if((!item.isSelected) && (await _userManager.IsInRoleAsync(user, role.Name)))
                            await _userManager.RemoveFromRoleAsync(user, role.Name);

                    }
                }
                return RedirectToAction(nameof(Update), new { id = roleId });
            }
            return View(users);
        }
    }
}
