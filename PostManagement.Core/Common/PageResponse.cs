namespace PostManagement.Core.Common
{
    public class PageResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public PageResponse(T data, int pageNumber, int pageSize): base(data)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Data = data;
            Message = string.Empty;
            Success = true;
        }
    }
}
