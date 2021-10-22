using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISBOT
{
    class Buytask
    {
        public bool Pause = true;
        public int Tasknum { get; set; }
        public string Number { get; set; }
        public int Info_random { get; set; }
        public string Status { get; set; }
        public string PaymentURL { get; set; }
        public bool Mask { get; set; }

        public bool Num_true { get; set; }

        public bool Num_False { get; set; }

        public int Working_Max { get; set; }

        public int Working { get; set; }
    }
}
