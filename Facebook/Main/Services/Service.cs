using Data;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Services
{
    public abstract class Service
    {
        public abstract bool CheckID(int id); 
    }
}
