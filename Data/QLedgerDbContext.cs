using Microsoft.EntityFrameworkCore;
using QLedger.Models.Entities;

namespace QLedger.Data
{
    public class QLedgerDbContext : DbContext
    {
        public QLedgerDbContext(DbContextOptions<QLedgerDbContext> options) : base(options)
        {
        }

        public DbSet<Line> Lines => Set<Line>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users => Set<User>();
        public DbSet<MachineChecklist> MachineChecklists => Set<MachineChecklist>();
        public DbSet<InspectionLedger> InspectionLedgers => Set<InspectionLedger>();
        public DbSet<NrsReport> NrsReports => Set<NrsReport>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Line
            modelBuilder.Entity<Line>(entity =>
            {
                entity.HasIndex(e => e.LineCode).IsUnique();
            });

            // 2. Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.ItemCode).IsUnique();
            });

            // 3. User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // 4. InspectionLedger
            modelBuilder.Entity<InspectionLedger>(entity =>
            {
                entity.HasOne(d => d.Line)
                    .WithMany(p => p.InspectionLedgers)
                    .HasForeignKey(d => d.LineId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.InspectionLedgers)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.PdPic)
                    .WithMany()
                    .HasForeignKey(d => d.PdPicId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.QaInspector)
                    .WithMany()
                    .HasForeignKey(d => d.QaInspectorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 5. NrsReport (1-to-1 with InspectionLedger)
            modelBuilder.Entity<NrsReport>(entity =>
            {
                entity.HasIndex(e => e.NrsNumber).IsUnique();

                entity.HasOne(d => d.Ledger)
                    .WithOne(p => p.NrsReport)
                    .HasForeignKey<NrsReport>(d => d.LedgerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.PicUser)
                    .WithMany()
                    .HasForeignKey(d => d.PicUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(d => d.ApprovedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SEED DATA
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Lines
            modelBuilder.Entity<Line>().HasData(
                new Line { LineId = 1, LineCode = "SW-01", LineName = "Switch Assembly 1", Area = "SWITCH 1", IsActive = true },
                new Line { LineId = 2, LineCode = "RL-01", LineName = "Relay Production Line 1", Area = "RELAY 1", IsActive = true },
                new Line { LineId = 3, LineCode = "RL-02", LineName = "Relay Production Line 2", Area = "RELAY 2", IsActive = true }
            );

            // Products
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, ItemCode = "G2R-1-24VDC", ItemName = "Power Relay G2R 24VDC", Category = "Power Relay", DefaultSamplingQty = 50, IsActive = true },
                new Product { ProductId = 2, ItemCode = "G6RL-1-12VDC", ItemName = "Slim Miniature Relay G6RL 12VDC", Category = "PCB Relay", DefaultSamplingQty = 50, IsActive = true },
                new Product { ProductId = 3, ItemCode = "A3U-301-RED", ItemName = "Pushbutton Switch A3U Series", Category = "Industrial Switch", DefaultSamplingQty = 30, IsActive = true },
                new Product { ProductId = 4, ItemCode = "LY2-0-24VDC", ItemName = "General Purpose Relay LY2", Category = "Power Relay", DefaultSamplingQty = 50, IsActive = true }
            );

            // Users
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Username = "budi", FullName = "Budi Santoso", Role = "PRODUCTION", RfidTag = "RFID-PD-001", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new User { UserId = 2, Username = "siti", FullName = "Siti Rahmawati", Role = "INSPECTOR", RfidTag = "RFID-QA-001", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new User { UserId = 3, Username = "joko", FullName = "Joko Widodo SPV", Role = "SUPERVISOR", RfidTag = "RFID-QA-SPV", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}

