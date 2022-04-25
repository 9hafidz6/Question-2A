using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace producer.Models
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options)
        : base(options)
        {
        }

        public DbSet<Credentials> Bookings { get; set; } = null!;
    }
}
