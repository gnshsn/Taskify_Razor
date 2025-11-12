using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Taskify.Data;
using Taskify.Models;

namespace Taskify.Pages.Todos
{
    public class DetailsModel : PageModel
    {
        private readonly Taskify.Data.ApplicationDbContext _context;

        public DetailsModel(Taskify.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public TodoItems TodoItems { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoitems = await _context.TodoItems.FirstOrDefaultAsync(m => m.Id == id);
            if (todoitems == null)
            {
                return NotFound();
            }
            else
            {
                TodoItems = todoitems;
            }
            return Page();
        }
    }
}
