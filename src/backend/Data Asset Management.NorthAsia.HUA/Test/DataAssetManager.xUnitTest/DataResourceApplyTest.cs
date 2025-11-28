using DataAssetManager.DataApiServer.Application;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;
using Furion;

using ITPortal.Core.ProxyApi;
using ITPortal.Core.Services;

using Mapster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit.Abstractions;

namespace DataAssetManager.xUnitTest
{
    public class DataResourceApplyTest
    {
        // 创建服务集合
        private static IDataApiLogService _apiLogService;
        private static IDataTableService _dataTableService;
        private static IDataAuthApplyService _dataAuthApplyService;
        private static EmployeeProxyService _userProxyService;
        private readonly ITestOutputHelper Output;

        /// <summary>
        /// 这里不能通过构造函数注入，而是采用 App.GetService<> 方式
        /// </summary>
        public DataResourceApplyTest(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
            _apiLogService = App.GetService<IDataApiLogService>();
            _dataTableService = App.GetService<IDataTableService>();
            _dataAuthApplyService = App.GetService<IDataAuthApplyService>();
            _userProxyService = App.GetService<EmployeeProxyService>();
        }

        /// <summary>
        /// 全column数据申请
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestStartFlowAllColumn()
        {
            var tableid = "0700954047930306560";// "100155";// "100208";
            var userNtid = "3393704";
            var tableInfo = await _dataTableService.GetInfo(tableid);
            var userInfo = (await _userProxyService.GetUsersAsync()).Data.Where(f => f.NtId == userNtid).FirstOrDefault();
            var table = tableInfo.Adapt<DataAuthTableInfo>();
            tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            //tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            //tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            //tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            //tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            //tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            //tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            //tableInfo.ColumnList.RemoveAt(tableInfo.ColumnList.Count() - 1);
            table.ColumnList = tableInfo.ColumnList;
            var result = await _dataAuthApplyService.ApplyAuth(new DataAuthApplyInfo
            {
                ApplyType = DataApiServer.Application.AssetClients.Dtos.AuthApplyType.Individual,
                UserId = userInfo.UserId,
                UserName = userInfo.UserName,
                Reason = "流程发起测试！",
                TableList = new List<DataAuthTableInfo>() { table }
            });

            Output.WriteLine(result.Msg.ToString());
            if (result.Success) Assert.True(result.Success);
            else Assert.Contains("正在审核中:", result.Msg.ToString());
        }

        /// <summary>
        /// 空column数据申请
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestStartFlowZoreColumn()
        {
            var tableid = "1763114707398549506";
            var userNtid = "3419954";
            var tableInfo = await _dataTableService.GetInfo(tableid);
            var userInfo = (await _userProxyService.GetUsersAsync()).Data.Where(f => f.NtId == userNtid).FirstOrDefault();
            tableInfo.ColumnList.Clear();
            var table = tableInfo.Adapt<DataAuthTableInfo>();
            table.ColumnList = tableInfo.ColumnList;
            var result = await _dataAuthApplyService.ApplyAuth(new DataAuthApplyInfo
            {
                ApplyType = DataApiServer.Application.AssetClients.Dtos.AuthApplyType.Individual,
                UserId = userInfo.UserId,
                UserName = userInfo.UserName,
                Reason = "流程发起测试！",
                TableList = new List<DataAuthTableInfo>() { table }
            });

            Output.WriteLine(result.Msg.ToString());
            if (result.Success) Assert.True(result.Success);
            else Assert.Contains("正在审核中:", result.Msg.ToString());
        }

        /// <summary>
        /// 移除两个column
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestStartFlowDelColumn()
        {
            var tableid = "1848990571641925634";
            var userNtid = "3419954";
            var tableInfo = await _dataTableService.GetInfo(tableid);
            var userInfo = (await _userProxyService.GetUsersAsync()).Data.Where(f => f.NtId == userNtid).FirstOrDefault();
            tableInfo.ColumnList.RemoveAt(1);
            tableInfo.ColumnList.RemoveAt(1);
            var table = tableInfo.Adapt<DataAuthTableInfo>();
            table.ColumnList = tableInfo.ColumnList;
            var result = await _dataAuthApplyService.ApplyAuth(new DataAuthApplyInfo
            {
                ApplyType = DataApiServer.Application.AssetClients.Dtos.AuthApplyType.Individual,
                UserId = userInfo.UserId,
                UserName = userInfo.UserName,
                Reason = "流程发起测试！",
                TableList = new List<DataAuthTableInfo>() { table }
            });
            Output.WriteLine(result.Msg.ToString());
            if (result.Success) Assert.True(result.Success);
            else Assert.Contains("正在审核中:", result.Msg.ToString());
        }

