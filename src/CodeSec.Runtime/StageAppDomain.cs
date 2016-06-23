using CodeSec.Runtime.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CodeSec.Runtime
{
    public class StageAppDomain : MarshalByRefObject
    {
        public Dictionary<int, MethodInfo> MethodInfos { get; private set; }
        public List<MethodInfoDto> MethodInfoDtos { get; private set; }

        internal void LoadAssemblies(string assemblyDirectoryPath)
        {
            var dllFiles = Directory.GetFiles(assemblyDirectoryPath, "*.dll", SearchOption.AllDirectories);
            var exeFiles = Directory.GetFiles(assemblyDirectoryPath, "*.exe", SearchOption.AllDirectories);

            var assemblies = dllFiles.Concat(exeFiles)
                .Select(x => Assembly.LoadFrom(x))
                .ToArray();

            var ignores = Settings.Default.IgnoreAssemblies.Cast<string>().ToArray();

            var types = assemblies
                .Where(x => !ignores.Contains(x.GetName().Name))
                .SelectMany(x =>
                {
                    Type[] _types = null;
                    try
                    {
                        _types = x.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        Console.WriteLine(ex.Message + Environment.NewLine
                            + ex.StackTrace + Environment.NewLine
                            + string.Join(Environment.NewLine, ex.LoaderExceptions.Select(_ => _.Message))
                        );
                    }

                    return _types ?? new Type[0];
                })
                .ToArray();

            var properties = types
                .SelectMany(x => x.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                .Select(x => x.GetMethod)
                .Where(x => x != null)
                .ToArray();

            var methods = types
                .SelectMany(x => x.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                .Concat(properties)
                .Where(x => !x.IsAbstract)
                .Where(x => x.DeclaringType.FullName == x.ReflectedType.FullName)
                .Where(x => !ignores.Contains(x.DeclaringType.Assembly.GetName().Name))
                .Where(x => x.DeclaringType.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
                .ToArray();

            MethodInfos = new Dictionary<int, MethodInfo>(methods.Length);
            MethodInfoDtos = new List<MethodInfoDto>(methods.Length);
            var id = 0;

            foreach (var method in methods)
            {
                var paramInfos = method.GetParameters();
                if (!paramInfos.All(x => x.ParameterType.IsPrimitive || x.ParameterType == typeof(string)))
                    continue;

                var paramDtos = paramInfos
                    .Select(x => new ParameterInfoDto
                    {
                        Position = x.Position,
                        Name = x.Name,
                        TypeName = x.ParameterType.FullName,
                        DefaultValue = x.HasDefaultValue ? x.DefaultValue?.ToString() ?? "null" : null
                    })
                    .ToArray();

                var methodDto = new MethodInfoDto
                {
                    Id = id,
                    Name = method.Name,
                    DefinedTypeName = method.ReflectedType.Name,
                    DefinedTypeNamespace = method.ReflectedType.Namespace,
                    Parameters = paramDtos
                };

                MethodInfos[id] = method;
                MethodInfoDtos.Add(methodDto);
                id++;
            }
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
            MethodInfo method;
            if (!MethodInfos.TryGetValue(methodId, out method))
                throw new InvalidOperationException("Does not exist specific methodId.");

            return TimeMeasure.Measure(method, classInstance, parameters, executeNum, timeout, enableWarmup);
        }
    }
}
