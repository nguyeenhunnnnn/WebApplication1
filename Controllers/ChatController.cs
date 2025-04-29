using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entities;
using WebApplication1.Data;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;
using WebApplication1.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
namespace WebApplication1.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly ApplicationDbContext _context;
        public ChatController(IChatService chatService, UserManager<TaiKhoan> userManager, ApplicationDbContext context)
        {
            _chatService = chatService;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index(string receiverId, string baiDangId)
        {
            var currentUserId = _userManager.GetUserId(User);
           
            var messages = new List<TinNhan>();
            TaiKhoanViewModel receiverInfo = null;
            BaiDang baiDang = null;

            if (!string.IsNullOrEmpty(receiverId) && !string.IsNullOrEmpty(baiDangId))
            {
                messages = await _chatService.LayTinNhanTheoBaiDangAsync(currentUserId, receiverId, baiDangId);

                receiverInfo = await _userManager.Users
                    .Where(u => u.Id == receiverId)
                    .Select(u => new TaiKhoanViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        sFile_Avata_Path = u.FileAvata
                    })
                    .FirstOrDefaultAsync();

                if (int.TryParse(baiDangId, out int parsedBaiDangId))
                {
                    baiDang = await _context.BaiDangs.FindAsync(parsedBaiDangId);
                }
            }

            // 1. Kéo toàn bộ tin nhắn liên quan về RAM trước
            var conversations = await _context.TinNhans
                .Where(t => t.NguoiGuiId == currentUserId || t.NguoiNhanId == currentUserId)
                .Include(t => t.NguoiGui)
                .Include(t => t.NguoiNhan)
                .Include(t => t.BaiDang)
                .ToListAsync();

            // 2. Group và xử lý ở C# thuần
            var userConversations = conversations
                .Select(t => new
                {
                    UserId = t.NguoiGuiId == currentUserId ? t.NguoiNhanId : t.NguoiGuiId,
                    User = t.NguoiGuiId == currentUserId ? t.NguoiNhan : t.NguoiGui,
                    BaiDang = t.BaiDang
                })
                .Where(x => x.User != null && x.BaiDang != null)
                .GroupBy(x => new { x.UserId, BaiDangId = x.BaiDang.PK_iMaBaiDang })
                .Select(g => g.FirstOrDefault())
                .Select(x => new TaiKhoanViewModel
                {
                    Id = x.User.Id,
                    UserName = x.User.UserName,
                    sFile_Avata_Path = x.User.FileAvata,
                    BaiDangId = x.BaiDang.PK_iMaBaiDang.ToString(),
                    BaiDangTieuDe = x.BaiDang.sTieuDe
                })
                .ToList();

            var viewModel = new ChatViewModel
            {
                ReceiverId = receiverId,
                BaiDangId = baiDangId,
                Messages = messages,
                BaiDang = baiDang,
                CurrentUserId = currentUserId,
                Users = userConversations,
                ReceiverInfo = receiverInfo
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Trangchat(string receiverId, string baiDangId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var messages = new List<TinNhan>();
            TaiKhoanViewModel receiverInfo = null;
            BaiDang baiDang = null;

            if (!string.IsNullOrEmpty(receiverId) && !string.IsNullOrEmpty(baiDangId))
            {
                messages = await _chatService.LayTinNhanTheoBaiDangAsync(currentUserId, receiverId, baiDangId);

                var receiver = await _userManager.Users
                    .Where(u => u.Id == receiverId)
                    .Select(u => new TaiKhoanViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        sFile_Avata_Path = u.FileAvata
                    })
                    .FirstOrDefaultAsync();

                receiverInfo = receiver;

                if (int.TryParse(baiDangId, out int parsedBaiDangId))
                {
                    baiDang = await _context.BaiDangs.FindAsync(parsedBaiDangId);
                }
            }

            var userConversations = await _context.TinNhans
                .Where(t => t.NguoiGuiId == currentUserId || t.NguoiNhanId == currentUserId)
                .Select(t => new
                {
                    User = t.NguoiGuiId == currentUserId ? t.NguoiNhan : t.NguoiGui,
                    BaiDang = t.BaiDang
                })
                .Distinct()
                .Select(x => new TaiKhoanViewModel
                {
                    Id = x.User.Id,
                    UserName = x.User.UserName,
                    sFile_Avata_Path = x.User.FileAvata,
                    BaiDangId = x.BaiDang.PK_iMaBaiDang.ToString(), // Lấy ID bài đăng
                    BaiDangTieuDe = x.BaiDang.sTieuDe     // Lấy tiêu đề bài đăng
                })
                .ToListAsync();

            var viewModel = new ChatViewModel
            {
                ReceiverId = receiverId,
                BaiDangId = baiDangId,
                Messages = messages,
                BaiDang = baiDang,
                CurrentUserId = currentUserId,
                Users = userConversations,
                ReceiverInfo = receiverInfo
            };

            return View(viewModel);
        }


    }
}
