using System;
using System.Collections.Generic;
 using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Data;
using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories
{
    public interface IChatRepository
    {
        Task<List<TinNhan>> LayTinNhanGiuaHaiNguoi(string nguoi1, string nguoi2);
        Task LuuTinNhan(TinNhan tinNhan);
        Task<List<TinNhan>> LayTinNhanTheoBaiDang(string nguoi1, string nguoi2, string baiDangId);
    }
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TinNhan>> LayTinNhanGiuaHaiNguoi(string nguoi1, string nguoi2)
        {
            return await _context.TinNhans
                .Where(t => (t.NguoiGuiId == nguoi1 && t.NguoiNhanId == nguoi2) ||
                            (t.NguoiGuiId == nguoi2 && t.NguoiNhanId == nguoi1))
                .OrderBy(t => t.ThoiGianGui)
                .Include(t => t.NguoiGui)
                .Include(t => t.NguoiNhan)
                .ToListAsync();
        }

        public async Task LuuTinNhan(TinNhan tinNhan)
        {
            _context.TinNhans.Add(tinNhan);
            await _context.SaveChangesAsync();
        }
        public async Task<List<TinNhan>> LayTinNhanTheoBaiDang(string nguoi1, string nguoi2, string baiDangId)
        {
            if (string.IsNullOrEmpty(nguoi1) || string.IsNullOrEmpty(nguoi2) || !int.TryParse(baiDangId, out int parsedBaiDangId))
            {
                return new List<TinNhan>(); // hoặc throw exception tùy bạn
            }

            return await _context.TinNhans
                .Where(t => t.BaiDangId == parsedBaiDangId &&
                            ((t.NguoiGuiId == nguoi1 && t.NguoiNhanId == nguoi2) ||
                             (t.NguoiGuiId == nguoi2 && t.NguoiNhanId == nguoi1)))
                .OrderBy(t => t.ThoiGianGui)
                .Include(t => t.NguoiGui)
                .ToListAsync();

        }
    }

}
