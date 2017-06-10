﻿using System.ComponentModel;

namespace Lithnet.Miiserver.AutoSync
{
    public enum ExecutionErrorBehaviour
    {
        [Description("Terminate the script")]
        Terminate = 0,

        [Description("Reset the script and resume execution")]
        ResetAndResume = 1,
    }
}
