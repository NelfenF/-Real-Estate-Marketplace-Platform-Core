﻿using Newtonsoft.Json;

namespace Project4.Models
{
    public class Homes : ListOfObjects<Home>
    {
        public Homes() { }
        public Homes(List<Home> list) { List = list; }

        public Homes Clone()
        {
            return new Homes(List);
        }
    }
}
