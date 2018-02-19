using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Svper
{
    public class IndexModel : PageModel
    {       
        public void onGet(string error, string errorDescription) 
        {
            // Console.WriteLine("OH MY GOD");
        }
        public void OnGet()
        {
        }
    }
}