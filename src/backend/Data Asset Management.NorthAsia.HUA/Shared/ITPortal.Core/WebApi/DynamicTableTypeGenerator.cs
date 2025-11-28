using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace ITPortal.Core.WebApi
{

    // 数据库类型枚举
    public enum DbType
    {
        MySql,
        PgSql,
        MsSql,
        Oracle,
        // 预留扩展其他数据库类型
    }

    public class DynamicTableTypeGenerator
    {
        // 数据库类型映射字典，可根据需要扩展
        private readonly Dictionary<DbType, Dictionary<string, Type>> _dbTypeMappings;

        public DynamicTableTypeGenerator()
        {
            // 初始化数据库类型映射
            _dbTypeMappings = new Dictionary<DbType, Dictionary<string, Type>>
        {
            // MySQL类型映射
            { DbType.MySql, new Dictionary<string, Type>
                {
                    { "int", typeof(int) },
                    { "bigint", typeof(long) },
                    { "varchar", typeof(string) },
                    { "char", typeof(string) },
                    { "text", typeof(string) },
                    { "datetime", typeof(DateTime) },
                    { "date", typeof(DateTime) },
                    { "time", typeof(TimeSpan) },
                    { "decimal", typeof(decimal) },
                    { "float", typeof(float) },
                    { "double", typeof(double) },
                    { "bit", typeof(bool) },
                    { "tinyint", typeof(byte) },
                    { "blob", typeof(byte[]) }
                }
            },
            // PostgreSQL类型映射
            { DbType.PgSql, new Dictionary<string, Type>
                {
                    { "integer", typeof(int) },
                    { "bigint", typeof(long) },
                    { "varchar", typeof(string) },
                    { "char", typeof(string) },
                    { "text", typeof(string) },
                    { "timestamp", typeof(DateTime) },
                    { "date", typeof(DateTime) },
                    { "time", typeof(TimeSpan) },
                    { "numeric", typeof(decimal) },
                    { "real", typeof(float) },
                    { "double precision", typeof(double) },
                    { "boolean", typeof(bool) },
                    { "bytea", typeof(byte[]) }
                }
            },
            // SQL Server类型映射
            { DbType.MsSql, new Dictionary<string, Type>
                {
                    { "int", typeof(int) },
                    { "bigint", typeof(long) },
                    { "varchar", typeof(string) },
                    { "char", typeof(string) },
                    { "text", typeof(string) },
                    { "datetime", typeof(DateTime) },
                    { "date", typeof(DateTime) },
                    { "time", typeof(TimeSpan) },
                    { "decimal", typeof(decimal) },
                    { "float", typeof(double) },
                    { "real", typeof(float) },
                    { "bit", typeof(bool) },
                    { "varbinary", typeof(byte[]) }
                }
            },
            // Oracle类型映射
            { DbType.Oracle, new Dictionary<string, Type>
                {
                    { "NUMBER(10)", typeof(int) },
                    { "NUMBER(19)", typeof(long) },
                    { "VARCHAR2", typeof(string) },
                    { "NVARCHAR2", typeof(string) },
                    { "CLOB", typeof(string) },
                    { "DATE", typeof(DateTime) },
                    { "TIMESTAMP", typeof(DateTime) },
                    { "NUMBER", typeof(decimal) },
                    { "FLOAT", typeof(double) },
                    { "BLOB", typeof(byte[]) }
                }
            }
        };
        }

        /// <summary>
        /// 根据表字段信息和数据库类型，动态生成对应的实体类型
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">字段信息列表</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns>动态生成的类型</returns>
        public virtual Type GetTableType(string tableName, List<DataColumnEntity> columns, DbType dbType)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (columns == null || columns.Count == 0)
                throw new ArgumentException("字段列表不能为空", nameof(columns));

            // 获取对应数据库的类型映射
            if (!_dbTypeMappings.TryGetValue(dbType, out var typeMapping))
                throw new NotSupportedException($"不支持的数据库类型: {dbType}");

            // 创建动态程序集和模块
            var assemblyName = new AssemblyName($"DynamicTableAssembly_{tableName}_{Guid.NewGuid()}");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicTableModule");

            // 定义类型
            var typeBuilder = moduleBuilder.DefineType(
                tableName,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

            // 为每个字段创建属性
            foreach (var column in columns)
            {
                // 解析数据库类型字符串，获取基础类型名称
                string baseDataType = GetBaseDataType(column.DataType);

                // 获取对应的C#类型，如果找不到映射则默认为string
                if (!typeMapping.TryGetValue(baseDataType, out Type propertyType))
                {
                    // 尝试处理带有长度的类型，如varchar(50)
                    var typeWithoutLength = baseDataType.Split('(')[0].Trim();
                    if (!typeMapping.TryGetValue(typeWithoutLength, out propertyType))
                    {
                        propertyType = typeof(string); // 默认类型
                    }
                }

                // 创建属性
                CreateProperty(typeBuilder, column.ColName, propertyType,
                              column.Nullable == "YES", column.ColKey == "PRI");
            }

            // 创建并返回类型
            return typeBuilder.CreateType();
        }

        /// <summary>
        /// 解析数据库类型字符串，获取基础类型
        /// </summary>
        private string GetBaseDataType(string dataType)
        {
            if (string.IsNullOrWhiteSpace(dataType))
                return string.Empty;

            // 移除可能的长度信息，如varchar(50) -> varchar
            int index = dataType.IndexOf('(');
            return index > 0 ? dataType.Substring(0, index).Trim().ToLower() : dataType.Trim().ToLower();
        }

        /// <summary>
        /// 为动态类型创建属性
        /// </summary>
        private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType,
                                  bool isNullable, bool isPrimaryKey)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return;

            // 如果字段可空且是值类型，则使用可空类型
            if (isNullable && propertyType.IsValueType && propertyType != typeof(Nullable<>))
            {
                propertyType = typeof(Nullable<>).MakeGenericType(propertyType);
            }

            // 定义私有字段
            var fieldBuilder = typeBuilder.DefineField(
                $"_{char.ToLower(propertyName[0])}{propertyName.Substring(1)}",
                propertyType,
                FieldAttributes.Private);

            // 定义公共属性
            var propertyBuilder = typeBuilder.DefineProperty(
                propertyName,
                PropertyAttributes.HasDefault,
                propertyType,
                null);

            // 生成get方法
            var getMethodBuilder = typeBuilder.DefineMethod(
                $"get_{propertyName}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes);

            var getIL = getMethodBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);

            // 生成set方法
            var setMethodBuilder = typeBuilder.DefineMethod(
                $"set_{propertyName}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                new[] { propertyType });

            var setIL = setMethodBuilder.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);

            // 关联get和set方法到属性
            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

        /// <summary>
        /// 添加新的数据库类型映射（用于扩展支持其他数据库）
        /// </summary>
        public void AddDbTypeMapping(DbType dbType, Dictionary<string, Type> typeMappings)
        {
            if (typeMappings == null)
                throw new ArgumentNullException(nameof(typeMappings));

            if (_dbTypeMappings.ContainsKey(dbType))
            {
                _dbTypeMappings[dbType] = typeMappings;
            }
            else
            {
                _dbTypeMappings.Add(dbType, typeMappings);
            }
        }

        /// <summary>
        /// 为现有数据库类型添加额外的类型映射
        /// </summary>
        public void AddTypeMappingForExistingDb(DbType dbType, string dbTypeName, Type clrType)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
                throw new ArgumentNullException(nameof(dbTypeName));
            if (clrType == null)
                throw new ArgumentNullException(nameof(clrType));

            if (!_dbTypeMappings.ContainsKey(dbType))
                throw new KeyNotFoundException($"数据库类型 {dbType} 不存在");

            _dbTypeMappings[dbType][dbTypeName.ToLower()] = clrType;
        }
    }

}
