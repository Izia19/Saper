﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper
{
    public class User
    {
        public string Miejsce { get; set; }
        public string Nick { get; set; }
        public string Wynik { get; set; }

        public User(string miejsce, string nick, string wynik) 
        {
            this.Miejsce = miejsce;
            this.Nick = nick;
            this.Wynik = wynik;    
        }
    }
}
