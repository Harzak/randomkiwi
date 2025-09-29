using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Models;

public enum EUserNavigationPattern
{
    Unknown,
    Reviewer, // Goes back frequently
    Explorer, // Mixed navigation
    Reader, // Mostly forward navigation
}