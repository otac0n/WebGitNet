//-----------------------------------------------------------------------
// <copyright file="IRouteRegisterer.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Routing;

    public interface IRouteRegisterer
    {
        void RegisterRoutes(RouteCollection routes);
    }
}
