﻿namespace AutoApi.Rest.EntityFramework.RequestManagement;

public class PaginationSettings
{
    public int DefaultPageSize { get; set; } = 20;
    public int MaxPageSize { get; set; } = 50;
}
