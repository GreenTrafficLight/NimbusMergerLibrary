using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusMergerLibrary.Tests.NimbusScript.Commands
{
    public abstract class BaseCommand
    {
        public abstract void Prepare(string[] splits);
    }
}
