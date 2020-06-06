using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Faculty.Data;
using Faculty.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Faculty.Areas.Identity.Pages.Account
{
    //[AllowAnonymous]
    [Authorize(Policy = "RequireAdministratorRole")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Поле Email обязательно для заполнения.")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            //The {0} index is the display name of property, {1} is the MaximumLength, {2} is the MinimumLength
            [Required]
            [StringLength(100, ErrorMessage = "{0} должен иметь не менее {2} и не более {1} символов.", MinimumLength = 3)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтверждение пароля")]
            [Compare("Password", ErrorMessage = "Пароль и пароль подтверждения не совпадают.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Поле Роль обязательно для заполнения.")]
            [Display(Name = "Роль")]
            public string RoleId { get; set; }

            public int personid { get; set; }

            public string persontype { get; set; }
            public string personname { get; set; }
        }


        public async Task OnGetAsync(int personid, string persontype, string personname, string returnUrl = null)
        {
            Input = new InputModel()
            {
                personid = personid,
                persontype = persontype,
                personname = personname
            };
            var Roles = _roleManager.Roles.Select(r => new SelectListItem() { Text = r.Name, Value = r.Id }).ToList();
            ViewData["Roles"] = Roles;

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            Persion p;
            if (Input.persontype == "Employee")
            {
                p = _context.Employees.FirstOrDefault(e => e.Id == Input.personid);
                returnUrl = "/Employee";
            }
            else
            {
                p = _context.Students.FirstOrDefault(e => e.Id == Input.personid);
                returnUrl = "/Student";
            }


            var Roles = _roleManager.Roles.Select(r => new SelectListItem() { Text = r.Name, Value = r.Id }).ToList();
            ViewData["Roles"] = Roles;
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // обновить ссылку на пользователя
                    p.AccountId = user.Id;
                    _context.Update(p);
                    await _context.SaveChangesAsync();

                    //задать роль
                    var role = await _roleManager.FindByIdAsync(Input.RoleId);
                    await _userManager.AddToRoleAsync(user, role.Name);

                    _logger.LogInformation("Пользователь создал новую учетную запись с паролем.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
