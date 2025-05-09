using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApplication1.Models.Entities
{
    public class TaiKhoan : IdentityUser
    {

        [StringLength(500)]
        public string? DiaChi { get; set; }

        public string? FileAvata { get; set; }
        public string? FileCCCD { get; set; }

        [Required]
        [StringLength(30)]
        public string VaiTro { get; set; } 

        [Required]
        [StringLength(30)]
        public string TrangThai { get; set; }

        [Required]
        public string CCCD { get; set; }
        // Quan hệ một-nhiều với BaiDang
        public List<BaiDang>? BaiDangs { get; set; } = new List<BaiDang>();

        // Quan hệ một-nhiều với HoSo
        public List<HoSo>? HoSos { get; set; } = new List<HoSo>();
        // Quan hệ một-nhiều với UngTuyen
        public List<UngTuyen> UngTuyens { get; set; } = new List<UngTuyen>();

        //quan hẹ vơi chat 
        public virtual ICollection<TinNhan> TinNhanGui { get; set; } = new List<TinNhan>();
        public virtual ICollection<TinNhan> TinNhanNhan { get; set; } = new List<TinNhan>();

        //
        // 💬 Danh sách đánh giá phụ huynh đã viết
        public virtual ICollection<DanhGiaGiaSu> DanhGiaDaViet { get; set; } = new List<DanhGiaGiaSu>();

        // ⭐ Danh sách đánh giá gia sư đã nhận
        public virtual ICollection<DanhGiaGiaSu> DanhGiaNhanDuoc { get; set; } = new List<DanhGiaGiaSu>();

        //thanh toán
        public bool GoiCuoc { get; set; } = false;
        public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();


    }
}
