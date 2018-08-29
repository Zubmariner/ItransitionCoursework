using iCoursework.Data;
using iCoursework.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        public IActionResult InstructionList()
        {
            var emptyInstructionsList = _instructionDbContext.Instructions.Where(i => i.Steps.Count == 0).ToList();
            if (emptyInstructionsList.Count > 0)
            {
                foreach (var instruction in emptyInstructionsList)
                {
                    instruction.Steps.Add(new Step { Image = "What's the problem.jpg", Index = 0, Text = "What is the problem?", Title = "Just do it" });
                }
                _instructionDbContext.SaveChanges();
            }
            ViewBag.CurrentUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            return View(_instructionDbContext.Instructions
                .Include(i => i.Steps).Include(i => i.Category).Include(i => i.Author).Include(i => i.Comments).ThenInclude(c => c.Likes).ThenInclude(l => l.User).Include(i => i.Author)
                .ToList());
        }

        [HttpPost]
        public IActionResult InstructionList(string categoryId, string userId)
        {
            var emptyInstructionsList = _instructionDbContext.Instructions.Where(i => i.Steps.Count == 0).ToList();
            if (emptyInstructionsList.Count > 0)
            {
                foreach (var instruction in emptyInstructionsList)
                {
                    instruction.Steps.Add(new Step { Image = "What's the problem.jpg", Index = 0, Text = "What is the problem?", Title = "Just do it" });
                }
                _instructionDbContext.SaveChanges();
            }
            ViewBag.CurrentUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            ViewBag.CurrentUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            return View(categoryId == null ? userId == null ?
                _instructionDbContext.Instructions //NOTHING
                    .Include(i => i.Steps).Include(i => i.Category).Include(i => i.Author).Include(i => i.Comments).ThenInclude(c => c.Likes).ThenInclude(l => l.User).Include(i => i.Author)
                    .ToList() :
                _instructionDbContext.Instructions //USER
                    .Include(i => i.Steps).Include(i => i.Category).Include(i => i.Author).Include(i => i.Comments).ThenInclude(c => c.Likes).ThenInclude(l => l.User).Include(i => i.Author)
                    .Where(i => i.Author.Id == userId).ToList() :
                _instructionDbContext.Instructions //CATEGORY
                    .Include(i => i.Steps).Include(i => i.Category).Include(i => i.Author).Include(i => i.Comments).ThenInclude(c => c.Likes).ThenInclude(l => l.User).Include(i => i.Author).Include(i => i.Comments).ThenInclude(c => c.Author)
                    .Where(i => i.Category.Id == categoryId).ToList());
        }

        [HttpGet]
        public IActionResult Instruction(string id)
        {
            return View(_instructionDbContext.Instructions
                .Include(i => i.Steps).Include(i => i.Category)
                .Single(i => i.Id == id));
        }

        [HttpGet]
        public IActionResult AddInstruction(string userId)
        {
            TempData["AuthorId"] = userId ?? _userManager.GetUserAsync(HttpContext.User).Result.Id;
            SelectList categories = new SelectList(_instructionDbContext.Categories, "Id", "Name");
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public IActionResult AddInstruction(Instruction instruction)
        {
            if (!ModelState.IsValid) return View(instruction);
            var category = _instructionDbContext.Categories.Find(instruction.Category.Id);
            instruction.Time = DateTime.Now;
            instruction.Author = _instructionDbContext.Users.Find((string) TempData["AuthorId"]);
            instruction.Category = category;
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
        public async Task<IActionResult> AddSteps(Step step, IFormFile file)
        {
            if (!ModelState.IsValid) return View(step);
            var instructionId = (string)TempData["Instruction"];
            var instruction = _instructionDbContext.Instructions.Include(i => i.Steps).Single(i => i.Id == instructionId);
            var path = "/images/" + file.FileName;
            using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            step.Image = file.FileName;
            step.Index = instruction.Steps.Count;
            instruction.Steps.Add(step);
            _instructionDbContext.SaveChanges();
            return RedirectToAction("AddSteps");
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