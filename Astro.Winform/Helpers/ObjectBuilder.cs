using Astro.Models;
using Astro.ViewModels;
using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Helpers
{
    internal class ObjectBuilder
    {
        public async Task<UserViewModel> CreateUserViewModel(short userId)
        {
            UserViewModel? uvm = null;
            using (var stream = await HttpClientSingleton.GetStreamAsync("/data/users/" + userId.ToString()))
            using (var reader = new IO.Reader(stream))
            {
                var userExists = reader.ReadBoolean();
                if (userExists)
                {
                    var user = User.Create(reader);
                    uvm = new UserViewModel(user);
                }
                else
                {
                    uvm = new UserViewModel(new User());
                }
                var itemCount = reader.ReadInt32();
                while (itemCount > 0)
                {
                    uvm.Roles.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                    itemCount--;
                }
                itemCount = reader.ReadInt32();
                while (itemCount > 0)
                {
                    uvm.Countries.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                    itemCount--;
                }
                itemCount = reader.ReadInt32();
                while (itemCount > 0)
                {
                    uvm.States.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                    itemCount--;
                }
                itemCount = reader.ReadInt32();
                while (itemCount > 0)
                {
                    uvm.Cities.Add(new Option()
                    {
                        Id = reader.ReadInt32(),
                        Text = reader.ReadString()
                    });
                    itemCount--;
                }
            }
            return uvm;
        }
        public async Task<Role> CreateRoleViewModel(short roleId)
        {
            var role = new Role();
            using (var stream = await HttpClientSingleton.GetStreamAsync("/data/roles/" + roleId.ToString()))
            using (var r = new IO.Reader(stream))
            {
                var roleExists = r.ReadBoolean();
                if (roleExists)
                {
                    role.Id = r.ReadInt16();
                    role.Name = r.ReadString();
                }
                var iCount = r.ReadInt32();
                while (iCount > 0)
                {
                    role.Permissions.Add(new Permission()
                    {
                        Id = r.ReadInt16(),
                        Name = "☰ " +  r.ReadString(),
                        AllowCreate = r.ReadBoolean(),
                        AllowRead = r.ReadBoolean(),
                        AllowEdit = r.ReadBoolean(),
                        AllowDelete = r.ReadBoolean()
                    });
                    iCount--;
                }
            }
            return role;
        }
        public async Task<ProductViewModel> CreateProductViewModel(short productId)
        {
            var model = new ProductViewModel();
            using (var stream = await HttpClientSingleton.GetStreamAsync("/data/products/" + productId.ToString()))
            using (var reader = new IO.Reader(stream))
            {
                var productExist = reader.ReadBoolean();
                if (productExist)
                {
                    model.Product = Product.Create(reader);
                }

                var iCount = reader.ReadInt32();
                while (iCount > 0)
                {
                    model.Categories.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                    iCount--;
                }
                iCount = reader.ReadInt32();
                while (iCount > 0)
                {
                    model.Units.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                    iCount--;
                }
            }
            return model;
        }
    }
}
