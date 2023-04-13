using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioManager.Domain.Entities;
using PortfolioManager.Persistence;

namespace PortfolioManager.Web.Pages
{
    public class PortfolioModel : PageModel
    {
        private readonly PortfolioManagerDbContext _dbContext;

        public PortfolioModel(PortfolioManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Asset> Assets { get; set; }
        [BindProperty]
        public Asset Asset { get; set; }

        public void OnGet()
        {
            ViewData["Title"] = "Portfolio";

            Assets = _dbContext.Assets;
        }

        public IActionResult OnPost()
        {
            if ((Asset is not null) && ModelState.IsValid)
            {
                _dbContext.Assets.Add(Asset);
                _dbContext.SaveChanges();
                return RedirectToAction("/portfolio");
            }
            else
            {
                return Page(); // return to original page
            }
        }
    }
}