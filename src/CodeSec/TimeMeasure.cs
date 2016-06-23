using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using CodeSec.Core.Extension;

namespace CodeSec
{
    /// <summary>
    /// Measure execution time of method at any count.
    /// </summary>
    public class TimeMeasure
    {
        public static long Measure(
            MethodInfo method
            , object classInstance
            , object[] parameters
            , int executeNum
            , TimeSpan timeout
            , bool enableWarmup
        )
        {
            // Validation.
            if (method == null)
                throw new ArgumentException("A method reference is null. It must be specified.");

            // Resolve classInstance if method is Static.
            if (!method.IsStatic && classInstance == null)
            {
                try
                {
                    classInstance = Activator.CreateInstance(method.ReflectedType);
                }
                finally
                {
                    if (classInstance == null)
                        throw new ArgumentException("A classInstance reference is null. It must be specified when execute Instance-Method.");
                }
            }

            var isAsync = method.IsAsync();

            // Warm-up(Method is execute just one time before main execution).
            if (enableWarmup)
            {
                var warmuped = Task.Run(async () =>
                {
                    if (isAsync)
                        await (dynamic)method.Invoke(classInstance, parameters);
                    else
                        method.Invoke(classInstance, parameters);
                })
                .Wait(timeout);

                if (!warmuped)
                    throw new TimeoutException("Warm-up could not complete in specific time.");
            }

            // Execute.
            var isEnable = true;
            var stopwatch = new Stopwatch();
            var done = Task.Run(async () =>
            {
                stopwatch.Start();

                for (var i = 0; i < executeNum && isEnable; i++)
                {
                    if (isAsync)
                        await (dynamic)method.Invoke(classInstance, parameters);
                    else
                        method.Invoke(classInstance, parameters);
                }

                stopwatch.Stop();
            })
            .Wait(timeout);

            isEnable = false;
            if (!done)
                throw new TimeoutException("Process could not complete in specific time.");

            Console.WriteLine($"{method.Name}: {stopwatch.ElapsedMilliseconds}");
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
