using NimbusMergerLibrary.Tests.NimbusScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusMergerLibrary.Tests.NimbusScript.Commands
{
    /// <summary>
    /// Command of type 'set_aircraft_short_name [aircraft string id] [aircraft short name]'
    /// </summary>
    public class SetAircraftShortNameCommand : BaseCommand
    {
        private string _aircraftStringID;
        private string _aircraftShortName;

        public override void Prepare(string[] splits)
        {
            if (splits.Length != 3) throw new InvalidArgsNumberException();

            _aircraftStringID = splits[1];
            _aircraftShortName = splits[2].ToUpperInvariant();

            throw new NotImplementedException();
        }
    }
}
