using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Svper
{
    [AllowAnonymous]
    public class AuthModel : PageModel
    {
        public void OnGet() {

        }
        public async void OnGetSignoutAsync()
        {
            await HttpContext.SignOutAsync(User.FindFirst("tfp").Value);
        }
    }
}