using FluentAssertions;
using LMSystem.Controllers;
using LMSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Tests
{
    public class BooksControllerTests : IDisposable
    {
        private readonly LibraryContext _context;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            var options =
                new DbContextOptionsBuilder<LibraryContext>()
                    .UseInMemoryDatabase(
                        databaseName: Guid.NewGuid().ToString())
                    .Options;

            _context = new LibraryContext(options);

            SeedDatabase();

            _controller = new BooksController(_context);
        }

        private void SeedDatabase()
        {
            _context.Books12.AddRange(
                new Book
                {
                    BookId = 1,
                    Title = "Bootstrap",
                    Author = "Amir",
                    ISBN = "888-0201616224",
                    PublishedDate = new DateTime(2026, 7, 24),
                    IsAvailable = true
                },

                new Book
                {
                    BookId = 2,
                    Title = "Node JS",
                    Author = "Shadab",
                    ISBN = "888-0201616225",
                    PublishedDate = new DateTime(2026, 7, 18),
                    IsAvailable = true
                },

                new Book
                {
                    BookId = 3,
                    Title = "Software Engineering",
                    Author = "Raju",
                    ISBN = "888-0201616226",
                    PublishedDate = new DateTime(2026, 7, 20),
                    IsAvailable = true
                },

                new Book
                {
                    BookId = 4,
                    Title = "ASP.NET Core",
                    Author = "David",
                    ISBN = "888-0201616227",
                    PublishedDate = new DateTime(2026, 7, 21),
                    IsAvailable = true
                },

                new Book
                {
                    BookId = 5,
                    Title = "Cloud Computing",
                    Author = "Robert",
                    ISBN = "888-0201616228",
                    PublishedDate = new DateTime(2026, 7, 22),
                    IsAvailable = true
                },

                new Book
                {
                    BookId = 6,
                    Title = "Database Systems",
                    Author = "James",
                    ISBN = "888-0201616229",
                    PublishedDate = new DateTime(2026, 7, 23),
                    IsAvailable = true
                }


            );

            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_FiltersBooks_WhenSearchQueryIsProvided()
        {
            var result = await _controller.Index(
                searchQuery: "Node",
                page: 1);

            var viewResult = result
                .Should()
                .BeOfType<ViewResult>()
                .Subject;

            var model = viewResult.Model
     .Should()
     .BeOfType<BookListViewModel>()
     .Subject;

            model.Books.Should().ContainSingle();

            model.Books.First().Title.Should().Be("Node JS");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [Fact]
        public async Task Index_ReturnsCorrectPageSize_ForPaginatedRequests()
        {
            // Act
            var result = await _controller.Index(
                searchQuery: null,
                page: 2);

            // Assert
            var viewResult = result
                .Should()
                .BeOfType<ViewResult>()
                .Subject;

            var model = viewResult.Model
                .Should()
                .BeOfType<BookListViewModel>()
                .Subject;

            model.Books.Count().Should().Be(1);
        }
    }
}