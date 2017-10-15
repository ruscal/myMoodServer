using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.DataSeeder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();

            MoodResponseDataSeeder seeder = new MoodResponseDataSeeder();
            seeder.SeedData(DateTime.Now.Date, DateTime.Now, 5, 700);

        }
    }
}
