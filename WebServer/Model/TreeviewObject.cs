using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer.Model
{
    class TreeviewObject
    {

        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<Food> Food { get; set; }

        public IEnumerable<Allergen> Allergens { get; set; }

    }
}
