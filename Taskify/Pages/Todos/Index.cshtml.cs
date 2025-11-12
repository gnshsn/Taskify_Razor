using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Taskify.Data;
using Taskify.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Taskify.Pages.Todos
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<TodoItems> TodoItems { get;set; } = default!;
        public List<Category> Categories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Models.TaskStatus StatusFilter { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TodoItems = new List<TodoItems>();
                return;
            }

            var query = _context.TodoItems
                .Include(t => t.Category)
                .Where(t => t.OwnerId == user.Id)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(t => t.Title.Contains(SearchTerm));
            }

            if (CategoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == CategoryId);
            }

            if (Enum.IsDefined(typeof(Models.TaskStatus), StatusFilter))
            {
                query = query.Where(t => t.Status == StatusFilter);
            }

            Categories = await _context.Categories.ToListAsync();
            TodoItems = await query.OrderBy(t => t.DueDate).ToListAsync();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return new JsonResult(new { success = false, message = "Not logged in" });

            var todo = await _context.TodoItems
                .Where(t => t.OwnerId == user.Id && t.Id == id)
                .FirstOrDefaultAsync();

            if (todo == null)
                return new JsonResult(new { success = false, message = "Not found" });

            todo.Status = Models.TaskStatus.Completed;
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
