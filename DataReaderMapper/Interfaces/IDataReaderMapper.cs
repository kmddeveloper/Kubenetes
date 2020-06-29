using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace AG.ApiLibrary.DataReaderMapper
{
    public interface IDataReaderMapper
    {
        Dictionary<string, MapRule> CreateMap();
        Task<T> MapAsync<T>(SqlCommand sqlCmd, Dictionary<string, MapRule> map) where T : new();
        Task<T> MapAsync<T>(SqlCommand sqlCmd, Dictionary<string, MapRule> map, string connectionString) where T : new();
        Task<List<T>> MapToListAsync<T>(SqlCommand sqlCmd, Dictionary<string, MapRule> map) where T : new();
        Task<List<T>> MapToListAsync<T>(SqlCommand sqlCmd, Dictionary<string, MapRule> map, string connectionString) where T : new();
    }
}
