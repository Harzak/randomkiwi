using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Models;

/// <summary>
/// Specifies the level of detail for an article.
/// </summary>
public enum EArticleDetail
{
    Unknown = 0,
    Any = 1,
    Medium = 2,
    Detailed = 3,
}