using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Models;

/// <summary>
/// Message sent when navigation completes.
/// </summary>
public sealed record NavigationCompletedMessage(string Url, bool IsSuccess);
