using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Entities
{
    public class RequestCountItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RequestName { get; set; }

        public int Count { get; set; }
    }
}
