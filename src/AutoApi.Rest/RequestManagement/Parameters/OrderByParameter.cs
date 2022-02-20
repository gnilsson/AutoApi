//using System.ComponentModel;
//using AutoApi.Defined.Descriptive;

//namespace AutoApi.Rest.RequestManagement.Parameters;

//public class OrderByParameter
//{
//    public ListSortDirection SortDirection { get; }
//    public string Node { get; }
//    public OrderByParameter(string[] orderQuery)
//    {
//        SortDirection = orderQuery[0].Contains(
//            OrderByParameterDescription.Ascending, StringComparison.OrdinalIgnoreCase)
//            ? ListSortDirection.Ascending
//            : ListSortDirection.Descending;
//        Node = orderQuery[1];
//    }
//}
