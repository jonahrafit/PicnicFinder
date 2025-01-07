using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PaginationModel
{
    public int CurrentPage { get; set; }
    public int TotalItems { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalPages { get; set; }

    public override string ToString()
    {
        return $"Page {CurrentPage} of {TotalPages}, Total Items: {TotalItems}, Items per Page: {ItemsPerPage}";
    }
}
