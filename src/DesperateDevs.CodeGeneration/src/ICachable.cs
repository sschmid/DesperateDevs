﻿using System.Collections.Generic;

namespace DesperateDevs.CodeGeneration
{
    public interface ICachable
    {
        Dictionary<string, object> ObjectCache { get; set; }
    }
}