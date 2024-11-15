using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUX__KASSA
{
    public interface IErrorHandler
    {

        void ShowError(string errorMessage);
        void LogError(string errorMessage);
    }
}
