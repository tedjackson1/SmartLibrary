using LMSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Controllers
{
    public class PublicationsController : Controller
    {
        private readonly LibraryContext _context;

        public PublicationsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Publications/Index?type=Newspaper
        // GET: Publications/Index?type=Magazine
        public async Task<IActionResult> Index(
            string type,
            string? searchString,
            int pageNumber = 1)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return BadRequest();
            }

            if (!Enum.TryParse(
                    type,
                    true,
                    out PublicationType publicationType))
            {
                return NotFound();
            }

            ViewData["CurrentType"] = publicationType.ToString();
            ViewData["CurrentFilter"] = searchString;

            var publications = _context.Publications
                .Where(p => p.Type == publicationType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                publications = publications.Where(p =>
                    (p.Title != null &&
                     p.Title.Contains(searchString)) ||
                    (p.Publisher != null &&
                     p.Publisher.Contains(searchString)));
            }

            int pageSize = 5;

            int totalItems = await publications.CountAsync();

            int totalPages = (int)Math.Ceiling(
                totalItems / (double)pageSize
            );

            if (totalPages < 1)
            {
                totalPages = 1;
            }

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            var paginatedList = await publications
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;

            return View(paginatedList);
        }

        // GET: Publications/Create?type=Newspaper
        public IActionResult Create(string type)
        {
            if (!Enum.TryParse(
                    type,
                    true,
                    out PublicationType publicationType))
            {
                return NotFound();
            }

            ViewData["CurrentType"] = publicationType.ToString();

            var publication = new Publication
            {
                Type = publicationType,
                PublishedDate = DateTime.Today,
                IsAvailable = true
            };

            return View(publication);
        }

        // POST: Publications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Publisher,PublishedDate,Type,IsAvailable")]
            Publication publication)
        {
            if (ModelState.IsValid)
            {
                _context.Publications.Add(publication);
                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        type = publication.Type.ToString()
                    }
                );
            }

            ViewData["CurrentType"] =
                publication.Type.ToString();

            return View(publication);
        }

        // GET: Publications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication =
                await _context.Publications.FindAsync(id);

            if (publication == null)
            {
                return NotFound();
            }

            ViewData["CurrentType"] =
                publication.Type.ToString();

            return View(publication);
        }

        // POST: Publications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Publisher,PublishedDate,Type,IsAvailable")]
            Publication publication)
        {
            if (id != publication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Publications.Update(publication);
                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        type = publication.Type.ToString()
                    }
                );
            }

            ViewData["CurrentType"] =
                publication.Type.ToString();

            return View(publication);
        }

        // GET: Publications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication =
                await _context.Publications
                    .FirstOrDefaultAsync(p => p.Id == id);

            if (publication == null)
            {
                return NotFound();
            }

            ViewData["CurrentType"] =
                publication.Type.ToString();

            return View(publication);
        }

        // POST: Publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publication =
                await _context.Publications.FindAsync(id);

            if (publication == null)
            {
                return NotFound();
            }

            string publicationType =
                publication.Type.ToString();

            _context.Publications.Remove(publication);
            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index),
                new
                {
                    type = publicationType
                }
            );
        }
    }
}