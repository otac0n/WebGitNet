using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGitNet.Models
{
    public class PaginationInfo
    {
        private readonly int page;
        private readonly int pageCount;
        private readonly string controllerName;
        private readonly string actionName;
        private readonly object routeValues;
        private readonly string routeKey;

        public PaginationInfo(int page, int pageCount, string controllerName, string actionName, object routeValues, string routeKey = "page")
        {
            this.page = page;
            this.pageCount = pageCount;
            this.controllerName = controllerName;
            this.actionName = actionName;
            this.routeValues = routeValues;
            this.routeKey = routeKey;
        }

        public int Page
        {
            get { return this.page; }
        }

        public int PageCount
        {
            get { return this.pageCount; }
        }

        public string ControllerName
        {
            get { return this.controllerName; }
        }

        public string ActionName
        {
            get { return this.actionName; }
        }

        public object RouteValues
        {
            get { return this.routeValues; }
        }

        public string RouteKey
        {
            get { return this.routeKey; }
        }
    }
}