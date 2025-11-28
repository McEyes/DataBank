using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;

using System.Reflection;
using System.Reflection.Emit;

namespace DataAssetManager.DataApiServer.Application.Services
{

    //// 数据库类型枚举
    //public enum DbType
    //{
    //    MySql,
    //    PgSql,
    //    MsSql,
    //    Oracle,
    //    // 预留扩展其他数据库类型
    //}

    public class DynamicTableTypeGenerator:ISingleton
    {
        // 数据库类型映射字典，可根据需要扩展
        private readonly Dictionary<ITPortal.Core.DataSource.DbType, Dictionary<string, Type>> _dbTypeMappings;
        private readonly Dictionary<string, Type> _tableMappings;

        public DynamicTableTypeGenerator()
        {
            _tableMappings = new Dictionary<string, Type>();
            // 初始化数据库类型映射
            _dbTypeMappings = new Dictionary<ITPortal.Core.DataSource.DbType, Dictionary<string, Type>>
                                {
                                    // MySQL类型映射
//tinyint tinyint整型
//int int整型
//bigint  bigint整型
//float   单精度
//double  双精度
//decimal 定点数
//char    定长字符串
//varchar 变长字符串
//text    长文本
//date    date日期
//time    time日期
//year    year日期
//datetime    datetime日期
                                    { ITPortal.Core.DataSource.DbType.MYSQL, new Dictionary<string, Type>
                                        {
                                          // 整数类型
                                        {"tinyint", typeof(sbyte)},
                                        {"tinyint unsigned", typeof(byte)},
                                        {"smallint", typeof(short)},
                                        {"smallint unsigned", typeof(ushort)},
                                        {"mediumint", typeof(int)},
                                        {"mediumint unsigned", typeof(uint)},
                                        {"int", typeof(int)},
                                        {"int unsigned", typeof(uint)},
                                        {"integer", typeof(int)},
                                        {"integer unsigned", typeof(uint)},
                                        {"bigint", typeof(long)},
                                        {"bigint unsigned", typeof(ulong)},
        
                                        // 浮点类型
                                        {"float", typeof(float)},
                                        {"double", typeof(double)},
                                        {"decimal", typeof(decimal)},
                                        {"numeric", typeof(decimal)},
        
                                        // 字符串类型
                                        {"char", typeof(string)},
                                        {"varchar", typeof(string)},
                                        {"tinytext", typeof(string)},
                                        {"text", typeof(string)},
                                        {"mediumtext", typeof(string)},
                                        {"longtext", typeof(string)},
        
                                        // 二进制类型
                                        {"binary", typeof(byte[])},
                                        {"varbinary", typeof(byte[])},
                                        {"tinyblob", typeof(byte[])},
                                        {"blob", typeof(byte[])},
                                        {"mediumblob", typeof(byte[])},
                                        {"longblob", typeof(byte[])},
        
                                        // 日期时间类型
                                        {"date", typeof(DateOnly)},
                                        {"time", typeof(TimeOnly)},
                                        {"datetime", typeof(DateTime)},
                                        {"timestamp", typeof(DateTime)},
                                        {"year", typeof(short)},
        
                                        // 布尔类型
                                        {"bool", typeof(bool)},
                                        {"boolean", typeof(bool)},
        
                                        // 其他类型
                                        {"bit", typeof(bool)}, // 对于bit(1)
                                        {"bit(8)", typeof(byte)}, // 对于bit(8)
                                        {"bit(16)", typeof(ushort)}, // 对于bit(16)
                                        {"bit(32)", typeof(uint)}, // 对于bit(32)
                                        {"bit(64)", typeof(ulong)}, // 对于bit(64)
                                        {"enum", typeof(string)},
                                        {"set", typeof(string)},
                                        {"json", typeof(string)},
                                        {"geometry", typeof(string)},
                                        {"point", typeof(ValueTuple<double, double>)},
                                        {"linestring", typeof(string)},
                                        {"polygon", typeof(string)},
                                        {"uuid", typeof(Guid)}
                                        }
                                    },
                                    // PostgreSQL类型映射
                                    { ITPortal.Core.DataSource.DbType.POSTGRE_SQL, new Dictionary<string, Type>
                                        {
                                             // 数值类型
                                            {"smallint", typeof(short)},
                                            {"int2", typeof(short)},
                                            {"integer", typeof(int)},
                                            {"int4", typeof(int)},
                                            {"bigint", typeof(long)},
                                            {"int8", typeof(long)},
                                            {"decimal", typeof(decimal)},
                                            {"numeric", typeof(decimal)},
                                            {"real", typeof(float)},
                                            {"float4", typeof(float)},
                                            {"double precision", typeof(double)},
                                            {"float8", typeof(double)},
                                            {"serial", typeof(int)},
                                            {"serial4", typeof(int)},
                                            {"bigserial", typeof(long)},
                                            {"serial8", typeof(long)},
        
                                            // 字符串类型
                                            {"varchar", typeof(string)},
                                            {"char", typeof(string)},
                                            {"character", typeof(string)},
                                            {"text", typeof(string)},
        
                                            // 二进制类型
                                            {"bytea", typeof(byte[])},
        
                                            // 日期时间类型
                                            {"timestamp", typeof(DateTime)},
                                            {"timestamp without time zone", typeof(DateTime)},
                                            {"timestamp with time zone", typeof(DateTimeOffset)},
                                            {"date", typeof(DateOnly)},
                                            {"time", typeof(TimeOnly)},
                                            {"time without time zone", typeof(TimeOnly)},
                                            {"time with time zone", typeof(DateTimeOffset)},
                                            {"interval", typeof(TimeSpan)},
        
                                            // 布尔类型
                                            {"boolean", typeof(bool)},
                                            {"bool", typeof(bool)},
        
                                            // 货币类型
                                            {"money", typeof(decimal)},
        
                                            // 网络地址类型
                                            {"inet", typeof(string)},
                                            {"cidr", typeof(string)},
                                            {"macaddr", typeof(string)},
                                            {"macaddr8", typeof(string)},
        
                                            // 几何类型
                                            {"point", typeof(ValueTuple<double, double>)},
                                            {"line", typeof(string)},
                                            {"lseg", typeof(string)},
                                            {"box", typeof(string)},
                                            {"path", typeof(string)},
                                            {"polygon", typeof(string)},
                                            {"circle", typeof(string)},
        
                                            // 文本搜索类型
                                            {"tsvector", typeof(string)},
                                            {"tsquery", typeof(string)},
        
                                            // UUID类型
                                            {"uuid", typeof(Guid)},
        
                                            // JSON类型
                                            {"json", typeof(string)},
                                            {"jsonb", typeof(string)},
        
                                            // 数组类型(这里使用object作为基础类型，实际使用时需要根据元素类型处理)
                                            {"ARRAY", typeof(object[])},
        
                                            // 其他特殊类型
                                            {"oid", typeof(uint)},
                                            {"xid", typeof(uint)},
                                            {"cid", typeof(uint)},
                                            {"xml", typeof(string)},
                                            {"pg_lsn", typeof(string)},
                                            {"txid_snapshot", typeof(string)},
                                            {"pg_node_tree", typeof(string)}
                                        }
                                    },
                // SQL Server类型映射
//image   变长二进制(max 2GB)
//text    变长文本
//uniqueidentifier    全局唯一标识符(GUID)
//date    Date(仅存储日期部分)
//time    Time(仅存储时间部分)
//datetime2   datetime2(存储日期和时间)
//datetimeoffset  日期时间偏移
//tinyint 微小整数
//smallint    小整数
//int 整数
//smalldatetime   小日期时间
//real    单精度浮点数
//money   金额(四位)
//datetime    datetime(存储日期和时间，具有更高的精度和可选的小数秒部分)
//float   浮点数
//sql_variant sql_variant
//ntext   可变长度文本
//bit 位值(0 | 1)
//decimal 小数(decimal)
//numeric 小数(numeric)
//char    字符串
//bigint  大整数
//varbinary   可变二进制
//varchar 变长字符串
//binary  二进制
//char    固定长度字符
//timestamp   时间戳
//nvarchar    可变长度的字符(nvarchar)
//sysname 特殊数据类型(sysname)
//nchar   固定长度字(nchar)
//hierarchyid Hierarchyid
//geometry    Geometry
//geography   Geography
//xml XML 数据和文档
//smallmoney  小金额(四位)
                                    { ITPortal.Core.DataSource.DbType.SQL_SERVER, new Dictionary<string, Type>
                                        {
                                            // 整数类型
                                            {"tinyint", typeof(byte)},
                                            {"smallint", typeof(short)},
                                            {"int", typeof(int)},
                                            {"bigint", typeof(long)},
        
                                            // 浮点类型
                                            {"decimal", typeof(decimal)},
                                            {"numeric", typeof(decimal)},
                                            {"float", typeof(double)},
                                            {"real", typeof(float)},
        
                                            // 字符串类型
                                            {"char", typeof(string)},
                                            {"varchar", typeof(string)},
                                            {"text", typeof(string)},
                                            {"nchar", typeof(string)},
                                            {"nvarchar", typeof(string)},
                                            {"ntext", typeof(string)},
                                            {"xml", typeof(string)},
        
                                            // 二进制类型
                                            {"binary", typeof(byte[])},
                                            {"varbinary", typeof(byte[])},
                                            {"image", typeof(byte[])},
        
                                            // 日期时间类型
                                            {"date", typeof(DateOnly)},
                                            {"time", typeof(TimeOnly)},
                                            {"datetime", typeof(DateTime)},
                                            {"datetime2", typeof(DateTime)},
                                            {"smalldatetime", typeof(DateTime)},
                                            {"datetimeoffset", typeof(DateTimeOffset)},
                                            {"timestamp", typeof(byte[])},
        
                                            // 布尔类型
                                            {"bit", typeof(bool)},
        
                                            // 货币类型
                                            {"money", typeof(decimal)},
                                            {"smallmoney", typeof(decimal)},
        
                                            // 其他类型
                                            {"uniqueidentifier", typeof(Guid)},
                                            {"sql_variant", typeof(object)},
                                            {"hierarchyid", typeof(string)},
                                            {"geography", typeof(string)},
                                            {"geometry", typeof(string)},
                                            //{"table", typeof(object)}, // 特殊类型，实际使用需单独处理
                                            //{"cursor", typeof(object)}, // 特殊类型，实际使用需单独处理
                                            {"rowversion", typeof(byte[])}
                                        }
                                    },
                                    // Oracle类型映射
                                    { ITPortal.Core.DataSource.DbType.ORACLE, new Dictionary<string, Type>
                                        {
                                           // 整数类型
                                            {"number(1)", typeof(byte)},
                                            {"number(2)", typeof(byte)},
                                            {"number(3)", typeof(short)},
                                            {"number(4)", typeof(short)},
                                            {"number(5)", typeof(int)},
                                            {"number(9)", typeof(int)},
                                            {"number(10)", typeof(long)},
                                            {"number(18)", typeof(long)},
                                            {"integer", typeof(int)},
                                            {"int", typeof(int)},
                                            {"smallint", typeof(short)},
        
                                            // 浮点类型
                                            {"number", typeof(decimal)},
                                            {"number(p,s)", typeof(decimal)},
                                            {"float", typeof(double)},
                                            {"binary_float", typeof(float)},
                                            {"binary_double", typeof(double)},
        
                                            // 字符串类型
                                            {"char", typeof(string)},
                                            {"varchar", typeof(string)},
                                            {"varchar2", typeof(string)},
                                            {"nvarchar2", typeof(string)},
                                            {"clob", typeof(string)},
                                            {"nclob", typeof(string)},
                                            {"long", typeof(string)},
        
                                            // 二进制类型
                                            {"blob", typeof(byte[])},
                                            {"raw", typeof(byte[])},
                                            {"long raw", typeof(byte[])},
        
                                            // 日期时间类型
                                            {"date", typeof(DateTime)},
                                            {"timestamp", typeof(DateTime)},
                                            {"timestamp with time zone", typeof(DateTimeOffset)},
                                            {"timestamp with local time zone", typeof(DateTimeOffset)},
                                            {"interval year to month", typeof(TimeSpan)},
                                            {"interval day to second", typeof(TimeSpan)},
        
                                            // 布尔类型
                                            {"boolean", typeof(bool)},
        
                                            // 其他类型
                                            {"rowid", typeof(string)},
                                            {"urowid", typeof(string)},
                                            {"xmltype", typeof(string)},
                                            {"bfile", typeof(string)}, // 实际使用需用 OracleBFile 处理
                                            {"guid", typeof(Guid)},
                                            {"sdot_geometry", typeof(string)}, // 空间类型
                                            {"json", typeof(string)},
                                            {"json_object_t", typeof(string)},
                                            {"json_array_t", typeof(string)}
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
        public virtual Type GetTableType(string tableName, List<DataColumnDto> columns, ITPortal.Core.DataSource.DbType dbType)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (columns == null || columns.Count == 0)
                throw new ArgumentException("字段列表不能为空", nameof(columns));

            // 获取对应数据库的类型映射
            if (!_dbTypeMappings.TryGetValue(dbType, out var typeMapping))
                throw new NotSupportedException($"不支持的数据库类型: {dbType}");
            var feildKey = MD5Encryption.Encrypt(string.Join("", columns.Select(f => f.ColName)));
            if (_tableMappings.TryGetValue(tableName+ feildKey, out Type type))
                return type;

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
                              column.Nullable == "1" || "true".Equals(column.Nullable, StringComparison.CurrentCultureIgnoreCase), column.ColKey == "PRI");
            }

            // 创建并返回类型
            var typeData = typeBuilder.CreateType();
            _tableMappings.TryAdd(tableName, typeData);
            return typeData;
        }

        /// <summary>
        /// 根据表字段信息和数据库类型，动态生成对应的实体类型
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">字段信息列表</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns>动态生成的类型</returns>
        public virtual Type GetTableType(string tableName, List<DataColumnEntity> columns, ITPortal.Core.DataSource.DbType dbType)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (columns == null || columns.Count == 0)
                throw new ArgumentException("字段列表不能为空", nameof(columns));

            // 获取对应数据库的类型映射
            if (!_dbTypeMappings.TryGetValue(dbType, out var typeMapping))
                throw new NotSupportedException($"不支持的数据库类型: {dbType}");

            if (_tableMappings.TryGetValue(tableName, out Type type))
                return type;

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
                              column.Nullable == "1" || "true".Equals(column.Nullable, StringComparison.CurrentCultureIgnoreCase), (column.ColKey == "1" || "1".Equals(column.ColKey, StringComparison.CurrentCultureIgnoreCase) || column.ColKey == "PRI"));
            }

