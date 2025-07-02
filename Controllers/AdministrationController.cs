using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManger;

        public AdministrationController(RoleManager<IdentityRole> roleManger)
        {
            this.roleManger = roleManger;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {

            return View();
        }
        [HttpPost]
        public async Task <IActionResult>CreateRole(CreateRoleViewModel model )
        {
           if (ModelState.IsValid)
            {
               IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

            
                IdentityResult result = await roleManger.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("index","home");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
                        }
            return View();
        }
        public IActionResult EditRole()
        {

            return View();
        }
    }
}
