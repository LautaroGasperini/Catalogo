﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Marcas
    {
        public int Id { get; set; }
        public string Marca { get; set; }

        public override string ToString()
        {
            return Marca; 
        }
    }
}