using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace BAL.Utils
{
    public class ObjectComparer : IEqualityComparer<object>
    {
        public bool Equals(dynamic x, dynamic y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return x.TableName == y.TableName &&
                   x.Data.SequenceEqual(y.Data);
        }

        public int GetHashCode(dynamic obj)
        {
            int hashTableName = obj.TableName == null ? 0 : obj.TableName.GetHashCode();
            int hashData = obj.Data.GetHashCode();
            return hashTableName ^ hashData;
        }
    }   
}