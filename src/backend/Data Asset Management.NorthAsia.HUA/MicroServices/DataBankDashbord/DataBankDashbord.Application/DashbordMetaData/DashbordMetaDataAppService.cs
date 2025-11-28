using DataBankDashbord.Application.DashbordMetaData.Dtos;
using DataBankDashbord.Application.DashbordMetaData.Services;
using ITPortal.Core.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData
{


    /// <summary>
    /// DashbordMetaData服务
    /// </summary>
    [AppAuthorize]
    [Route("api/Dashbord/", Name = "DashbordMetaData服务")]
    [ApiDescriptionSettings(GroupName = "DashbordMetaData")]
    public class DashbordMetaDataAppService : IDynamicApiController
    {


        private readonly IDashbordMetaDataService _dashbordMetaDataService;
        public DashbordMetaDataAppService(IDashbordMetaDataService dashbordMetaDataService)
        {
            _dashbordMetaDataService = dashbordMetaDataService;
        }


        [HttpGet("getDashbordListForOwnerId")]
        public async Task<List<DashbordMetadataDto>> getDashbordListForOwnerId(string ownerId)
        {
            return await _dashbordMetaDataService.GetDashbordListForOwnerId(ownerId);
        }

        [HttpPost("getDashbordListForColumnId")]
        public async Task<List<DashbordMetadataDto>> getDashbordListForColumnId([FromBody] apiDashbordDto dto)
        {
            return await _dashbordMetaDataService.getDashbordListForColumnId(dto.listColumnId);
        }

        [HttpGet("getDashbordList")]
        public async Task<List<DashbordMetadataDto>> GetDashbordList()
        {
            return await _dashbordMetaDataService.GetDashbordList();
        }


        [HttpPost("updateStatu")]
        public async Task<bool> updateStatu([FromBody] apiDashbordDto dto)
        {

            return await _dashbordMetaDataService.UpdateDashbordStatu(dto);
        }

        //[HttpGet("GetAllIndicatorCode"), NonUnify]
        //public async Task<IActionResult> GetAllIndicatorCode()
        //{

        //    var list = await _dashbordMetaDataService.GetAllIndicatorCode();

        //    return ExcelExporter.ExportExcel(list, "AllIndicatorCode");


        //}


        [HttpGet("updateIndicatorCode")]
        public async Task<bool> updateIndicatorCode()
        {

            return await _dashbordMetaDataService.CraeteIndicatorCode();
        }



    }
}
