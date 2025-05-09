using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using WebApplication1.Models.Entities;
namespace WebApplication1.Models.ViewModels
{
    public class BaiDangViewModel
    {
        public int PK_iMaBaiDang { get; set; }
        public string sTieuDe { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string sMoTa { get; set; }

        public string sDiaDiem { get; set; }
        public decimal fMucLuong { get; set; }
        public string sTrangThai { get; set; } = null!;

        public DateTime dNgayTao { get; set; }
        public DateTime dThoiGianHetHan { get; set; } // Tùy chọn

        public string Nguoitao { get; set; } 
        //cột mới 
        public string sMonday { get; set; }
        public string sYCau { get; set; }
        public string sGioiTinh { get; set; }
        public string sTuoi { get; set; }
        public string sKinhNghiem { get; set; }
        public string sBangCap { get; set; }
        public string sfileAvata { get; set; }
        public string? Vaitro { get; set; } 
        public string? FileCVPath { get; set; }

        //// lien ke voi thanh toan
        public string sTrangThaiGD { get; set; }
        public DateTime dUuTienDen { get; set; }

        public List<DanhGiaGiaSu> DanhGias { get; set; } = new();
        public double DiemTrungBinh { get; set; }
        public int TongDanhGia => DanhGias.Count;

        public int SoSao1 => DanhGias.Count(d => d.SoSao == 1);
        public int SoSao2 => DanhGias.Count(d => d.SoSao == 2);
        public int SoSao3 => DanhGias.Count(d => d.SoSao == 3);
        public int SoSao4 => DanhGias.Count(d => d.SoSao == 4);
        public int SoSao5 => DanhGias.Count(d => d.SoSao == 5);


        
    }
}
