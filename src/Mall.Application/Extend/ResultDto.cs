﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Mall.Extend
{
    public class ResultDto<T> where T:class
    {
        public List<T> Rows { get; set; }

        public int Count { get; set; }
    }
}
