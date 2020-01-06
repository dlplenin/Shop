using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Data.Entities;
using Shop.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;
        private Random random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            this.context = context;
            this.userHelper = userHelper;
            this.random = new Random();
        }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();
            await this.userHelper.CheckRoleAsync("Admin");
            await this.userHelper.CheckRoleAsync("Customer");

            if (!this.context.Countries.Any())
            {
                var cities = new List<City>
                {
                    new City { Name = "Medellín" },
                    new City { Name = "Bogotá" },
                    new City { Name = "Calí" }
                };

                this.context.Countries.Add(new Country
                {
                    Cities = cities,
                    Name = "Colombia"
                });

                this.context.Countries.Add(new Country
                {
                    Cities = new List<City>
                    {
                        new City { Name = "Quito" },
                        new City { Name = "Guayaquil" },
                        new City { Name = "Cuenca" }
                    },
                    Name = "Ecuador"
                });

                await this.context.SaveChangesAsync();
            }

            // Add user
            var user = await this.userHelper.GetUserByEmailAsync("dp@gmail.com");
            if (user == null)
            {
                var city = this.context.Countries
                    .Include(c => c.Cities)
                    .Where(x => x.Name == "Ecuador")
                    .FirstOrDefault()
                    .Cities
                    .Where(x => x.Name == "Quito")
                    .FirstOrDefault();

                user = new User
                {
                    FirstName = "Diego",
                    LastName = "Pardo",
                    Email = "dp@gmail.com",
                    UserName = "dp@gmail.com",
                    PhoneNumber = "098",
                    Address = "Calle Luna Calle Sol",
                    CityId = city.Id,
                    City = city


                };

                var result = await this.userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
                await this.userHelper.AddUserToRoleAsync(user, "Admin");
                var token = await this.userHelper.GenerateEmailConfirmationTokenAsync(user);
                await this.userHelper.ConfirmEmailAsync(user, token);
            }

            var isInRole = await this.userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await this.userHelper.AddUserToRoleAsync(user, "Admin");
            }

            // Add products
            if (!this.context.Products.Any())
            {
                this.AddProduct("First Product", user);
                this.AddProduct("Second Product", user);
                this.AddProduct("Third Product", user);
                await this.context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, User user)
        {
            this.context.Products.Add(new Product
            {
                Name = name,
                Price = this.random.Next(1000),
                IsAvailabe = true,
                Stock = this.random.Next(100),
                User = user
            });
        }

    }
}
