using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Security.Principal;



namespace KrpanoCMS.Business
{
    public static class UserIdentityq
    {
        //public static string GetUserId()
        //{
        //    var claimsIdentity = User.Identity as ClaimsIdentity;
            
        //    if (claimsIdentity != null)
        //    {
        //        // the principal identity is a claims identity.
        //        // now we need to find the NameIdentifier claim
        //        var userIdClaim = claimsIdentity.Claims
        //            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        //        if (userIdClaim != null)
        //        {
        //            var userIdValue = userIdClaim.Value;
        //        }
        //    }


        //    return claimsIdentity.GetUserId();
        //}

    }
}