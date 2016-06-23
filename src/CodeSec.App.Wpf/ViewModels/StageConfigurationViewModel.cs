using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSec.App.Wpf.ViewModels
{
    public class StageConfigurationViewModel
    {
        public StageConfigurationViewModel()
        {
        }

        public string AssemblyRootPath { get; set; }
        public string ConfigurationFilePath { get; set; }
    }
}