            // 创建并返回类型
            var typeData = typeBuilder.CreateType();
            _tableMappings.TryAdd(tableName, typeData);
            return typeData;
        }

        /// <summary>
        /// 生成表类型的List集合类型
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">字段信息列表</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns>List&lt;动态表类型&gt;的类型</returns>
        public virtual Type GetTableListType(string tableName, List<DataColumnEntity> columns, ITPortal.Core.DataSource.DbType dbType)
        {
            // 首先生成表类型
            Type tableType = GetTableType(tableName, columns, dbType);

            // 创建List<表类型>的泛型类型
            Type listType = typeof(List<>).MakeGenericType(tableType);

            return listType;
        }

        /// <summary>
        /// 生成表类型的List集合类型
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">字段信息列表</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns>List&lt;动态表类型&gt;的类型</returns>
        public virtual Type GetTableListType(string tableName, List<DataColumnDto> columns, ITPortal.Core.DataSource.DbType dbType)
        {
            // 首先生成表类型
            Type tableType = GetTableType(tableName, columns, dbType);

            // 创建List<表类型>的泛型类型
            Type listType = typeof(List<>).MakeGenericType(tableType);

            return listType;
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
        public void AddDbTypeMapping(ITPortal.Core.DataSource.DbType dbType, Dictionary<string, Type> typeMappings)
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
        public void AddTypeMappingForExistingDb(ITPortal.Core.DataSource.DbType dbType, string dbTypeName, Type clrType)
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