        /// <summary>
        /// null column数据申请
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestStartFlowNullColumn()
        {
            var tableid = "1836294326574100482";
            var userNtid = "3419954";
            var tableInfo = await _dataTableService.GetInfo(tableid);
            var userInfo = (await _userProxyService.GetUsersAsync()).Data.Where(f => f.NtId == userNtid).FirstOrDefault();
            tableInfo.ColumnList = null;
            var table = tableInfo.Adapt<DataAuthTableInfo>();
            table.ColumnList = tableInfo.ColumnList;
            var result = await _dataAuthApplyService.ApplyAuth(new DataAuthApplyInfo
            {
                ApplyType = DataApiServer.Application.AssetClients.Dtos.AuthApplyType.Individual,
                UserId = userInfo.UserId,
                UserName = userInfo.UserName,
                Reason = "流程发起测试！",
                TableList = new List<DataAuthTableInfo>() { table }
            });

            Output.WriteLine(result.Msg.ToString());
            if (result.Success) Assert.True(result.Success);
            else Assert.Contains("正在审核中:", result.Msg.ToString());
        }


        /// <summary>
        /// table id 不存在测试
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestStartFlowNotTableId()
        {
            var tableid = "100001";
            var userNtid = "3419954";
            try
            {
                //var tableInfo = await _dataTableService.GetInfo(tableid);
                var userInfo = (await _userProxyService.GetUsersAsync()).Data.Where(f => f.NtId == userNtid).FirstOrDefault();
                //var table = tableInfo.Adapt<DataAuthTableInfo>();
                //table.ColumnList = tableInfo?.ColumnList;
                var result = await _dataAuthApplyService.ApplyAuth(new DataAuthApplyInfo
                {
                    ApplyType = DataApiServer.Application.AssetClients.Dtos.AuthApplyType.Individual,
                    UserId = userInfo.UserId,
                    UserName = userInfo.UserName,
                    Reason = "流程发起测试！",
                    TableList = new List<DataAuthTableInfo>() { new DataAuthTableInfo() { Id = tableid } }
                });
                Output.WriteLine(result.Msg.ToString());
                if (result.Success) Assert.True(!result.Success);
                else Assert.Equal("", result.Msg.ToString());
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message);
                Output.WriteLine(ex.StackTrace);
                Assert.Equal("表不存在", ex.Message);
                //Assert.Equal("申请表不能为null", ex.Message);
            }
        }



        ///// <summary>
        ///// 测试流程回调
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task TestFlowPassCallback()
        //{
        //    var flowId = "54239ac0-5bac-4089-8dd4-4c417c300997";
        //    var result = await _dataAuthApplyService.AuthBack(new DataGrantAuthBackInput
        //    {
        //        ApplyFormId = flowId,
        //        Remark = "审批批注",
        //        Result = 1//1 通过
        //    });
        //    Output.WriteLine(result.Msg.ToString());
        //    if (result.Success) Assert.True(result.Success);
        //    else Assert.Equal("", result.Msg.ToString());
        //}


        ///// <summary>
        ///// 测试流程回调
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task TestFlowRejectCallback()
        //{
        //    var flowId = "c8b0cef9-6962-4011-be35-5467ce232852";
        //    var result = await _dataAuthApplyService.AuthBack(new DataGrantAuthBackInput
        //    {
        //        ApplyFormId = flowId,
        //        Remark = "审批批注",
        //        Result = 2//2 拒绝
        //    });
        //    Output.WriteLine(result.Msg.ToString());
        //    if (result.Success) Assert.True(result.Success);
        //    else Assert.Equal("", result.Msg.ToString());
        //}
    }
}
