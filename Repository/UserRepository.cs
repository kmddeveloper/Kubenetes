using AG.ApiLibrary.DataReaderMapper;
using Kubernetes.TransferObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Kubernetes.Repository
{
    public class UserRepository: IUserRepository
    {
        readonly IDataReaderMapper _dataReaderMapper;
        public UserRepository(IDataReaderMapper dataReaderMapper)
        {
            _dataReaderMapper = dataReaderMapper;
        }

        #region GetUserByIdAsync
        public async Task<User> GetUserByIdAsync(int Id)
        {
            try
            {
                var map = _dataReaderMapper.CreateMap()
                   .ForMember<User, Int32, SourceDataField>(d => d.Id, s => s.GetFieldName("Id"))
                   .ForMember<User, String, SourceDataField>(d => d.FirstName, s => s.GetFieldName("FirstName"))
                   .ForMember<User, String, SourceDataField>(d => d.LastName, s => s.GetFieldName("LastName"))                
                    ;
                var sqlCmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "appV1_getUserById"
                };
                sqlCmd.Parameters.AddWithValue("@id", Id);             

                var data = await _dataReaderMapper.MapAsync<User>(sqlCmd, map);
                return data;
            }
            catch (Exception e)
            {
               // _logger.LogError("UserRepository:GetUserAUthoizedOptionsAsync:{0}", e.Message);
            }            
            return   null;
        }
        #endregion

    }
}
