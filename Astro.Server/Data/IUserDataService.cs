using Astro.Data;
using Astro.Models;
using Astro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Data
{
    internal interface IUserDataService
    {
        Task<UserViewModel> GetByIdAsync(short id);
        Task<CommonResult> CreateAsync(User user);
        Task<CommonResult> UpdateAsync(User user);
        Task<CommonResult> DeleteAsync(string userId);

    }
   internal class UserDataService : IUserDataService
    {
        private readonly IDatabase _db;
        public UserDataService(IDatabase db)
        {
            _db = db;
        }
        public Task<CommonResult> CreateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<CommonResult> DeleteAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserViewModel> GetByIdAsync(short id)
        {
            throw new NotImplementedException();
        }

        public Task<CommonResult> UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
