using System.Threading.Tasks;
using IdentityService.ViewModels;

namespace IdentityService.Services;

public interface IRoleService
{
    Task AddRoleAsync(AddRoleInput input);
}