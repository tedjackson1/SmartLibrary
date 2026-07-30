using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMSystem.Models;

namespace LMSystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(
    string? searchQuery,
    int page = 1)
        {
            try
            {
                const int pageSize = 5;

                var booksQuery = _context.Books12
                    .Include(b => b.BorrowRecords)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();

                    booksQuery = booksQuery.Where(b =>
                        (b.Title != null &&
                         b.Title.Contains(searchQuery)) ||

                        (b.Author != null &&
                         b.Author.Contains(searchQuery)) ||

                        (b.ISBN != null &&
                         b.ISBN.Contains(searchQuery)));
                }

                int totalItems = await booksQuery.CountAsync();

                int totalPages = (int)Math.Ceiling(
                    (double)totalItems / pageSize);

                if (page < 1)
                {
                    page = 1;
                }

                if (page > totalPages && totalPages > 0)
                {
                    page = totalPages;
                }

                var books = await booksQuery
                    .OrderBy(b => b.BookId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new BookListViewModel
                {
                    Books = books,
                    SearchQuery = searchQuery,
                    CurrentPage = page,
                    TotalPages = totalPages
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    "An error occurred while loading the books.";

                return View("Error");
            }
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided.";
                return View("NotFound");
            }

            try
            {
                var book = await _context.Books12
                    .FirstOrDefaultAsync(m => m.BookId == id);

                if (book == null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {id}.";
                    return View("NotFound");
                }

                return View(book);
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while loading the book details.";
                return View("Error");
            }
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Books12.Add(book);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] =
                        $"Successfully added the book: {book.Title}.";

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    TempData["ErrorMessage"] =
                        "An error occurred while adding the book.";

                    return View(book);
                }
            }

            return View(book);
        }

    }
}