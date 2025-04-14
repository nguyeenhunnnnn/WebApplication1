using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entities;
namespace WebApplication1.Data
{
    public class ApplicationDbContext :IdentityDbContext<TaiKhoan>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
      //  public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<BaiDang> BaiDangs { get; set; } 
        public DbSet<HoSo> HoSos { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoan");
            //modelBuilder.Entity<TaiKhoan>().Property(x => x.Id).HasColumnName("Id");
            //modelBuilder.Entity<TaiKhoan>().Property(x => x.UserName).HasColumnName("UserName");
            //modelBuilder.Entity<TaiKhoan>().Property(x => x.Email).HasColumnName("Email");
            //modelBuilder.Entity<TaiKhoan>().Property(x => x.PasswordHash).HasColumnName("PasswordHash");
            modelBuilder.Entity<TaiKhoan>(entity =>
            {
              

                // Cấu hình mối quan hệ giữa TaiKhoan và HoSo
                entity.HasMany(tk => tk.HoSos)
                      .WithOne(hs => hs.TaiKhoan)
                      .HasForeignKey(hs => hs.FK_iMaTK)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // Cấu hình Fluent API cho bảng BaiDang
            modelBuilder.Entity<BaiDang>(entity =>
            {
                entity.HasKey(bd => bd.PK_iMaBaiDang); // Primary Key
                entity.Property(bd => bd.PK_iMaBaiDang).ValueGeneratedOnAdd(); // Tự động tăng
                entity.Property(bd => bd.sTieuDe).IsRequired().HasMaxLength(255); // Tiêu đề
                entity.Property(bd => bd.sMoTa).HasColumnType("NVARCHAR(MAX)"); // Mô tả (nullable)
                entity.Property(bd => bd.sDiaDiem).HasMaxLength(255); // Địa điểm
                entity.Property(bd => bd.fMucLuong).HasColumnType("DECIMAL(10,2)"); // Mức lương
                entity.Property(bd => bd.sTrangThai).HasMaxLength(20); // Trạng thái
                entity.Property(bd => bd.dNgayTao).HasDefaultValueSql("GETDATE()"); // Ngày tạo (mặc định là ngày hiện tại)

                // Cấu hình các mối quan hệ
                entity.HasOne(bd => bd.TaiKhoan)
                    .WithMany(tk => tk.BaiDangs)
                    .HasForeignKey(bd => bd.FK_iMaTK)
                    .OnDelete(DeleteBehavior.Restrict); // Xóa bài đăng khi xóa tài khoản

                entity.HasOne(bd => bd.HoSo)
                    .WithMany(hs => hs.BaiDangs)
                    .HasForeignKey(bd => bd.FK_iMaHS)
                    .OnDelete(DeleteBehavior.SetNull); // không xoá bài đăng khi xóa hồ sơ
            });

            // Cấu hình Fluent API cho bảng HoSo
            modelBuilder.Entity<HoSo>(entity =>
            {
                entity.HasKey(hs => hs.iMaHS); // Primary Key
                entity.Property(hs => hs.iMaHS).ValueGeneratedOnAdd(); // Tự động tăng
                entity.Property(hs => hs.sTieuDe).IsRequired().HasMaxLength(255); // Tiêu đề
                entity.Property(hs => hs.sBangCap).IsRequired().HasMaxLength(255); // Bằng cấp
                entity.Property(hs => hs.sKyNang).IsRequired(); // Kỹ năng
                entity.Property(hs => hs.sDuongDanTep).HasMaxLength(255); // Đường dẫn tệp (nullable)

                // Cấu hình mối quan hệ với bảng TaiKhoan
                entity.HasOne(hs => hs.TaiKhoan)
                    .WithMany(tk => tk.HoSos)
                    .HasForeignKey(hs => hs.FK_iMaTK)
                    .OnDelete(DeleteBehavior.Cascade); // Xóa hồ sơ khi xóa tài khoản
            });
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}
