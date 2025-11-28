using System;
namespace jb.home.Application.Contracts.FlowDataSourceAppCfgs
{
    public class CreateFlowDataSourceAppCfgInputDto
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public string Ntid { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}