using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using Discover.DomainModel;

namespace MyMood.Configurator
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<Discover.DomainModel.IDomainDataContext>().Singleton().Use<MyMood.Infrastructure.EntityFramework.MyMoodDbContext>();
            });

            EventGenerator eg = new EventGenerator(ObjectFactory.GetInstance<IDomainDataContext>());
            eg.GenerateNewEvent("NovartisTest", "Novartis Test Event");

        }
    }
}
