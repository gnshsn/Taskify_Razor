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

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                TodoItems = await _context.TodoItems
                    .Include(t => t.Category)
                    .Where(t => t.OwnerId == user.Id)
                    .ToListAsync();
            }
            else
            {
                TodoItems = new List<TodoItems>();
            }
        }
    }
}
