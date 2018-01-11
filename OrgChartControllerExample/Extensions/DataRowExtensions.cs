using System;
using System.Data;

namespace OrgChartControllerExample.Extensions {
    public static class DataRowExtensions {
        public static string GetTrimmedValue(this DataRow row, string fieldName) {
            var ret = row.GetValue<object>(fieldName);
            if (!string.IsNullOrEmpty(ret as string)) {
                return ((string)ret).Trim();
            }
            return ret as string;
        }

        public static T GetValueIfExists<T>(this DataRow row, string fieldName) {
            if (row.Table.Columns.Contains(fieldName)) {
                if (typeof(T) == typeof(string)) {
                    return (T)Convert.ChangeType(row.GetTrimmedValue(fieldName), typeof(T)); 
                }
                return row.GetValue<T>(fieldName);
            }
            return default(T);
        }

        public static T GetValue<T>(this DataRow row, string fieldName) {
            return GetValue<T>(row, fieldName, false);
        }

        public static T GetValue<T>(this DataRow row, string fieldName, bool throwException) {
            if (!row.Table.Columns.Contains(fieldName)) {
                if (throwException) {
                    throw new ArgumentException($"The given DataRow does not contain a field with the name \"{fieldName}\".");
                }
            }
            if (row.IsNull(fieldName)) {
                return default(T);
            }

            return row.Field<T>(fieldName);
        }

        public static T GetValue<T>(this DataRow row, int columnIndex, bool throwException) {
            if (columnIndex >= row.Table.Columns.Count) {
                if (throwException) {
                    throw new ArgumentException($"The given DataRow does not contain a field with the columnIndex \"{columnIndex}\".");
                }
                return default(T);
            }
            if (row.IsNull(columnIndex)) {
                return default(T);
            }

            return row.Field<T>(columnIndex);
        }
    }
}
