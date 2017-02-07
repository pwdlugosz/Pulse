﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// 
    /// </summary>
    public interface IElementHeader
    {

        string Name { get; }

        int OriginPageID { get; set; }

        int TerminalPageID { get; set; }

        int RootPageID { get; set; }

        long RecordCount { get; set; }

        int PageCount { get; set; }

    }

}
