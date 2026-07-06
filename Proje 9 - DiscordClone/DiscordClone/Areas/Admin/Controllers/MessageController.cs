using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DiscordClone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MessageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /Admin/Message
        public IActionResult Index()
        {
            var messages = _unitOfWork.Message.GetAll(includeProperties: "Sender,Channel,Channel.Server").ToList();
            return View(messages);
        }

        // GET: /Admin/Message/Delete/id
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var message = _unitOfWork.Message.GetFirstOrDefault(
                m => m.Id == id, 
                includeProperties: "Sender,Channel"
            );
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: /Admin/Message/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var message = _unitOfWork.Message.GetFirstOrDefault(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            _unitOfWork.Message.Remove(message);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
