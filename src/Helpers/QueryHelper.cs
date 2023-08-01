using workflow.Models.DataTableViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Data;

namespace api.Helpers
{
    public static class QueryHelper
    {
        public static IQueryable<T> SortOrder<T>(IQueryable<T> query, PagingRequest paging)
        {
            var param = Expression.Parameter(typeof(T));

            // get the sortable column index if any
            var colOrder = paging.Order[0];

            // get the sortable column name
            var field = paging.Columns[colOrder.Column];

            // get property type of sortable column
            var sortby = typeof(T).GetProperty(field.Name);

            if (sortby != null)
            {
                // generate the member access expression
                var expr = Expression.Lambda<Func<T, object>>(
                    Expression.Convert(Expression.Property(param, sortby), typeof(object)), param
                );

                // generate the column order according to selection
                switch (colOrder.Dir)
                {
                    case "asc":
                        query = query.OrderBy(expr);
                        break;
                    case "desc":
                        query = query.OrderByDescending(expr);
                        break;
                }
            }

            return query;
        }

        public static PagingResponse<T> Paginate<T>(IQueryable<T> query, PagingRequest paging)
        {
            var pagingResponse = new PagingResponse<T>()
            {
                Draw = paging.Draw
            };

            var recordsTotal = query.Count();
            pagingResponse.Data = query.Skip(paging.Start).Take(paging.Length).ToArray();
            pagingResponse.RecordsTotal = recordsTotal;
            pagingResponse.RecordsFiltered = recordsTotal;

            return pagingResponse;
        }

        public static DataTable DataTableSortOrder(DataTable query, PagingRequest paging)
        {
            // get the sortable column index if any
            var colOrder = paging.Order[0];

            // get the sortable column name
            var field = paging.Columns[colOrder.Column];

            var queryToSort = query.DefaultView;

            queryToSort.Sort = field.Name + " " + colOrder.Dir;

            query = queryToSort.ToTable();

            return query;
        }

        public static DataTablePagingResponse DataTablePaginate(DataTable query, PagingRequest paging)
        {
            var pagingResponse = new DataTablePagingResponse()
            {
                Draw = paging.Draw
            };

            var recordsTotal = query.Rows.Count;

            if (query.Rows.Count > 0)
                pagingResponse.Data = query.Rows.Cast<DataRow>()
                                       .Skip(paging.Start)
                                       .Take(paging.Length)
                                       .ToArray()
                                       .CopyToDataTable();
            else
                pagingResponse.Data = query;

            pagingResponse.RecordsTotal = recordsTotal;
            pagingResponse.RecordsFiltered = recordsTotal;

            return pagingResponse;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }

}
