using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public string Genre { get; set; }
    }
}
