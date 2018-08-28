using iCoursework.Data;
using iCoursework.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace iCoursework.Controllers
{
    public class InstructionController : Controller
    {
        private readonly ApplicationDbContext _instructionDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _appEnvironment;

        public InstructionController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment appEnvironment)
        {
            _instructionDbContext = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }


        [HttpGet]
        public IActionResult InstructionList(string id)
        {
            ViewBag.CurrentUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            return View(id == null ?
                _instructionDbContext.Instructions
                    .Include(i => i.Steps).Include(i => i.Category).Include(i => i.Comments).ThenInclude(i => i.Likes).ThenInclude(l => l.User).Include(i => i.Author)
                    .ToList() :
                _instructionDbContext.Instructions
                    .Include(i => i.Steps).Include(i => i.Category).Include(i => i.Comments).ThenInclude(i => i.Likes).ThenInclude(l => l.User).Include(i => i.Author)
                    .Where(i => i.Category.Id == id).ToList());
        }

        [HttpGet]
        public IActionResult Instruction(string id)
        {
            return View(_instructionDbContext.Instructions
                .Include(i => i.Steps).Include(i => i.Category)
                .Single(i => i.Id == id));
        }

        [HttpGet]
        public IActionResult AddInstruction()
        {
            SelectList categories = new SelectList(_instructionDbContext.Categories, "Id", "Name");
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public IActionResult AddInstruction(Instruction instruction, string categoryId)
        {
            if (!ModelState.IsValid) return View(instruction);
            instruction.Time = DateTime.Now;
            instruction.Author = _userManager.GetUserAsync(HttpContext.User).Result;
            instruction.Category = _instructionDbContext.Categories.Find(categoryId);
            _instructionDbContext.Add(instruction);
            _instructionDbContext.SaveChanges();
            TempData["Instruction"] = instruction.Id;
            return RedirectToAction("AddSteps");
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            _instructionDbContext.Add(category);
            _instructionDbContext.SaveChanges();
            return RedirectToAction("CreateCategory");
        }

        [HttpGet]
        public IActionResult AddSteps()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RateInstruction(string instructionId, string rate)
        {
            var newRate = Convert.ToInt32(rate);
            var instruction = _instructionDbContext.Instructions.Find(instructionId);
            instruction.Rating = ((instruction.Rating * instruction.RatesCount++) + newRate) / instruction.RatesCount;
            _instructionDbContext.SaveChanges();
            return RedirectToAction("InstructionList");
        }

        [HttpPost]
        public IActionResult LikeComment(string commentId, string userId)
        {
            var comment = _instructionDbContext.InstructionComments
                .Include(c => c.Likes)
                .Single(c => c.Id == commentId);
            var isThereCurrentUser = false;
            var likes = _instructionDbContext.CommentLikes
                .Include(l => l.User).Where(l => l.Comment.Id == commentId)
                .ToList();
            foreach (var like in likes)
            {
                if (like.User.Id == userId)
                {
                    _instructionDbContext.Remove(like);
                    isThereCurrentUser = true;
                }
            }
            var newLike = new Like { User = _userManager.GetUserAsync(HttpContext.User).Result };
            if (!isThereCurrentUser) comment.Likes.Add(newLike);
            _instructionDbContext.SaveChanges();

            return RedirectToAction("InstructionList");
        }

        [HttpPost]
        public IActionResult DeleteComment(string commentId)
        {
            var comment = _instructionDbContext.InstructionComments.Include(c => c.Likes).Single(c => c.Id == commentId);
            foreach (var like in comment.Likes)
            {
                like.User = null;
                _instructionDbContext.CommentLikes.Remove(like);
            }
            _instructionDbContext.InstructionComments.Remove(comment);
            _instructionDbContext.SaveChanges();
            return RedirectToAction("InstructionList");
        }

        [HttpPost]
        public IActionResult AddSteps(Step step)
        {
            if (!ModelState.IsValid) return View(step);
            var instructionId = (string)TempData["Instruction"];
            var instruction = _instructionDbContext.Instructions.Include(i => i.Steps).Single(i => i.Id == instructionId);
            step.Index = instruction.Steps.Count;
            instruction.Steps.Add(step);
            _instructionDbContext.SaveChanges();
            return RedirectToAction("AddSteps");
        }

        [HttpGet]
        public IActionResult FinishAddInstruction()
        {
            return View(_instructionDbContext.Categories.ToList());
        }

        [HttpPost]
        public IActionResult FinishAddInstruction(Category category)
        {
            var x = category;
            return View("InstructionList");
        }

        [HttpPost]
        public IActionResult AddComment(Comment comment, string instructionId)
        {
            comment.Author = _userManager.GetUserAsync(HttpContext.User).Result;
            comment.Time = DateTime.Now;
            _instructionDbContext.Instructions.Find(instructionId).Comments.Add(comment);
            _instructionDbContext.SaveChanges();
            return RedirectToAction("InstructionList");
        }
    }
}