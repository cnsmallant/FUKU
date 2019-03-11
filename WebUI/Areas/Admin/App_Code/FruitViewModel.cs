using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



    public class FruitViewModel
    {
        public IEnumerable<Fruit> AvailableFruits { get; set; }
        public IEnumerable<Fruit> SelectedFruits { get; set; }
        public PostedFruits PostedFruits { get; set; }
    }
