using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSec.Runtime.Dto
{
    [Serializable]
    public class ParameterInfoDto
    {
        public int Position { get; internal set; }
        public string Name { get; internal set; }
        public string TypeName { get; internal set; }
        public string DefaultValue { get; internal set; }
    }
}
