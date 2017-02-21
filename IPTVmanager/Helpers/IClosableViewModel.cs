using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPTVman.Helpers
{
    interface IClosableViewModel
    {
        event EventHandler CloseWindowEvent;
    }
}
