using System.Collections.Generic;

namespace DesperateDevs.CodeGen
{
    public interface ICachable
    {
        Dictionary<string, object> ObjectCache { get; set; }
    }
}
