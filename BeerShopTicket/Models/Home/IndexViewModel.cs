using BeerShopAppSimple.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerShopTicket.Models.Home
{
    public class IndexViewModel
    {
        //injeção de dependência
        public IndexViewModel()
        {
            Events = new List<Event>();
        }

        public List<Event> Events { get; set; }
    }
}