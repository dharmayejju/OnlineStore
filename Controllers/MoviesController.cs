using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

            foreach(var r in result)
            {
                var file = r.Image;
                var base64 = Convert.ToBase64String(file);
                var imgSrc = String.Format("data:image/jpg;base64,{0}", base64);
                r.ImagePath = imgSrc;
            }

            return View(result);
        }
    }
}
