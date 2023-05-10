using System;
using Common.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace OrderService.Services.App;

public class CurrentUser : ICurrentUser
{
    public CurrentUser()
    {
        UserId = new Random().Next(200);
    }
    public int UserId { get; set; }
    
}