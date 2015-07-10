using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Businessense.Models
{
    public class Item
    {
        public long ID { get; set; }
        public string ItemName { get; set; }
        public double ItemCount { get; set; }
        public string ItemUnit { get; set; }
    }
}
