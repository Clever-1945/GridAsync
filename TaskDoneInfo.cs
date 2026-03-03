using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncUI;

public struct TaskDoneInfo<RT>
{
    public bool Success;
    public RT Data;
    public Exception Exception;
}