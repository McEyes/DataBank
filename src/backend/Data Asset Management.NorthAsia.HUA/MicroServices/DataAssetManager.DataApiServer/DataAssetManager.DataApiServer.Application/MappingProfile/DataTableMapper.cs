using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataApiServer.Core;

using ITPortal.Core.ProxyApi.Dto;

using System.Data.Common;
using System.Runtime.Serialization;

namespace DataAssetManager.DataApiServer.Application.MappingProfile
{
    public partial class DataTableMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<DbColumn, DataColumnEntity>();
            config.ForType<DataColumnEntity, DbColumn>();

            config.ForType<DataColumnEntity, DataTableInput>();
            config.ForType<DataTableInput, DataColumnEntity>();
            config.ForType<DataColumnEntity, DataTableInfo>();
            config.ForType<DataTableInfo, DataColumnEntity>();

            config.ForType<MetaDataUserEntity, ApproveUser>();
            config.ForType<ApproveUser, MetaDataUserEntity>();


            config.ForType<DataAuthApplyInfo, DataAuthApplyEntity>();
            config.ForType<DataAuthApplyEntity, DataAuthApplyInfo>();

            config.ForType<DataAuthTableInfo, DataAuthApplyEntity>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.ColumnList)
                .Map(dest => dest.SmeId, src => src.OwnerId)
                .Map(dest => dest.SmeName, src => src.OwnerName)
                .Map(dest => dest.SmeDept, src => src.OwnerDept);
            config.ForType<DataAuthApplyEntity, DataAuthTableInfo>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.ColumnList)
                .Map(dest => dest.OwnerId, src => src.SmeId)
                .Map(dest => dest.OwnerName, src => src.SmeName)
                .Map(dest => dest.OwnerDept, src => src.SmeDept);


            config.ForType<DataAuthTableInfo, DataAuthApplyDetailEntity>()
                .IgnoreNullValues(true)//忽略空值映射//忽略指定字段
                .Ignore(dest => dest.Id)
                .Map(dest => dest.ObjectId, src => src.TableId)
                .Map(dest => dest.ObjectName, src => src.TableName)
                .Map(dest => dest.ObjectType, src => "table");

            //.IgnoreNullValues(true)//忽略空值映射
            //.Ignore(dest => dest.UserAge)//忽略指定字段
            //.IgnoreAttribute(typeof(DataMemberAttribute))//忽略指定特性的字段
            //.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase)//忽略字段名称的大小写
            //.IgnoreNonMapped(true);//忽略除以上配置的所有字段
            config.ForType<DataAuthApplyDetailEntity, DataAuthTableInfo>()
                .IgnoreNullValues(true)//忽略空值映射//忽略指定字段
                .Ignore(dest => dest.Id)//忽略指定字段
                .Map(dest => dest.TableId, src => src.ObjectId)
                .Map(dest => dest.TableName, src => src.ObjectName);

            config.ForType<DataTableInfo, DataTableInput>();
            config.ForType<DataTableInput, DataTableInfo>();
            config.ForType<DataTableInput, DataAuthTableInfo>().Ignore(dest => dest.ColumnList);
            config.ForType<DataAuthTableInfo, DataTableInput>().Ignore(dest => dest.ColumnList);

            config.ForType<DataColumnDto, DataColumnEntity>();
            config.ForType<DataColumnEntity, DataColumnDto>();
            config.ForType<DataColumnInfo, DataColumnEntity>();
            config.ForType<DataColumnEntity, DataColumnInfo>();


            config.ForType<DataAuthApplyInfo, DataAuthCheckDto>();
            config.ForType<DataAuthTableInfo, CheckTableInfo>();
        }
    }
}
