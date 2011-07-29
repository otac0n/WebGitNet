//-----------------------------------------------------------------------
// <copyright file="FileController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.Web.Mvc;

    public class FileController : Controller
    {
        public ActionResult Fetch(string url)
        {
            ViewBag.Location = url ?? string.Empty;
            return View("List");
        }
    }
}
