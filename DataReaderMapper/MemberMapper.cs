using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace AG.ApiLibrary.DataReaderMapper
{
    #region Mapper Objects
    public class SourceDataField
    {
        public string GetFieldName(string name)
        {
            return name;
        }
    }

    public class MapRule
    {
        public Type DataType { get; set; }
        public string PropertyName { get; set; }
    }

    #endregion

    static public class MemberMapper
    {
        static public Dictionary<string, MapRule> ForMember<DestinationObject, DestFieldType, SourceFieldName>(this Dictionary<string, MapRule> map, Expression<Func<DestinationObject, DestFieldType>> destination, Expression<Func<SourceFieldName, string>> source)
        {
            try
            {
                map = map == null ? new Dictionary<string, MapRule>() : map;
                var destBody = (MemberExpression)destination.Body;

                var argument = ((MethodCallExpression)source.Body).Arguments;

                if (argument != null && argument.Count > 0)
                {
                    var fieldName = argument[0].ToString();

                    if (destBody != null && destBody.Member != null && !String.IsNullOrEmpty(destBody.Member.Name))
                    {
                        map.Add(fieldName.Replace("\"", "").ToUpper(), new MapRule { PropertyName = destBody.Member.Name, DataType = destBody.Type });
                    }
                }
                return map;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Mapping Error: {0}", ex.Message));
            }
            return null;
        }
    }
}
