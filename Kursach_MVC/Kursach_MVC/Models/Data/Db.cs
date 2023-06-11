using Kursach_MVC.Models.ViewModels.Pages;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Kursach_MVC.Models.Data
{
    public class Db : DbContext
    {
        
        public DbSet<PagesDTO> Pages { get; set; }

        //6 day
        public DbSet<SidebarDTO> Sidebars { get; set; }

        public DbSet<RestaurantDTO> Restaurants { get; set;}

        public DbSet<CategoryDTO> Categories { get; set; }

        public DbSet<CoordinateRestDTO> CoordinateRest { get; set; }

        public DbSet<ProductDTO> Products { get; set; }
        public override int SaveChanges()
        {
            var newRestaurants = ChangeTracker.Entries<RestaurantVM>().Where(e => e.State == EntityState.Added).Select(e => e.Entity);

            foreach (var restaurant in newRestaurants)
            {
                CoordinateRestVM coordinate = new CoordinateRestVM { IdRest = restaurant.Id };
              
            }

            return base.SaveChanges();
        }
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<RoleDTO> Roles { get; set; }
        public DbSet<UserRoleDTO> UserRoles { get; set; }
        public DbSet<OrderDTO> Orders { get; set; }
        public DbSet<OrderDetailsDTO> OrderDetails { get; set; }

    }

}