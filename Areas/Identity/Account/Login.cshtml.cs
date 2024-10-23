using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FriendsCompatibilityGame.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // التحقق من دور المستخدم
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        // توجيه المستخدمين العاديين إلى صفحة الأسئلة
                        if (roles.Contains("Player"))
                        {
                            return RedirectToAction("Index", "UserAnswers"); // توجيه اللاعب إلى صفحة الإجابات
                        }
                        else if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "Dashboard"); // توجيه المسؤول إلى لوحة التحكم
                        }
                    }
                }
                ModelState.AddModelError(string.Empty, "فشل تسجيل الدخول.");
            }
            return Page();
        }
    }
}
