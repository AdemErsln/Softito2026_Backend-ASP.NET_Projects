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
    public class FriendshipController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public FriendshipController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: /Admin/Friendship
        public IActionResult Index()
        {
            var friendships = _unitOfWork.Friendship.GetAll(includeProperties: "Sender,Receiver").ToList();
            return View(friendships);
        }

        // GET: /Admin/Friendship/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.UserList = _userManager.Users.Select(u => new SelectListItem
            {
                Text = $"{u.UserName} ({u.Email})",
                Value = u.Id
            });

            return View(new Friendship());
        }

        // POST: /Admin/Friendship/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Friendship model)
        {
            if (ModelState.IsValid)
            {
                if (model.SenderId == model.ReceiverId)
                {
                    ModelState.AddModelError("", "Bir kullanıcı kendisiyle arkadaş olamaz.");
                }
                else
                {
                    // Check if exists
                    var existing = _unitOfWork.Friendship.GetFirstOrDefault(
                        f => (f.SenderId == model.SenderId && f.ReceiverId == model.ReceiverId) ||
                             (f.SenderId == model.ReceiverId && f.ReceiverId == model.SenderId)
                    );

                    if (existing != null)
                    {
                        ModelState.AddModelError("", "Bu iki kullanıcı arasında zaten bir ilişki tanımlı.");
                    }
                    else
                    {
                        _unitOfWork.Friendship.Add(model);
                        _unitOfWork.Save();
                        return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.UserList = _userManager.Users.Select(u => new SelectListItem
            {
                Text = $"{u.UserName} ({u.Email})",
                Value = u.Id
            });

            return View(model);
        }

        // GET: /Admin/Friendship/Edit/id
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var friendship = _unitOfWork.Friendship.GetFirstOrDefault(
                f => f.Id == id, 
                includeProperties: "Sender,Receiver"
            );

            if (friendship == null)
            {
                return NotFound();
            }

            return View(friendship);
        }

        // POST: /Admin/Friendship/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Friendship model)
        {
            if (ModelState.IsValid)
            {
                var friendship = _unitOfWork.Friendship.GetFirstOrDefault(f => f.Id == model.Id);
                if (friendship == null)
                {
                    return NotFound();
                }

                friendship.Status = model.Status;

                _unitOfWork.Friendship.Update(friendship);
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: /Admin/Friendship/Delete/id
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var friendship = _unitOfWork.Friendship.GetFirstOrDefault(
                f => f.Id == id, 
                includeProperties: "Sender,Receiver"
            );

            if (friendship == null)
            {
                return NotFound();
            }

            return View(friendship);
        }

        // POST: /Admin/Friendship/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var friendship = _unitOfWork.Friendship.GetFirstOrDefault(f => f.Id == id);
            if (friendship == null)
            {
                return NotFound();
            }

            _unitOfWork.Friendship.Remove(friendship);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
