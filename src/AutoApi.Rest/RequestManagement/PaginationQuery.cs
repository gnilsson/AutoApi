using AutoApi.Rest.Shared.Requests;

namespace AutoApi.Rest.RequestManagement;

//public class PaginationSettings
//{
//    public int DefaultPageSize { get; set; } = 20;
//    public int MaxPageSize { get; set; } = 50;
//}

//public class PaginationQuery : IPaginateable
//{
//    public PaginationQuery()
//    { }

//    public PaginationQuery(IPaginateable pagination, PaginationSettings settings)
//    {
//        PageNumber = pagination?.PageNumber < 1
//            ? _defaultPageNumber
//            : pagination!.PageNumber;

//        PageSize = pagination.PageSize < 1
//            ? settings.DefaultPageSize
//            : pagination.PageSize > settings.MaxPageSize ? settings.MaxPageSize
//            : pagination.PageSize;

//        IsApplied = !(pagination.PageSize < 1 && pagination.PageNumber < 1);
//    }

//    private readonly int _defaultPageNumber = 1;

//    public int PageNumber { get; set; }

//    public int PageSize { get; set; }

//    public bool IsApplied { get; }

//    //public bool ExcludeTotal { get; set; }
//}
