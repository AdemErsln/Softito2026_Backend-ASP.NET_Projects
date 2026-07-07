using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DiscordClone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /Admin/Server
        public IActionResult Index()
        {
            var servers = _unitOfWork.Server.GetAll(includeProperties: "Owner").ToList();
            return View(servers);
        }

        // GET: /Admin/Server/Edit/id
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var server = _unitOfWork.Server.GetFirstOrDefault(s => s.Id == id, includeProperties: "Owner");
            if (server == null)
            {
                return NotFound();
            }

            return View(server);
        }

        // POST: /Admin/Server/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Server model)
        {
            if (ModelState.IsValid)
            {
                var server = _unitOfWork.Server.GetFirstOrDefault(s => s.Id == model.Id);
                if (server == null)
                {
                    return NotFound();
                }

                server.Name = model.Name;
                server.Description = model.Description;
                server.InvitationCode = model.InvitationCode;

                _unitOfWork.Server.Update(server);
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: /Admin/Server/Delete/id
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var server = _unitOfWork.Server.GetFirstOrDefault(s => s.Id == id, includeProperties: "Owner");
            if (server == null)
            {
                return NotFound();
            }

            return View(server);
        }

        // POST: /Admin/Server/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var server = _unitOfWork.Server.GetFirstOrDefault(s => s.Id == id);
            if (server == null)
            {
                return NotFound();
            }

            _unitOfWork.Server.Remove(server);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
