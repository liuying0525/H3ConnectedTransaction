using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common
{
    public class Converter
    {
        public static List<T> ToList<T>(DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
            {
                return null;
            }
            List<T> result = new List<T>();

            foreach (DataRow row in data.Rows)
            {
                var r = typeof(T);

                object obj = Activator.CreateInstance(r, null);
                foreach (var propInfo in r.GetProperties())
                {
                    if (propInfo.CanWrite)
                    {
                        try
                        {
                            propInfo.SetValue(obj, Convert.ChangeType(row[propInfo.Name], propInfo.PropertyType));
                        }
                        catch (Exception ex)
                        {
                            propInfo.SetValue(obj, "");
                        }
                    }
                }
                result.Add((T)obj);
            }
            return result;
        }

        public static T ToObject<T>(DataRow row)
        {
            if (row == null)
            {
                return default;
            }

            var r = typeof(T);

            object obj = Activator.CreateInstance(r, null);
            foreach (var propInfo in r.GetProperties())
            {
                if (propInfo.CanWrite)
                {
                    try
                    {
                        propInfo.SetValue(obj, Convert.ChangeType(row[propInfo.Name], propInfo.PropertyType));
                    }
                    catch (Exception ex)
                    {
                        propInfo.SetValue(obj, "");
                    }
                }
            }
            return (T)obj;
        }
    }
}