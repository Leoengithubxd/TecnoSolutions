﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TecnoSolutions.Dtos
{
    public class Product
    {
        public int IdProduct { get; set; }
        public string Name { get; set; }
        public float Stock { get; set; }
        public string UnitPrice { get; set; }
    }
}