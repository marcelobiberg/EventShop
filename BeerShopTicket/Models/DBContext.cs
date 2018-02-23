using BeerShopAppSimple.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BeerShopTicket.Models
{
    public class DBContext : DbContext
    {
        public DBContext()
    : base("BancoContexto")
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
    }
}