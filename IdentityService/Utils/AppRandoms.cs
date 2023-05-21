using System;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Utils;

public interface IAppRandoms
{
    string GetRandom(int? lenght);
}

public class AppRandoms : IAppRandoms
{
    public string GetRandom(int? lenght)
    {
        var bytes = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString());
        
        var base64 = Convert.ToBase64String(bytes);
        
        if (lenght is not null)
            base64 = base64.Substring(lenght.Value);
        
        return base64;
    }
}