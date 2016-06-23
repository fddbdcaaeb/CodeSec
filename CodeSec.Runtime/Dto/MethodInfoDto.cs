using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSec.Runtime.Dto
{
    [Serializable]
    public class MethodInfoDto
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; }
        public string DefinedTypeName { get; internal set; }
        public string DefinedTypeNamespace { get; internal set; }
        public ParameterInfoDto[] Parameters { get; internal set; }
    }
}
