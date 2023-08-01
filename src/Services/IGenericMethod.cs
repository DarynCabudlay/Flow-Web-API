using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Services
{
    public interface IGenericMethod<T>
    {
        // Retrieve Single Record
        IQueryable<T> Find(object obj);
        // Retrieve all Record
        IQueryable<T> FindAll();
        // Add Records
        Task<T> Add(T data, bool saveIpPerSession = true, IDbContextTransaction transaction = null);
        // Update Records
        Task Update(T data, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null);
        // Delete Single Record
        Task Delete(object obj, bool saveIpPerSession = true, IDbContextTransaction transaction = null);
        // Delete Multiple Records
        Task DeleteAll(T model, bool saveIpPerSession = true, IDbContextTransaction transaction = null);
        Task<bool> IfExists(string toSearch, object entityPrimaryKey = null);
    }
}
