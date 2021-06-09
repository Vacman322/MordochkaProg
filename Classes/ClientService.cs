using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MordochkaProg.EF
{
    public partial class ClientService
    {
        public string CountOfDocs { get => DocumentByService.Count.ToString(); }
    }
}
