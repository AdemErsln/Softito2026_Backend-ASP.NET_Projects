using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace DiscordClone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServerMemberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public ServerMemberController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: /Admin/ServerMember
        public IActionResult Index()
        {
            var members = _unitOfWork.ServerMember.GetAll(includeProperties: "Server,User").ToList();
            return View(members);
        }

        // GET: /Admin/ServerMember/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ServerList = _unitOfWork.Server.GetAll().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });

            ViewBag.UserList = _userManager.Users.Select(u => new SelectListItem
            {
                Text = $"{u.UserName} ({u.Email})",
                Value = u.Id
            });

            return View(new ServerMember());
        }

        // POST: /Admin/ServerMember/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServerMember model)
        {
            if (ModelState.IsValid)
            {
                // Check if already a member
                var existing = _unitOfWork.ServerMember.GetFirstOrDefault(
                    sm => sm.ServerId == model.ServerId && sm.UserId == model.UserId
                );

                if (existing != null)
                {
                    ModelState.AddModelError("", "Bu kullanıcı zaten bu sunucunun üyesidir.");
                }
                else
                {
                    _unitOfWork.ServerMember.Add(model);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.ServerList = _unitOfWork.Server.GetAll().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });

            ViewBag.UserList = _userManager.Users.Select(u => new SelectListItem
            {
                Text = $"{u.UserName} ({u.Email})",
                Value = u.Id
            });

            return View(model);
        }

        // GET: /Admin/ServerMember/Delete
        [HttpGet]
        public IActionResult Delete(int serverId, string userId)
        {
            if (serverId <= 0 || string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var member = _unitOfWork.ServerMember.GetFirstOrDefault(
                sm => sm.ServerId == serverId && sm.UserId == userId,
                includeProperties: "Server,User"
            );

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: /Admin/ServerMember/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int serverId, string userId)
        {
            var member = _unitOfWork.ServerMember.GetFirstOrDefault(
                sm => sm.ServerId == serverId && sm.UserId == userId
            );

            if (member == null)
            {
                return NotFound();
            }

            _unitOfWork.ServerMember.Remove(member);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
