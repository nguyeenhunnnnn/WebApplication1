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
        public DbSet<UngTuyen> UngTuyen { get; set; }
        public DbSet<TinNhan> TinNhans { get; set; }
        public DbSet<DanhGiaGiaSu> DanhGiaGiaSus { get; set; }
        public DbSet<GoiDichVu> GoiDichVus { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }

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

            // Cau hinh cho bang ungtuyen 
                    modelBuilder.Entity<UngTuyen>()
            .HasOne(ut => ut.TaiKhoanGiaSu)
            .WithMany(tk => tk.UngTuyens)
            .HasForeignKey(ut => ut.FK_iMaTK_GiaSu)
            .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ giữa UngTuyen và HoSo
            modelBuilder.Entity<UngTuyen>()
                .HasOne(u => u.HoSo)
                .WithMany(h => h.UngTuyens)
                .HasForeignKey(u => u.FK_iMaHS)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa khi HoSo bị xóa

            // Quan hệ giữa UngTuyen và BaiDang
            modelBuilder.Entity<UngTuyen>()
                .HasOne(u => u.BaiDang)
                .WithMany(b => b.UngTuyens)
                .HasForeignKey(u => u.FK_iMaBaiDang)
                .OnDelete(DeleteBehavior.Restrict);// không xoá khi bài đăng bị xoá 

            // goi dich vu
            modelBuilder.Entity<GoiDichVu>(entity =>
            {
                entity.ToTable("tbl_GoiDichVu");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TenGoi).IsRequired().HasMaxLength(255);
                entity.Property(e => e.MoTa).HasMaxLength(255);
                entity.Property(e => e.Gia).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SoNgayHieuLuc).IsRequired();
            });

            // Thanh toan
            // Cấu hình bảng ThanhToan
            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.ToTable("tbl_ThanhToan");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NgayThanhToan).IsRequired();

                entity.HasOne(e => e.TaiKhoan)
                      .WithMany(t => t.ThanhToans)
                      .HasForeignKey(e => e.TaiKhoanId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.GoiDichVu)
                      .WithMany(g => g.ThanhToans)
                      .HasForeignKey(e => e.GoiDichVuId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BaiDang)
                      .WithMany(b => b.ThanhToans)
                      .HasForeignKey(e => e.BaiDangId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // chat 
            modelBuilder.Entity<TinNhan>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Tự động tăng
                entity.HasOne(e => e.NguoiGui)
                      .WithMany(u => u.TinNhanGui)
                      .HasForeignKey(e => e.NguoiGuiId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NguoiNhan)
                      .WithMany(u => u.TinNhanNhan)
                      .HasForeignKey(e => e.NguoiNhanId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BaiDang)
                      .WithMany(b => b.TinNhans)
                      .HasForeignKey(e => e.BaiDangId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            //
            // Quan hệ DanhGiaGiaSu - NguoiDanhGia (Phụ huynh)
            modelBuilder.Entity<DanhGiaGiaSu>()
                .HasOne(d => d.NguoiDanhGia)
                .WithMany(t => t.DanhGiaDaViet)
                .HasForeignKey(d => d.NguoiDanhGiaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ DanhGiaGiaSu - GiaSu
            modelBuilder.Entity<DanhGiaGiaSu>()
                .HasOne(d => d.GiaSu)
                .WithMany(t => t.DanhGiaNhanDuoc)
                .HasForeignKey(d => d.GiaSuId)
                .OnDelete(DeleteBehavior.Restrict);

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
