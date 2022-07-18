using GoodsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace GoodsAPI.Data
{
    public class GoodsContext : DbContext
    {
        public GoodsContext(DbContextOptions<GoodsContext> options)
            : base(options)
        {
        }
        /************   Fix relationship behavior on delete and added seeding here        **********/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //PO
            modelBuilder.Entity<PurchaseOrders>().HasOne(p => p.BP).WithMany(p=> p.PO).HasForeignKey(p => p.BPCode).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<PurchaseOrders>().HasOne(p => p.Users1).WithMany(p=> p.PO1).HasForeignKey(p => p.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<PurchaseOrders>().HasOne(p => p.Users2).WithMany(p => p.PO2).HasForeignKey(p => p.LastUpdatedBy).OnDelete(DeleteBehavior.NoAction);
            //POL
            
            modelBuilder.Entity<PurchaseOrdersLines>().HasOne(p => p.items).WithOne().HasForeignKey<PurchaseOrdersLines>(p => p.ItemCode).OnDelete(DeleteBehavior.NoAction);
    
            //SO
            modelBuilder.Entity<SaleOrders>().HasOne(p => p.BP).WithMany(p => p.SO).HasForeignKey(p => p.BPCode).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SaleOrders>().HasOne(p => p.Users1).WithMany(p => p.SO1).HasForeignKey(p => p.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SaleOrders>().HasOne(p => p.Users2).WithMany(p => p.SO2).HasForeignKey(p => p.LastUpdatedBy).OnDelete(DeleteBehavior.NoAction);
            //SOL
            modelBuilder.Entity<SaleOrdersLines>().HasOne(p => p.items).WithOne().HasForeignKey<SaleOrdersLines>(p => p.ItemCode).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SaleOrdersLines>().HasOne(p => p.SaleOrders).WithOne().HasForeignKey<SaleOrdersLines>(p => p.DocID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<BusinessPartners>().HasOne(p => p.Btype).WithMany(p => p.BP).HasForeignKey(p => p.BPType).OnDelete(DeleteBehavior.NoAction);

            //BP
            modelBuilder.Entity<BusinessPartners>().HasOne(p => p.Btype).WithMany(p => p.BP).HasForeignKey(p=> p.BPType).OnDelete(DeleteBehavior.NoAction);
            //Seeding
            modelBuilder.Entity<Users>().HasData(new Users { ID=1,FullName="name1", UserName="U1",Password="P1" ,Active= true });
            modelBuilder.Entity<Users>().HasData(new Users { ID = 2, FullName = "name2", UserName = "U2", Password = "P2", Active = false });
            modelBuilder.Entity<BPType>().HasData(new BPType { TypeCode = 'C', TypeName = "Customer" });
            modelBuilder.Entity<BPType>().HasData(new BPType { TypeCode = 'V', TypeName = "Vendor" });
            modelBuilder.Entity<BusinessPartners>().HasData(new BusinessPartners { BPCode = "C0001", BPName = "Customer 1",BPType = 'C' , Active=true });
            modelBuilder.Entity<BusinessPartners>().HasData(new BusinessPartners { BPCode = "C0002", BPName = "Customer 2", BPType = 'C', Active = false });
            modelBuilder.Entity<BusinessPartners>().HasData(new BusinessPartners { BPCode = "V0001", BPName = "Vendor 1", BPType = 'V', Active = true });
            modelBuilder.Entity<BusinessPartners>().HasData(new BusinessPartners { BPCode = "V0002", BPName = "Vendor 2", BPType = 'V', Active = false });
            modelBuilder.Entity<Items>().HasData(new Items { ItemsCode = "Itm1", ItemName = "Item 1" , Active = true });
            modelBuilder.Entity<Items>().HasData(new Items { ItemsCode = "Itm2", ItemName = "Item 2", Active = true });
            modelBuilder.Entity<Items>().HasData(new Items { ItemsCode = "Itm3", ItemName = "Item 3", Active = false });




        }

        //setting models to DB
        public DbSet<Users> Users => Set<Users>();
        public DbSet<BPType> BPType => Set<BPType>();

        public DbSet<BusinessPartners> BusinessPartners => Set<BusinessPartners>();
        public DbSet<Items> Items => Set<Items>();
        public DbSet<SaleOrders> SaleOrders => Set<SaleOrders>();
        public DbSet<SaleOrdersLines> SaleOrdersLines => Set<SaleOrdersLines>();
        public DbSet<SaleOrdersLinesComments> SaleOrdersLinesComments => Set<SaleOrdersLinesComments>();

        public DbSet<PurchaseOrders> PurchaseOrders => Set<PurchaseOrders>();

        public DbSet<PurchaseOrdersLines> PurchaseOrdersLines => Set<PurchaseOrdersLines>();




    }
}