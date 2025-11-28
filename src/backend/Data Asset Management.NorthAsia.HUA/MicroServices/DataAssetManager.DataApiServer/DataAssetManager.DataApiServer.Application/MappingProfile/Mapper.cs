using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataSource.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Dtos;
using DataAssetManager.DataApiServer.Core;

using ITPortal.Core;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

namespace DataAssetManager.DataApiServer.Application.MappingProfile
{
    public partial class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<DataApiEntity, RouteInfo>();
            config.ForType<DataApiEntity, DataApiDto>();
            config.ForType<DataApiDto, DataApiEntity>();

            config.ForType<ITPortal.Core.DataSource.DbSchema, DbSchemaDto>()
                .Map(d => d.Password, s => "*******");
            config.ForType<DataSourceEntity, DataSourceInfo>();

            config.ForType<ApiFeedbackDto, ApiFeedbackEntity>();
            config.ForType<ApiFeedbackEntity, ApiFeedbackDto>();

            config.ForType<FeedbackInputDto, ApiFeedbackEntity>();
            config.ForType<ApiFeedbackEntity, FeedbackInputDto>();


            config.ForType<JabusEmployeeInfo, StaffInfo>()
                .Map(d=>d.Ntid,s=>s.WorkNTID)
                .Map(d => d.Email, s => s.WorkEmail)
                .Map(d => d.Department, s => s.DepartmentName)
                .Map(d => d.Name, s => s.Name);

            config.ForType<FlowTempActDto, FlowActApprover>();

            //.Map(dest => dest.FullName, src => src.FirstName + src.LastName)
            //.Map(dest => dest.IdCard, src => src.IdCard.Replace("1234", "****"));


            config.ForType<EmployeeBaseInfo, JabusEmployeeInfo>()
                .Map(desc => desc.WorkNTID, source => source.Ntid)
                .Map(desc => desc.EmployeeCode, source => source.employee_id)
                .Map(desc => desc.Workcell, source => source.employee_workcell)
                .Map(desc => desc.WorkEmail, source => source.Work_email)
                .Map(desc => desc.ChineseName, source => source.Employee_chi_name)
                .Map(desc => desc.EnglishName, source => source.Employee_first_name + " " + source.Employee_last_name)
                .Map(desc => desc.Name, f => $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})")
                .Map(desc => desc.JobTitle, source => source.global_job_title)
                .Map(desc => desc.BusinessTitleLocal, source => source.business_title)
                .Map(desc => desc.DepartmentName, source => source.Department_name)
                .Map(desc => desc.JobFamily, source => source.job_classification)
                .Map(desc => desc.Location, source => source.company_location)
                .Map(desc => desc.Country, source => source.employee_nationality)
                .Map(desc => desc.PlantDivision, source => source.employee_workcell)
                .Map(desc => desc.CompanyCode, source => source.company_code)
                .Map(desc => desc.ManagerNTID, source => source.direct_manager_ntid)
                .Map(desc => desc.ManagerEmail, source => source.direct_manager_email)
                .Map(desc => desc.ManagementDivision, source => source.direct_manager_wdid);

            config.ForType<MasterEmployeeInfo, EmployeeBaseInfo>()
                .Map(desc => desc.Id, source => source.workday_id);
            config.ForType<EmployeeBaseInfo, MasterEmployeeInfo>()
                .Map(desc => desc.workday_id, source => source.Id);


            config.ForType<EmployeeBaseInfo, UserInfo>()
                .Map(desc => desc.Id, source => source.Ntid)
                .Map(desc => desc.UserName, source => source.Ntid)
                .Map(desc => desc.UserId, source => source.Ntid)
                .Map(desc => desc.Name, f => $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})")
                .Map(desc => desc.EnglishName, source => source.Employee_first_name + " " + source.Employee_last_name)
                .Map(desc => desc.ChineseName, source => source.Employee_chi_name)
                .Map(desc => desc.Surname, source => source.Ntid)
                .Map(desc => desc.Email, source => source.Work_email)
                .Map(desc => desc.Department, source => source.Department_name);

            //config.ForType<EmployeeBaseInfo, UserEntity>()
            //    .Map(desc => desc.Id, source => source.Ntid)
            //    .Map(desc => desc.UserName, source => source.Ntid)
            //    .Map(desc => desc.Name, f => $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})")
            //    .Map(desc => desc.Surname, source => source.Ntid)
            //    .Map(desc => desc.Email, source => source.Work_email)
            //    .Map(desc => desc.Department, source => source.Department_name);

            //config.ForType<EmployeeBaseInfo, UserDto>()
            //    .Map(desc => desc.Id, source => source.Ntid)
            //    .Map(desc => desc.UserName, source => source.Ntid)
            //    .Map(desc => desc.Name, f => $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})")
            //    .Map(desc => desc.Surname, source => source.Ntid)
            //    .Map(desc => desc.Email, source => source.Work_email)
            //    .Map(desc => desc.Department, source => source.Department_name);

            //config.ForType<EmployeeBaseInfo, LoginDto>()
            //    .Map(desc => desc.Id, source => source.Ntid)
            //    .Map(desc => desc.UserName, source => source.Ntid)
            //    .Map(desc => desc.Name, f => $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})")
            //    .Map(desc => desc.Surname, source => source.Ntid)
            //    .Map(desc => desc.Email, source => source.Work_email)
            //    .Map(desc => desc.Department, source => source.Department_name);
        }
    }
}
