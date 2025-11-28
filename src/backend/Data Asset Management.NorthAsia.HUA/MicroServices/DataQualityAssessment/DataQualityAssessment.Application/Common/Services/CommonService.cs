using DataQualityAssessment.Application.DataAssets.Services;
using DataQualityAssessment.Core.DbContextLocators;
using DataQualityAssessment.Core.Entities.DataAsset;
using Microsoft.Extensions.DependencyInjection;

namespace DataQualityAssessment.Application.Common.Services
{
    public class CommonService : ICommonService, ITransient
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public CommonService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public List<string> GetAllDepartments()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var employeeRepository = Db.GetRepository<EmployeeBaseInfoEntity, DataAssetDbContextLocator>(serviceProvider);
                var query = employeeRepository.Where(t => !string.IsNullOrWhiteSpace(t.department_name)).AsQueryable();

                return query.Select(t => t.department_name).Distinct().ToList();
            }
        }
    }
}
