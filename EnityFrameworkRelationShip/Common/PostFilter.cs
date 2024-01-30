namespace EnityFrameworkRelationShip.Common
{
    public class PostFilter: PaginationFilter
    {
        public string? Tag { get; set; }

        public PostFilter(): base()
        {
            this.Tag = null;
        }
        public PostFilter(string tag, int pageNumber, int pageSize): base(pageNumber, pageSize) {
            Tag = tag;
        }
 
    }
}
