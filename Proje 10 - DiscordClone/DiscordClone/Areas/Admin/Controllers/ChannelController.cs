using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace DiscordClone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ChannelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChannelController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /Admin/Channel
        public IActionResult Index()
        {
            var channels = _unitOfWork.Channel.GetAll(includeProperties: "Server").ToList();
            return View(channels);
        }

        // GET: /Admin/Channel/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ServerList = _unitOfWork.Server.GetAll().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });

            return View(new Channel());
        }

        // POST: /Admin/Channel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Channel model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Channel.Add(model);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.ServerList = _unitOfWork.Server.GetAll().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });
            return View(model);
        }

        // GET: /Admin/Channel/Edit/id
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var channel = _unitOfWork.Channel.GetFirstOrDefault(c => c.Id == id);
            if (channel == null)
            {
                return NotFound();
            }

            ViewBag.ServerList = _unitOfWork.Server.GetAll().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString(),
                Selected = s.Id == channel.ServerId
            });

            return View(channel);
        }

        // POST: /Admin/Channel/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Channel model)
        {
            if (ModelState.IsValid)
            {
                var channel = _unitOfWork.Channel.GetFirstOrDefault(c => c.Id == model.Id);
                if (channel == null)
                {
                    return NotFound();
                }

                channel.Name = model.Name.Trim().ToLower().Replace(" ", "-");
                channel.Type = model.Type;
                channel.ServerId = model.ServerId;

                _unitOfWork.Channel.Update(channel);
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            ViewBag.ServerList = _unitOfWork.Server.GetAll().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });
            return View(model);
        }

        // GET: /Admin/Channel/Delete/id
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var channel = _unitOfWork.Channel.GetFirstOrDefault(c => c.Id == id, includeProperties: "Server");
            if (channel == null)
            {
                return NotFound();
            }

            return View(channel);
        }

        // POST: /Admin/Channel/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var channel = _unitOfWork.Channel.GetFirstOrDefault(c => c.Id == id);
            if (channel == null)
            {
                return NotFound();
            }

            _unitOfWork.Channel.Remove(channel);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
