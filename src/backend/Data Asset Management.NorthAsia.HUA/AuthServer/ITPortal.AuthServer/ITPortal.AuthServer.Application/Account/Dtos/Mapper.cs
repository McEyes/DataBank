using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.EmployeeInfos.Dtos;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Extension.System;

namespace ITPortal.AuthServer.Application
{
    public partial class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<UserEntity, LoginDto>();
            config.ForType<UserEntity, UserDto>();

            config.ForType<PrivilegeEntity, PrivilegeDto>();
            config.ForType<PrivilegeDto, PrivilegeEntity>();

            config.ForType<JabusEmployeeInfo, UserEntity>()
                .Map(desc => desc.Id, source => source.WorkNTID.ToLower())
                .Map(desc => desc.UserName, source => source.WorkNTID)
                .Map(desc => desc.Name, source => source.Name)
                .Map(desc => desc.Surname, source => source.WorkNTID)
                .Map(desc => desc.Email, source => source.WorkEmail)
                .Map(desc => desc.Department, source => source.DepartmentName);
            config.ForType<JabusEmployeeInfo, UserDto>()
                .Map(desc => desc.Id, source => source.WorkNTID.ToLower())
                .Map(desc => desc.UserName, source => source.WorkNTID)
                .Map(desc => desc.Name, source => source.Name)
                .Map(desc => desc.Surname, source => source.WorkNTID)
                .Map(desc => desc.Email, source => source.WorkEmail)
                .Map(desc => desc.Department, source => source.DepartmentName);
            config.ForType<JabusEmployeeInfo, LoginDto>()
                .Map(desc => desc.Id, source => source.WorkNTID.ToLower())
                .Map(desc => desc.UserName, source => source.WorkNTID)
                .Map(desc => desc.Name, source => source.Name)
                .Map(desc => desc.Surname, source => source.WorkNTID)
                .Map(desc => desc.Email, source => source.WorkEmail)
                .Map(desc => desc.Department, source => source.DepartmentName);
            config.ForType<JabusEmployeeInfo, UserInfoDto>()
                .Map(desc => desc.Id, source => source.WorkNTID.ToLower())
                .Map(desc => desc.UserName, source => source.WorkNTID)
                .Map(desc => desc.UserId, source => source.WorkNTID)
                .Map(desc => desc.Name, source => source.Name)
                .Map(desc => desc.EnglishName, source => source.EnglishName)
                .Map(desc => desc.ChineseName, source => source.ChineseName)
                .Map(desc => desc.Surname, source => source.WorkNTID)
                .Map(desc => desc.Email, source => source.WorkEmail)
                .Map(desc => desc.Department, source => source.DepartmentName);


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
                .Map(desc => desc.ManagementDivision, source => source.direct_manager_wdid)
                .Map(desc => desc.CostCenterId, source => source.cost_center_id);

            config.ForType<MasterEmployeeInfo, EmployeeBaseInfo>()
                .Map(desc => desc.Id, source => source.workday_id);
            config.ForType<EmployeeBaseInfo, MasterEmployeeInfo>()
                .Map(desc => desc.workday_id, source => source.Id);


            config.ForType<EmployeeBaseInfo, UserInfoDto>()
                .Map(desc => desc.Id, source => source.Ntid.ToLower())
                .Map(desc => desc.UserName, source => source.Ntid)
                .Map(desc => desc.UserId, source => source.Ntid)
                .Map(desc => desc.Name, f => $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})")
                .Map(desc => desc.EnglishName, source => source.Employee_first_name + " " + source.Employee_last_name)
                .Map(desc => desc.ChineseName, source => source.Employee_chi_name)
                .Map(desc => desc.Surname, source => source.Ntid)
                .Map(desc => desc.Email, source => source.Work_email)
                .Map(desc => desc.Department, source => source.Department_name);

            config.ForType<EmployeeBaseInfo, UserEntity>()
                .Map(desc => desc.Id, source => source.Ntid.ToLower())
                .Map(desc => desc.UserName, source => source.Ntid)
                .Map(desc => desc.Name, f => $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})")
                .Map(desc => desc.Surname, source => source.Ntid)
                .Map(desc => desc.Email, source => source.Work_email)
                .Map(desc => desc.Department, source => source.Department_name);

            config.ForType<EmployeeBaseInfo, UserDto>()
                .Map(desc => desc.Id, source => source.Ntid.ToLower())
                .Map(desc => desc.UserName, source => source.Ntid)
                .Map(desc => desc.Name, f => $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})")
                .Map(desc => desc.Surname, source => source.Ntid)
                .Map(desc => desc.Email, source => source.Work_email)
                .Map(desc => desc.Department, source => source.Department_name);

            config.ForType<EmployeeBaseInfo, LoginDto>()
                .Map(desc => desc.Id, source => source.Ntid.ToLower())
                .Map(desc => desc.UserName, source => source.Ntid)
                .Map(desc => desc.Name, f => $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})")
                .Map(desc => desc.Surname, source => source.Ntid)
                .Map(desc => desc.Email, source => source.Work_email)
                .Map(desc => desc.Department, source => source.Department_name);
        }
    }
}
