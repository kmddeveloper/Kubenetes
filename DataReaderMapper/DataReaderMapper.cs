
//using FastMember;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using Kubernetes.AppConfiguration;

namespace AG.ApiLibrary.DataReaderMapper
{
    public class DataReaderMapper:IDataReaderMapper
    {
        readonly IAppSettingManager _appSettingManager;
      
        public DataReaderMapper(IAppSettingManager appSettingManager)
        {
            _appSettingManager = appSettingManager;
        }

        #region CreateMap
        public Dictionary<string, MapRule> CreateMap()
        {
            return null;
        }
        #endregion

        #region MapAsync
        public async Task<T> MapAsync<T>(SqlCommand sqlCmd, Dictionary<string, MapRule> map) where T : new()
        {
          
            string connectionString = _appSettingManager.SqlConnectionString;

            return await MapAsync<T>(sqlCmd, map, connectionString);

        }

        public async Task<T> MapAsync<T>(SqlCommand sqlCmd, Dictionary<string, MapRule> map, string connectionString) where T : new()
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("Invalid connectionString!");

            if (sqlCmd == null)
                throw new ArgumentNullException("Invalid SqlCommand!");

            if (map == null && map.Count <= 0)
                throw new ArgumentNullException("Invalid MapRule!");

            T data = new T();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlCmd.Connection = sqlConn;
                    using (sqlCmd)
                    {
                        await sqlConn.OpenAsync();
                        using (var reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            T t = new T();
                            var members = t.GetType().GetProperties();
                            var memberCached = new Dictionary<string, PropertyInfo>();

                            if (reader != null && await reader.ReadAsync())
                            {
                                try
                                {
                                    data = GetRecord<T>(reader, members, memberCached, map);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(String.Format("MapToAsync Error in reader: {0}", ex.Message), "Error");
                                }
                            }
                            if (reader != null && !reader.IsClosed)
                                reader.Close();
                            return data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("MapToAsync Error: {0}", ex.Message), "Error");
            }
            return data;
        }
        #endregion

        #region MapToListAsync
        public async Task<List<T>> MapToListAsync<T>(SqlCommand sqlCmd, Dictionary<string, MapRule> map, string connectionString) where T : new()
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("Invalid connectionString!");

            if (sqlCmd == null)
                throw new ArgumentNullException("Invalid SqlCommand!");

            if (map == null && map.Count <= 0)
                throw new ArgumentNullException("Invalid MapRule!");

            var results = new List<T>();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlCmd.Connection = sqlConn;
                    using (sqlCmd)
                    {

                        await sqlConn.OpenAsync();
                        using (var reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {

                            T t = new T();
                            var members = t.GetType().GetProperties();
                            var memberCached = new Dictionary<string, PropertyInfo>();


                            while (reader != null && await reader.ReadAsync())
                            {
                                try
                                {
                                    T data = GetRecord<T>(reader, members, memberCached, map);
                                    if (memberCached.Count > 0)
                                        results.Add(data);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(String.Format("MapToListAsync Error in reader: {0}", ex.Message), "Error");
                                }

                            }
                            if (reader != null && !reader.IsClosed)
                                reader.Close();
                            return results;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("MapToListAsync Error: {0}", ex.Message), "Error");
            }
            return results;
        }

        public async Task<List<T>> MapToListAsync<T>(SqlCommand sqlCmd, Dictionary<string, MapRule> map) where T : new()
        {

            string connectionString = _appSettingManager.SqlConnectionString;

            return await MapToListAsync<T>(sqlCmd, map, connectionString);
        }

        #endregion


        #region Private Methods

