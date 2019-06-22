using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Infrastructure;
using OnlineStore.Models;

namespace OnlineStore.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IConnection _connection;

        public MoviesController(IConnection connection)
        {
            _connection = connection;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            const string sql = @"SELECT * FROM movies";
            var result = await _connection.QueryAsync<Movie>(sql);
            return View(result);
        }
    }
}
