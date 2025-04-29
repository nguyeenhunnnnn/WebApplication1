using WebApplication1.Models.Entities;

namespace WebApplication1.Models.ViewModels
{
    public class ChatViewModel
    {
        public string ReceiverId { get; set; }
        public string BaiDangId { get; set; }
        public List<TinNhan> Messages { get; set; }
        public List<TaiKhoanViewModel> Users { get; set; }
        public string CurrentUserId { get; set; }
        public BaiDang BaiDang { get; set; }
        public TaiKhoanViewModel ReceiverInfo { get; set; }
    }
}
