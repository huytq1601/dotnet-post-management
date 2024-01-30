namespace PostManagement.Core.Common
{
    public class PostFilter : PaginationFilter
    {
        public string? Tag { get; set; }

        public PostFilter() : base()
        {
            Tag = null;
        }
        public PostFilter(string tag, int pageNumber, int pageSize) : base(pageNumber, pageSize)
        {
            Tag = tag;
        }

    }
}
