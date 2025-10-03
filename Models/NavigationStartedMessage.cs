using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Models;

/// <summary>
/// Message sent when navigation starts.
/// </summary>
public sealed record NavigationStartedMessage(Uri? Url);
