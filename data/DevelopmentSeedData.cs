using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravelLocationManagement.Models;
using Microsoft.Extensions.DependencyInjection;

namespace TravelLocationManagement.Data
{
    public static class DevelopmentSeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var context = new TravelLocationContext(
                serviceProvider.GetRequiredService<DbContextOptions<TravelLocationContext>>()))
            {
                // Look for any users.
                if (await context.Users.AnyAsync())
                {
                    return;   // DB has been seeded
                }

                var user1 = new User
                {
                    UserName = "user1@example.com",
                    PasswordHash = "Password1!"
                };

                var user2 = new User
                {
                    UserName = "user2@example.com",
                    PasswordHash = "Password2!"
                };

                context.Users.AddRange(user1, user2);
                await context.SaveChangesAsync();

                var list1 = new List
                {
                    ListName = "Favorites",
                    OwnerId = user1.Id
                };

                var list2 = new List
                {
                    ListName = "MyList",
                    OwnerId = user2.Id
                };

                context.Lists.AddRange(list1, list2);
                await context.SaveChangesAsync();

                var location1 = new Location
                {
                    PlaceId = "Eiffel Tower",
                    LocationAddress = new Address
                    {
                        Street = "Champ de Mars",
                        CityTown = "Paris",
                        StateProv = "",
                        ZipCode = "75007",
                        Country = "France"
                    }
                };

                var location2 = new Location
                {
                    PlaceId = "Great Wall of China",
                    Latitude = 40.431908,
                    Longitude = 116.570374
                };

                var location3 = new Location
                {
                    PlaceId = "Statue of Liberty",
                    LocationAddress = new Address
                    {
                        Street = "Liberty Island",
                        CityTown = "New York",
                        StateProv = "NY",
                        ZipCode = "10004",
                        Country = "USA"
                    }
                };

                var location4 = new Location
                {
                    PlaceId = "Sydney Opera House",
                    Latitude = -33.8567844,
                    Longitude = 151.2152967
                };

                context.Locations.AddRange(location1, location2, location3, location4);
                await context.SaveChangesAsync();

                var listItem1 = new ListItem
                {
                    ListId = list1.ListId,
                    LocationId = location1.LocationId
                };

                var listItem2 = new ListItem
                {
                    ListId = list1.ListId,
                    LocationId = location2.LocationId
                };

                var listItem3 = new ListItem
                {
                    ListId = list2.ListId,
                    LocationId = location3.LocationId
                };

                var listItem4 = new ListItem
                {
                    ListId = list2.ListId,
                    LocationId = location4.LocationId
                };

                context.ListItems.AddRange(listItem1, listItem2, listItem3, listItem4);
                await context.SaveChangesAsync();
            }
        }
    }
}