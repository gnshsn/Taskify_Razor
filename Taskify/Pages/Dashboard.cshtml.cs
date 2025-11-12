using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Taskify.Data;
using Taskify.Models;

namespace Taskify.Pages;

public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public DashboardModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int TotalTodos { get; set; }
    public int CompletedTodos { get; set; }
    public int PendingTodos { get; set; }
    public List<CategorySummary> CategorySummaries { get; set; } = new();
    public List<TodoItems> UpcomingTodos { get; set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        var userTodos = _context.TodoItems
            .Include(t => t.Category)
            .Where(t => t.OwnerId == user.Id);

        TotalTodos = await userTodos.CountAsync();
        CompletedTodos = await userTodos.CountAsync(t => t.Status == Models.TaskStatus.Completed);
        PendingTodos = await userTodos.CountAsync(t => t.Status != Models.TaskStatus.Completed);

        CategorySummaries = await userTodos
            .GroupBy(t => t.Category!.Name)
            .Select(g => new CategorySummary
            {
                CategoryName = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        UpcomingTodos = await userTodos
            .Where(t => t.DueDate != null && t.DueDate > DateTime.Now)
            .OrderBy(t => t.DueDate)
            .Take(5)
            .ToListAsync();
    }

    public class CategorySummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