        #region IsNullableType
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        } 
        #endregion
        

        #region GetRecord
        private T GetRecord<T>(IDataReader reader, PropertyInfo[] members, Dictionary<string, PropertyInfo> memberCached, Dictionary<string, MapRule> map) where T : new()
        {
            var data = new T();
            if (reader != null)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        string fieldName = reader.GetName(i);

                        if (!String.IsNullOrEmpty(fieldName))
                        {
                            fieldName = fieldName.ToUpper();
                            if (map.ContainsKey(fieldName))
                            {
                                if (!memberCached.ContainsKey(fieldName))
                                {
                                    var member = members.FirstOrDefault(m => String.Equals(m.Name, map[fieldName].PropertyName, StringComparison.OrdinalIgnoreCase));
                                    if (member != null && !memberCached.ContainsKey(map[fieldName].PropertyName))
                                        memberCached.Add(map[fieldName].PropertyName, member);
                                }
                                if (memberCached.ContainsKey(map[fieldName].PropertyName))
                                {
                                    var content = reader.GetValue(i);
                                    var targetType = IsNullableType(memberCached[map[fieldName].PropertyName].PropertyType) ? Nullable.GetUnderlyingType(memberCached[map[fieldName].PropertyName].PropertyType) : memberCached[map[fieldName].PropertyName].PropertyType;

                                    try
                                    {
                                        if (map[fieldName].DataType != content.GetType())
                                            content = MapTo(content, memberCached[map[fieldName].PropertyName].PropertyType);

                                        if (map[fieldName].DataType != content.GetType())
                                            content = Convert.ChangeType(content, targetType);
                                    }
                                    catch
                                    {
                                        content = null;
                                    }

                                    if (content != null)
                                    {
                                        Type type = data.GetType();
                                        PropertyInfo property = type.GetProperty(memberCached[map[fieldName].PropertyName].Name);
                                        property.SetValue(data, content);
                                    }


                                }
                            }
                        }
                    }
                }
            }
            return data;
        }
        #endregion

        #region MapTo
        private object MapTo(object src, Type type)
        {
            if (type == typeof(string))
            {
                return src.ToString();
            }
            else if (type == typeof(int))
            {
                int result = 0;
                Int32.TryParse(src.ToString(), out result);
                return result;
            }
            else if (type == typeof(DateTime))
            {
                DateTime result;
                DateTime.TryParse(src.ToString(), out result);
                return result;
            }
            else if (type == typeof(bool))
            {
                bool result = false;
                Boolean.TryParse(src.ToString(), out result);
                return result;
            }
            else if (type == typeof(decimal))
            {
                decimal result = 0;
                Decimal.TryParse(src.ToString(), out result);
                return result;
            }
            else if (type == typeof(float))
            {
                float result = 0;
                float.TryParse(src.ToString(), out result);
                return result;
            }
            else if (type == typeof(long))
            {
                long result = 0;
                long.TryParse(src.ToString(), out result);
                return result;
            }
            else if (type == typeof(short))
            {
                short result = 0;
                short.TryParse(src.ToString(), out result);
                return result;
            }
            else if (type == typeof(uint))
            {
                uint result = 0;
                uint.TryParse(src.ToString(), out result);
                return result;
            }
            else if (type == typeof(double))
            {
                double result = 0;
                double.TryParse(src.ToString(), out result);
                return result;
            }
            else
            {
                return src.ToString();
            }
        } 
        #endregion

        #endregion



