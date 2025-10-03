using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Models;

/// <summary>
/// Message sent when a link is clicked.
/// </summary>
public sealed record LinkClickedMessage(Uri ClickedUrl);
