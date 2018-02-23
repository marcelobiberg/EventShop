using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerShopAppSimple.Entities
{
    public class Event : BaseEntity
    {
        public string Title { get; set; }
        public DateTime EventDate { get; set; }
        public string LongDescription { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; }
    }
}