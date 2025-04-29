using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Data;
using WebApplication1.Models.Entities;

namespace WebApplication1.Services
{
    public interface IChatService
    {
        Task<List<TinNhan>> LayTinNhanAsync(string userId, string receiverId);
        Task LuuTinNhanAsync(TinNhan tinNhan);
        Task<List<TinNhan>> LayTinNhanTheoBaiDangAsync(string userId, string receiverId, string baiDangId);
    }
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepo;

        public ChatService(IChatRepository chatRepo)
        {
            _chatRepo = chatRepo;
        }

        public Task<List<TinNhan>> LayTinNhanAsync(string userId, string receiverId)
        {
            return _chatRepo.LayTinNhanGiuaHaiNguoi(userId, receiverId);
        }

        public Task LuuTinNhanAsync(TinNhan tinNhan)
        {
            return _chatRepo.LuuTinNhan(tinNhan);
        }
        public Task<List<TinNhan>> LayTinNhanTheoBaiDangAsync(string userId, string receiverId, string baiDangId)
        {
            return _chatRepo.LayTinNhanTheoBaiDang(userId, receiverId, baiDangId);
        }
    }

}
