using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Models;

/// <summary>
/// Message sent when a URL is about to change.
/// </summary>
public sealed record UrlChangingMessage(Uri? OldUrl, Uri? NewUrl);
