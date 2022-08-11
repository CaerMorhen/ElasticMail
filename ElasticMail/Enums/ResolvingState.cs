using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticMail.Enums
{
    /// <summary>
    /// State of dns resolving process.
    /// </summary>
    public enum ResolvingState
    {
        ERROR = -1,
        READY = 1,
        IN_PROGRESS = 2,
        FINISHED = 3
    }
}
