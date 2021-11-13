using CicekSepeti.Core.Context;
using CicekSepeti.Domain.Concrete;
using CicekSepeti.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Concrete
{
    public class DatabaseInitializerService : IDatabaseInitializer
    {
        private readonly AplicationContext _context;
        UserManager<User> _userManager;
        public DatabaseInitializerService(AplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Uygulama çalışırken default mock dataları oluşturur.
        /// </summary>
        /// <returns></returns>
        public async Task SeedAsync()
        {
            if (!_context.Products.Any())
            {
                List<Product> products = new List<Product>
                {
                   new Product("1000", "Test-1000", null, 1000, 10,150,true, 1),
                   new Product("2000", "Test-2000", null, 1250, 5,500, true,1),
                   new Product("3000", "Test-3000", null, 150, null,1000, true,1),
                   new Product("4000", "Test-4000", null, 5, 1,25000, true,1),
                };

                await _context.Products.AddRangeAsync(products);
            }

            if (!_context.Discounts.Any())
            {
                List<Discount> discounts = new List<Discount>
                {
                   new Discount("1000", 100, DateTime.Now.AddDays(1),200,1),
                   new Discount("2000", 125, DateTime.Now.AddDays(1), 250,1),
                   new Discount("3000", 50,  DateTime.Now.AddMinutes(1),90, 1),
                };

                await _context.Discounts.AddRangeAsync(discounts);
            }

            if (!_userManager.Users.Any())
            {
                var result = await _userManager.CreateAsync(new User
                {
                    UserName = "test",
                    Email = "test@test.com",
                }, "test");

                result = await _userManager.CreateAsync(new User
                {
                    UserName = "test2",
                    Email = "test2@test.com",
                }, "test2");

                if (!result.Succeeded)
                    throw new Exception(result.Errors.FirstOrDefault().Description);
            }
        }
    }
}
