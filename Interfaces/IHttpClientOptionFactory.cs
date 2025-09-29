using randomkiwi.Services.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

public interface IHttpClientOptionFactory
{
    HttpClientOption CreateOption();
}
