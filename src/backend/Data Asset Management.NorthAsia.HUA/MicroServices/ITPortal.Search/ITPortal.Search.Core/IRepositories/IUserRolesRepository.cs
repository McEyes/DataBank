using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace ITPortal.Search.Core.IRepositories
{
    public interface IUserRolesRepository
    {

        Task<List<Guid>> GetRolesByUserId(Guid userId);

    }
}
