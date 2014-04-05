namespace WebGitNet.Models
{
    public class PaginationInfo
    {
        private readonly string actionName;
        private readonly string controllerName;
        private readonly int page;
        private readonly int pageCount;
        private readonly string routeKey;
        private readonly object routeValues;

        public PaginationInfo(int page, int pageCount, string controllerName, string actionName, object routeValues, string routeKey = "page")
        {
            this.page = page;
            this.pageCount = pageCount;
            this.controllerName = controllerName;
            this.actionName = actionName;
            this.routeValues = routeValues;
            this.routeKey = routeKey;
        }

        public string ActionName
        {
            get { return this.actionName; }
        }

        public string ControllerName
        {
            get { return this.controllerName; }
        }

        public int Page
        {
            get { return this.page; }
        }

        public int PageCount
        {
            get { return this.pageCount; }
        }

        public string RouteKey
        {
            get { return this.routeKey; }
        }

        public object RouteValues
        {
            get { return this.routeValues; }
        }
    }
}