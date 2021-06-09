using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MordochkaProg.EF;

namespace MordochkaProg
{
    public class DB
    {
        public static Context Context { get; set; } = new Context();
    }
}
