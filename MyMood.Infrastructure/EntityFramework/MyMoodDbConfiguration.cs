using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Migrations;
using MyMood.Domain;

namespace MyMood.Infrastructure.EntityFramework
{
    internal sealed class MyMoodDbConfiguration : DbMigrationsConfiguration<MyMoodDbContext>
    {
        string defaultCategoryName = "Default";
        string[] moods = new string[]{
            "Excited",
            "Optimistic",
            "Engaged",
            "Proud",
            "Passionate",
            "Frustrated",
            "Disengaged",
            "Deflated",
            "Bored",
            "Worried"
        };


        public MyMoodDbConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(MyMoodDbContext db)
        {
            //this.SeedMoods(db);
            EventGenerator eg = new EventGenerator(db);
            Event ev = eg.GenerateNewEvent("NovartisTest", "Novartis Test");
            ev.ApplicationConfig.AppPassCode = new Guid("ABBA6130-9663-4FA4-D1D4-08CF435A7DE9");
            ev.ApplicationConfig.TimeZone = "Central Europe Standard Time";
            var adminUser = db.Users.FirstOrDefault(u => u.UserName == "admin");

            if (adminUser == null)
            {
                adminUser = new User("admin", "D1scov3r!", "admin@discover-corporation.com", "System Administrator", true, "System Administrator");
                db.Users.Add(adminUser);
            }

            adminUser.AddToRoles(UserRole.SystemAdmin);

            db.SaveChanges();
        }


        //protected void SeedMoods(MyMoodDbContext db)
        //{
        //    MoodCategory category = db.Categories.FirstOrDefault(c => c.Name.Equals(defaultCategoryName, StringComparison.InvariantCultureIgnoreCase));
        //    if (category == null)
        //    {
        //        category = new MoodCategory(new Guid(), defaultCategoryName);
        //        db.Add(category);
        //    }

        //    category.AddMood("Passionate", MoodType.Positive, "#f2932f", 1);
        //    category.AddMood("Excited", MoodType.Positive, "#f9a924", 2);
        //    category.AddMood("Proud", MoodType.Positive, "#fdbc18", 3);
        //    category.AddMood("Engaged", MoodType.Positive, "#ffcc03", 4);
        //    category.AddMood("Optimistic", MoodType.Positive, "#ffda00", 5);

        //    category.AddMood("Frustrated", MoodType.Negative, "#eb641c", 6);
        //    category.AddMood("Worried", MoodType.Negative, "#c85424", 7);
        //    category.AddMood("Bored", MoodType.Negative, "#a3442a", 8);
        //    category.AddMood("Deflated", MoodType.Negative, "#8e4c31", 9);
        //    category.AddMood("Disengaged", MoodType.Negative, "#775536", 10);

        //    //foreach(string mood in moods){
        //    //    if (!category.Moods.Any(m => m.Name == mood))
        //    //    {
        //    //        category.AddMood(mood, index);
        //    //    }
        //    //    index++;
        //    //}
        //    db.SaveChanges();
        //}
    }
}
