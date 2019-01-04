using System;
using System.Collections.Generic;
using System.Text;
using ChurchGivingRecorder.Models;
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

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Query<DepositView>().ToView("DepositView");
        }
    }
}
