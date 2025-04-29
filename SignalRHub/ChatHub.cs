using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entities;
using WebApplication1.Services;
using WebApplication1.Data;

namespace WebApplication1.SignalRHub
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ApplicationDbContext _context;
        public ChatHub(IChatService chatService, ApplicationDbContext context)
        {
            _chatService = chatService;
            _context = context;
        }

        public async Task SendMessage(string receiverId, string baiDangId, string message)
        {
            var senderId = Context.UserIdentifier;

            if (!int.TryParse(baiDangId, out int parsedBaiDangId))
            {
                // Nếu không thể chuyển đổi, có thể ném ngoại lệ hoặc trả về một danh sách trống.
                throw new ArgumentException("BaiDangId phải là một số hợp lệ."); // Hoặc ném ngoại lệ tùy vào yêu cầu.
            }

            // Kiểm tra xem gia sư có quyền tham gia cuộc trò chuyện này không
            var baiDang = await _context.BaiDangs
            .Include(b => b.UngTuyens)
            .FirstOrDefaultAsync(b => b.PK_iMaBaiDang == parsedBaiDangId);
            if (baiDang == null)
            {
                throw new InvalidOperationException("Bài đăng không tồn tại.");
            }
            Console.WriteLine("Danh sách ứng tuyển:");
            foreach (var ungTuyen in baiDang.UngTuyens)
            {
                Console.WriteLine($"Gia sư ID: {ungTuyen.FK_iMaTK_GiaSu}");
            }
            var giaSuDangKy = baiDang.UngTuyens.Any(g => g.FK_iMaTK_GiaSu == senderId); // Kiểm tra gia sư đã ứng tuyển chưa
            var phuHuynhCoQuyen = baiDang.FK_iMaTK == senderId; // Kiểm tra phụ huynh là người tạo bài đăng

            if (!giaSuDangKy && !phuHuynhCoQuyen)
            {
                Console.WriteLine($"Gia sư đã ứng tuyển: {giaSuDangKy}, Phụ huynh có quyền: {phuHuynhCoQuyen}");
                
                throw new UnauthorizedAccessException("Bạn không có quyền tham gia vào cuộc trò chuyện này.");
            }
            var tinNhan = new TinNhan
            {
                NguoiGuiId = senderId,
                NguoiNhanId = receiverId,
                BaiDangId = parsedBaiDangId,
                NoiDung = message,
                ThoiGianGui = DateTime.Now
            };

            await _chatService.LuuTinNhanAsync(tinNhan);

            await Clients.Users(senderId, receiverId)
                .SendAsync("ReceiveMessage", senderId, receiverId, baiDangId, message);
        }

    }

}
