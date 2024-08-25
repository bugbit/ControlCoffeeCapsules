using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlCoffeeCapsules
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ControlData))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
