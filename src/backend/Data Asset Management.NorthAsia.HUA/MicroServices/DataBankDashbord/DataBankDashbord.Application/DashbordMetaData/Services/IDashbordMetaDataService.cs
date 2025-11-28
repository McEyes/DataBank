using DataBankDashbord.Application.DashbordMetaData.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData.Services
{
    public interface IDashbordMetaDataService
    {


        /// <summary>
        /// 根据ownerId获取所有标记记录
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        Task<List<DashbordMetadataDto>> GetDashbordListForOwnerId(string ownerId);


        /// <summary>
        /// 根据columnIdList获取所有标记记录
        /// </summary>
        /// <param name="columnIdList"></param>
        /// <returns></returns>
        Task<List<DashbordMetadataDto>> getDashbordListForColumnId(List<string> columnIdList);


        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>

        Task<List<DashbordMetadataDto>> GetDashbordList();



        /// <summary>
        /// 生成所有IndicatorCode
        /// </summary>
        /// <returns></returns>

        Task<List<IndicatorCodeDto>> GetAllIndicatorCode();

        /// <summary>
        /// 更新标记
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> UpdateDashbordStatu(apiDashbordDto dto);

        Task<bool> CraeteIndicatorCode();

  
    }
}
