﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sicsoft.Checkin.Web
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
      
        public void OnGet()
        {

        }


        public  async Task<IActionResult> OnPost(string returnUrl = null)
        {
            
            await HttpContext.SignOutAsync();
            //_logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                 

                return Redirect("./Index");
            }
        }
    }
}