#if FASTMEMBER

        #region MapToAsync
        public async Task<T> MapAsync<T>(string cid, SqlCommand sqlCmd, Dictionary<string, MapRule> map) where T : new()
        {
            if (String.IsNullOrEmpty(cid))
                throw new ArgumentNullException("Invalid cid!");

            if (sqlCmd == null)
                throw new ArgumentNullException("Invalid SqlCommand!");

            if (map == null && map.Count <= 0)
                throw new ArgumentNullException("Invalid MapRule!");

            T data = new T();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(_apiSettingManager.GetConnectionString(cid)))
                {
                    sqlCmd.Connection = sqlConn;
                    using (sqlCmd)
                    {
                        sqlConn.Open();
                        using (IDataReader reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            TypeAccessor accessor = TypeAccessor.Create(typeof(T));
                            MemberSet members = accessor.GetMembers();
                            var memberCached = new Dictionary<string, Member>();

                            if (reader != null && reader.Read())
                            {
                                try
                                {
                                    data = GetRecord<T>(reader, accessor, members, memberCached, map);
                                }
                                catch (Exception ex)
                                {
                                    var msg = ex.Message;
                                }
                            }
                            if (reader != null && !reader.IsClosed)
                                reader.Close();
                            return data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("MapToAsync Error: {0}", ex.Message), "Error");
            }
            return data;
        }
        #endregion

        #region MapToListAsync
        public async Task<List<T>> MapToListAsync<T>(string cid, SqlCommand sqlCmd, Dictionary<string, MapRule> map) where T : new()
        {
            if (String.IsNullOrEmpty(cid))
                throw new ArgumentNullException("Invalid cid!");

            if (sqlCmd == null)
                throw new ArgumentNullException("Invalid SqlCommand!");

            if (map == null && map.Count <= 0)
                throw new ArgumentNullException("Invalid MapRule!");

            var results = new List<T>();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(_apiSettingManager.GetConnectionString(cid)))
                {
                    sqlCmd.Connection = sqlConn;
                    using (sqlCmd)
                    {

                        sqlConn.Open();
                        using (IDataReader reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {

                            TypeAccessor accessor = TypeAccessor.Create(typeof(T));
                            MemberSet members = accessor.GetMembers();
                            var memberCached = new Dictionary<string, Member>();

                            while (reader != null && reader.Read())
                            {
                                try
                                {
                                    T data = GetRecord<T>(reader, accessor, members, memberCached, map);
                                    if (memberCached.Count > 0)
                                        results.Add(data);
                                }
                                catch (Exception ex)
                                {
                                    var msg = ex.Message;
                                }

                            }
                            if (reader != null && !reader.IsClosed)
                                reader.Close();
                            return results;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("MapToListAsync Error: {0}", ex.Message), "Error");
            }
            return results;
        }
        #endregion


        #region GetRecord
        private T GetRecord<T>(IDataReader reader, TypeAccessor accessor, MemberSet members, Dictionary<string, Member> memberCached, Dictionary<string, MapRule> map) where T : new()
        {
            var data = new T();
            if (reader != null)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        string fieldName = reader.GetName(i);

                        if (!String.IsNullOrEmpty(fieldName))
                        {
                            fieldName = fieldName.ToUpper();
                            if (map.ContainsKey(fieldName))
                            {
                                if (!memberCached.ContainsKey(fieldName))
                                {
                                    var member = members.FirstOrDefault(m => String.Equals(m.Name, map[fieldName].PropertyName, StringComparison.OrdinalIgnoreCase));
                                    if (member != null && !memberCached.ContainsKey(map[fieldName].PropertyName))
                                        memberCached.Add(map[fieldName].PropertyName, member);
                                }
                                if (memberCached.ContainsKey(map[fieldName].PropertyName))
                                {
                                    var content = reader.GetValue(i);
                                    if (map[fieldName].DataType != content.GetType())
                                        content = MapTo(content);


                                    //Type type = data.GetType();
                                    //PropertyInfo property = type.GetProperty(memberCached[map[fieldName].PropertyName].Name);
                                    //property.SetValue(data, content);

                                    accessor[data, memberCached[map[fieldName].PropertyName].Name] = content;
                                }
                            }
                        }
                    }
                }
            }
            return data;
        }
        #endregion


        #region InCompleted
        public async Task<List<T>> GetDataListAsync<T>(string cid, SqlCommand sqlCmd) where T : new()
        {
            var results = new List<T>();
            TypeAccessor accessor = TypeAccessor.Create(typeof(T));
            MemberSet members = accessor.GetMembers();
            var memberCached = new Dictionary<string, Member>();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(_apiSettingManager.GetConnectionString(cid)))
                {
                    if (sqlCmd == null)
                        throw new ArgumentNullException("Invalid SqlCommand!");

                    using (sqlCmd)
                    {
                        sqlConn.Open();
                        using (IDataReader reader = await sqlCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            var d = reader.Depth;
                            while (reader != null && reader.Read())
                            {
                                try
                                {
                                    var t = new T();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (!reader.IsDBNull(i))
                                        {
                                            string fieldName = reader.GetName(i);

                                            if (!memberCached.ContainsKey(fieldName))
                                            {
                                                var member = members.FirstOrDefault(m => String.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase));
                                                if (member != null)
                                                    memberCached.Add(fieldName, member);
                                            }
                                            if (memberCached.ContainsKey(fieldName))
                                            {
                                                accessor[t, memberCached[fieldName].Name] = reader.GetValue(i);
                                            }
                                        }
                                    }
                                    if (memberCached.Count > 0)
                                        results.Add(t);
                                }
                                catch (Exception ex)
                                {
                                    var msg = ex.Message;
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
            finally
            {

            }
            return results;
        } 
        #endregion
#endif






    }
}
