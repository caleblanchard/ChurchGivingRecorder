using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChurchGivingRecorder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChurchGivingRecorder.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Fund> Funds { get; set; }
        public DbSet<Giver> Givers { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<GiftDetail> GiftDetails { get; set; }

        public DbQuery<DepositView> DepositView { get; set; }

        private readonly IHttpContextAccessor _contextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor contextAccessor)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Query<DepositView>().ToView("DepositView");
        }

        public override int SaveChanges()
        {
            UpdateEnvelopeIdString();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateEnvelopeIdString();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateEnvelopeIdString()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is Giver && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                ((Giver)entity.Entity).EnvelopIdString = $"{((Giver)entity.Entity).EnvelopeID}";
            }
        }
    }
}
