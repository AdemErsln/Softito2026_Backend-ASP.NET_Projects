using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DiscordClone.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ChatController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: /Chat/Index
        public IActionResult Index(int? serverId, int? channelId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Get all servers the user is a member of
            var memberships = _unitOfWork.ServerMember.GetAll(
                sm => sm.UserId == userId, 
                includeProperties: "Server"
            );

            var userServers = memberships.Select(m => m.Server).Where(s => s != null).ToList();

            var viewModel = new ChatVM
            {
                UserServers = userServers!,
                CurrentUserId = userId
            };

            // If there is an active server list, select active server
            if (userServers.Any())
            {
                var activeServer = serverId.HasValue 
                    ? userServers.FirstOrDefault(s => s!.Id == serverId.Value) 
                    : userServers.First();

                if (activeServer != null)
                {
                    viewModel.ActiveServer = activeServer;

                    // Get channels for active server
                    var channels = _unitOfWork.Channel.GetAll(c => c.ServerId == activeServer.Id).ToList();
                    viewModel.Channels = channels;

                    if (channels.Any())
                    {
                        var activeChannel = channelId.HasValue
                            ? channels.FirstOrDefault(c => c.Id == channelId.Value)
                            : channels.FirstOrDefault(c => c.Type == ChannelType.Text);

                        if (activeChannel != null)
                        {
                            viewModel.ActiveChannel = activeChannel;

                            // Load messages for active channel
                            viewModel.Messages = _unitOfWork.Message.GetAll(
                                m => m.ChannelId == activeChannel.Id,
                                includeProperties: "Sender"
                            ).OrderBy(m => m.SendDate).ToList();
                        }
                    }

                    // Load members for active server
                    viewModel.Members = _unitOfWork.ServerMember.GetAll(
                        sm => sm.ServerId == activeServer.Id,
                        includeProperties: "User"
                    ).ToList();
                }
            }

            return View(viewModel);
        }

        // POST: /Chat/CreateServer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateServer(string name, IFormFile? file)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index");
            }

            var server = new Server
                {
                    Name = name,
                    OwnerId = userId,
                    InvitationCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                };

            // Image Upload Logic
            if (file != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                var uploadRoot = Path.Combine(wwwRootPath, "img", "serverpics");
                var extension = Path.GetExtension(file.FileName);

                if (!Directory.Exists(uploadRoot))
                {
                    Directory.CreateDirectory(uploadRoot);
                }

                using (var fileStream = new FileStream(Path.Combine(uploadRoot, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                server.Photo = "/img/serverpics/" + fileName + extension;
            }
            else
            {
                server.Photo = "https://cdn.discordapp.com/embed/avatars/0.png"; // Default placeholder
            }

            // Save Server
            _unitOfWork.Server.Add(server);
            _unitOfWork.Save(); // Save to generate Server.Id

            // Automatically join creator as Owner
            var member = new ServerMember
            {
                ServerId = server.Id,
                UserId = userId,
                ServerRole = "Owner"
            };
            _unitOfWork.ServerMember.Add(member);

            // Automatically create default text channel "# genel"
            var defaultChannel = new Channel
            {
                Name = "genel",
                Type = ChannelType.Text,
                ServerId = server.Id
            };
            _unitOfWork.Channel.Add(defaultChannel);

            _unitOfWork.Save();

            return RedirectToAction("Index", new { serverId = server.Id, channelId = defaultChannel.Id });
        }

        // POST: /Chat/JoinServer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult JoinServer(string inviteCode)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(inviteCode))
            {
                return RedirectToAction("Index");
            }

            var server = _unitOfWork.Server.GetFirstOrDefault(s => s.InvitationCode == inviteCode.Trim().ToUpper());
            if (server == null)
            {
                TempData["Error"] = "Geçersiz davet kodu.";
                return RedirectToAction("Index");
            }

            // Check if already a member
            var existingMember = _unitOfWork.ServerMember.GetFirstOrDefault(
                sm => sm.ServerId == server.Id && sm.UserId == userId
            );

            if (existingMember == null)
            {
                var member = new ServerMember
                {
                    ServerId = server.Id,
                    UserId = userId,
                    ServerRole = "Member"
                };
                _unitOfWork.ServerMember.Add(member);
                _unitOfWork.Save();
            }

            // Find default channel
            var firstChannel = _unitOfWork.Channel.GetAll(c => c.ServerId == server.Id).FirstOrDefault();

            return RedirectToAction("Index", new { serverId = server.Id, channelId = firstChannel?.Id });
        }

        // POST: /Chat/CreateChannel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateChannel(int serverId, string name, ChannelType type)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", new { serverId });
            }

            // Verify membership & role
            var member = _unitOfWork.ServerMember.GetFirstOrDefault(
                sm => sm.ServerId == serverId && sm.UserId == userId
            );

            if (member == null || (member.ServerRole != "Owner" && member.ServerRole != "Admin"))
            {
                TempData["Error"] = "Sadece sunucu yöneticileri kanal açabilir.";
                return RedirectToAction("Index", new { serverId });
            }

            var channel = new Channel
            {
                Name = name.Trim().ToLower().Replace(" ", "-"),
                Type = type,
                ServerId = serverId
            };

            _unitOfWork.Channel.Add(channel);
            _unitOfWork.Save();

            return RedirectToAction("Index", new { serverId, channelId = channel.Id });
        }

        // POST: /Chat/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMessage(int channelId, string content)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(content))
            {
                return BadRequest();
            }

            var channel = _unitOfWork.Channel.GetFirstOrDefault(c => c.Id == channelId);
            if (channel == null)
            {
                return NotFound();
            }

            // Verify membership
            var member = _unitOfWork.ServerMember.GetFirstOrDefault(
                sm => sm.ServerId == channel.ServerId && sm.UserId == userId
            );
            if (member == null)
            {
                return Forbid();
            }

            var message = new Message
            {
                Content = content,
                ChannelId = channelId,
                SenderId = userId,
                SendDate = DateTime.UtcNow
            };

            _unitOfWork.Message.Add(message);
            _unitOfWork.Save();

            return RedirectToAction("Index", new { serverId = channel.ServerId, channelId });
        }
    }
}
