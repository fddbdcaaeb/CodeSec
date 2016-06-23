using CodeSec.Runtime.Dto;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeSec.Runtime
{
    public class Stage
    {
        public Stage(string assemblyDirectoryPath, string appDomainConfigFilePath)
        {
            AssemblyDirectoryPath = assemblyDirectoryPath;
            AppDomainConfigFilePath = appDomainConfigFilePath;

            Reset();
        }

        public string AssemblyDirectoryPath { get; }
        public string AppDomainConfigFilePath { get; }
        public AppDomain Domain { get; private set; }
        public StageAppDomain StageDomain { get; private set; }
        public List<MethodInfoDto> MethodInfoDtos => StageDomain.MethodInfoDtos;

        private void ReloadAppDomain()
        {
            if (Domain != null)
                AppDomain.Unload(Domain);

            var domainInfo = new AppDomainSetup
            {
                ConfigurationFile = AppDomainConfigFilePath ?? string.Empty
            };

            Domain = AppDomain.CreateDomain(MethodInfo.GetCurrentMethod().ReflectedType.FullName, null, domainInfo);
        }


        public void Reset()
        {
            ReloadAppDomain();

            StageDomain = (StageAppDomain)Domain.CreateInstanceAndUnwrap(typeof(StageAppDomain).Assembly.GetName().Name, typeof(StageAppDomain).FullName);
            StageDomain.LoadAssemblies(AssemblyDirectoryPath);
        }

        public long Measure(
            int methodId
            , object classInstance
            , object[] parameters
            , int executeNum
            , TimeSpan timeout
            , bool enableWarmup
        )
        {
            return StageDomain.Measure(methodId, classInstance, parameters, executeNum, timeout, enableWarmup);
        }
    }
}
