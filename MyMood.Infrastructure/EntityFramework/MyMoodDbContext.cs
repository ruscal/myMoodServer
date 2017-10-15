using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Discover.DomainModel;
using System.Linq.Expressions;
using MyMood.Domain;
using System.Data.Entity.ModelConfiguration.Conventions;
using Discover.Data.EntityFramework4;


namespace MyMood.Infrastructure.EntityFramework
{
    public class MyMoodDbContext : DbContext, IDomainDataContext
    {
        private static readonly ExpressionVisitor InterceptedSetUnwrapper = new Discover.Linq.InterceptingQueryUnwrapperExpressionVisitor(typeof(IDomainDataContext).GetMethod("Get"));
        private static readonly ExpressionVisitor EnumExpressionRewriter = new Discover.Linq.EnumRewriterExpressionVisitor("EnumValue");
        private static readonly ExpressionVisitor ReadOnlyEnumerableExpressionRewriter = new Discover.Linq.ReadOnlyEnumerableRewriterExpressionVisitor("Collection");

        private Dictionary<Type, IQueryable> InterceptedSets = new Dictionary<Type, IQueryable>();

        public IQueryable<T> Get<T>() where T : class, IEntity
        {
            if (!InterceptedSets.ContainsKey(typeof(T)))
            {
                InterceptedSets.Add(typeof(T), new Discover.Linq.InterceptingQuery<T>(Set<T>(), InterceptedSetUnwrapper, EnumExpressionRewriter, ReadOnlyEnumerableExpressionRewriter));
            }

            return (IQueryable<T>)InterceptedSets[typeof(T)];
        }

        public T Add<T>(T entity) where T : class, IEntity
        {
            return Set<T>().Add(entity);
        }

        public void Remove<T>(T entity) where T : class, IEntity
        {
            Set<T>().Remove(entity);
        }


        public IDbSet<Event> Events { get { return Set<Event>(); } }
        public IDbSet<MoodServer> MoodServers { get { return Set<MoodServer>(); } }
        public IDbSet<User> Users { get { return Set<User>(); } }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.ApplyDiscoverMappingConventionsToTypesDerivedFrom<Discover.Mail.MailMessage>();
            modelBuilder.ApplyDiscoverMappingConventionsToTypesDerivedFrom<Entity>();

            modelBuilder.Entity<User>().Property("RolesString").HasColumnName("Roles"); 
            modelBuilder.Entity<Event>().HasOptional(e => e.ApplicationConfig).WithRequired();
            modelBuilder.Entity<Event>().HasMany<Event, Activity>("ActivitiesCollection").WithRequired(p => p.Event).WillCascadeOnDelete();
            modelBuilder.Entity<Event>().HasMany<Event, RegisteredInterest>("RegisteredInterestsCollection").WithRequired(p => p.Event).WillCascadeOnDelete();
            modelBuilder.Entity<Event>().HasMany<Event, MoodCategory>("MoodCategoriesCollection").WithRequired(p => p.Event).WillCascadeOnDelete();
            modelBuilder.Entity<Event>().HasMany<Event, MoodPrompt>("MoodPromptsCollection").WithRequired(p => p.Event).WillCascadeOnDelete();
            modelBuilder.Entity<Event>().HasMany<Event, Responder>("RespondersCollection").WithRequired(p => p.Event).WillCascadeOnDelete();
        }

        public override int SaveChanges()
        {
            foreach (var o in this.ChangeTracker.Entries<Entity>().Where(e => e.State == System.Data.EntityState.Modified))
            {
                o.Entity.LastEditedDate = DateTime.UtcNow;
            }
            return base.SaveChanges();
        }

        public static void InitializeDatabase(bool force)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MyMoodDbContext, MyMoodDbConfiguration>());

            using (var db = new MyMoodDbContext())
            {
                db.Database.Initialize(force);
            }
        }
    }
}
