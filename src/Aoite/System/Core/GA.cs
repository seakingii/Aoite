using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace System
{
    /// <summary>
    /// 通用全局函数。
    /// </summary>
    public static partial class GA
    {
        /// <summary>
        /// 提供 Unicode 字节顺序标记的 UTF-8 编码。
        /// </summary>
        public readonly static Encoding UTF8 = new UTF8Encoding(false);

        /// <summary>
        /// 获取应用程序当前的操作系统主版本是否少于 6（XP/2003 含以下的操作系统）。
        /// </summary>
        public static readonly bool IsOldOS = Environment.OSVersion.Version.Major < 6;
        /// <summary>
        /// 获取包含该应用程序的目录的名称。该字符串结尾包含“\”。
        /// </summary>
        public static readonly string AppDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        /// <summary>
        /// 获取 Aoite 的临时目录。
        /// </summary>
        public static readonly string TempFolder = Path.Combine(Path.GetTempPath(), "Aoite - " + Aoite.AoiteInfo.Core.AssemblyVersion);
        /// <summary>
        /// 当要求抛出错误时的全局事件。
        /// </summary>
        public static event ExceptionEventHandler GlobalError;

        /// <summary>
        /// 当要求抛出错误时的全局事件委托。
        /// </summary>
        /// <param name="sender">事件的对象。允许为 null 值。</param>
        /// <param name="exception">抛出的异常。不允许为 null 值。</param>
        public static void OnGlobalError(object sender, Exception exception)
        {
            if(exception == null) throw new ArgumentNullException(nameof(exception));
            System.Diagnostics.Trace.TraceError(exception.ToString());

            var handler = GlobalError;
            if(handler != null) handler(sender, new ExceptionEventArgs(exception));
        }

        #region Comm

        static GA()
        {
            _IsUnitTestRuntime = (from ass in AppDomain.CurrentDomain.GetAssemblies()
                                  let fn = ass.FullName
                                  where fn.iStartsWith("Microsoft.VisualStudio.QualityTools.UnitTestFramework")
                                  || fn.iStartsWith("xunit")
                                  || fn.iStartsWith("nuint")
                                  select true).FirstOrDefault();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //System.Runtime.CompilerServices.RuntimeWrappedException
            if(e.ExceptionObject is Exception)
            {
                GA.OnGlobalError(sender, e.ExceptionObject as Exception);
            }
            else
            {
                WriteUnhandledException("应用崩溃了：{0}", e.ExceptionObject);
            }
        }
        /// <summary>
        /// 写入未捕获的异常。该异常不记录到日志管理器，而是独立出一个 LogError{yyyy-MM-dd}.txt 文件。
        /// </summary>
        /// <param name="message">复合格式的错误消息。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        public static void WriteUnhandledException(string message, params object[] args)
        {
            var path = GA.IsWebRuntime ? System.Web.Webx.MapUrl("~/LogError" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt") : GA.FullPath("LogError" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            System.IO.File.AppendAllText(path, DateTime.Now + " " + string.Format(message, args));
        }

        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern int UuidCreateSequential(out Guid guid);

        /// <summary>
        /// 初始化一个有顺序规则 <see cref="Guid"/> 的新实例。
        /// </summary>
        /// <returns><see cref="Guid"/> 的新实例。</returns>
        public static Guid NewComb()
        {
            const int RPC_S_OK = 0;
            Guid guid;
            var result = UuidCreateSequential(out guid);
            if(result == RPC_S_OK) return guid;
            return Guid.NewGuid();
        }

        /// <summary>
        /// 将指定的命令行进行拆分。
        /// </summary>
        /// <param name="commandLine">命令行。</param>
        /// <returns>命令行。</returns>
        public static string[] ToCommandLines(string commandLine)
        {
            int numberOfArgs;
            IntPtr ptrToSplitArgs;
            string[] splitArgs;

            ptrToSplitArgs = CommandLineToArgvW(commandLine, out numberOfArgs);
            if(ptrToSplitArgs == IntPtr.Zero)
                throw new ArgumentException("Unable to split argument.",
                  new System.ComponentModel.Win32Exception());
            try
            {
                splitArgs = new string[numberOfArgs];
                for(int i = 0; i < numberOfArgs; i++)
                    splitArgs[i] = System.Runtime.InteropServices.Marshal.PtrToStringUni(
                        System.Runtime.InteropServices.Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size));
                return splitArgs;
            }
            finally
            {
                LocalFree(ptrToSplitArgs);
            }
        }

        [System.Runtime.InteropServices.DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lpCmdLine,
            out int pNumArgs);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr LocalFree(IntPtr hMem);

        private static bool _IsUnitTestRuntime;
        /// <summary>
        /// 获取或设置一个值，指示当前是否为单元测试的运行环境。
        /// </summary>
        public static bool IsUnitTestRuntime
        {
            get { return _IsUnitTestRuntime; }
            set { _IsUnitTestRuntime = value; }
        }

        /// <summary>
        /// 获取一个值，指示当前线程是否为 Web 线程。
        /// </summary>
        public static bool IsWebRuntime { get { return System.Web.HttpContext.Current != null; } }
        /// <summary>
        /// 获取一个值，该值指示当前应用程序是否以管理员权限的运行。
        /// </summary>
        public static bool IsAdministrator
        {
            get
            {
                if(IsOldOS) return true;

                var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// 以管理员权限重新运行当前应用程序。
        /// </summary>
        public static void RunAsAdministrator()
        {
            if(IsAdministrator) return;
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Assembly.GetEntryAssembly().Location,
                Verb = "runas"
            });
        }

        /// <summary>
        /// 获取指定路径的完整路径。
        /// </summary>
        /// <param name="path">绝对路径或相对路径。</param>
        /// <returns>若 <paramref name="path"/> 是绝对路径，则返回本身，否则返回基于当前应用程序目录的绝对路径。</returns>
        public static string FullPath(string path)
        {
            if(string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            if(Path.IsPathRooted(path)) return path;
            return Path.Combine(AppDirectory, path);
        }
        /// <summary>
        /// 获取指定路径的完整路径。
        /// </summary>
        /// <param name="paths">绝对路径或相对路径数组。</param>
        /// <returns>基于当前应用程序目录的绝对路径。</returns>
        public static string FullPath(params string[] paths)
        {
            if(paths == null || paths.Length == 0) throw new ArgumentNullException(nameof(paths));
            if(paths.Length == 1)
            {
                return FullPath(paths[0]);
            }
            else
            {
                if(Path.IsPathRooted(paths[0])) return Path.Combine(paths);
                string[] newPaths = new string[paths.Length + 1];
                newPaths[0] = AppDirectory;
                Array.Copy(paths, 0, newPaths, 1, paths.Length);
                return Path.Combine(newPaths);
            }
        }
        /// <summary>
        /// 使用指定的对象数组和格式设置信息向 <see cref="Diagnostics.Trace.Listeners"/> 集合中的跟踪侦听器中写入错误消息。
        /// </summary>
        /// <param name="format">包含零个或多个格式项的格式字符串，这些项与 <paramref name="args"/> 数组中的对象相对应。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="object"/> 数组。</param>
        [Conditional("TRACE")]
        public static void TraceError(string format, params object[] args)
        {
            if(args == null || args.Length == 0) Trace.TraceError(format);
            else Trace.TraceError(format, args);
        }

        /// <summary>
        /// 使用指定的对象数组和格式设置信息向 <see cref="Diagnostics.Trace.Listeners"/> 集合中的跟踪侦听器中写入警告信息。
        /// </summary>
        /// <param name="format">包含零个或多个格式项的格式字符串，这些项与 <paramref name="args"/> 数组中的对象相对应。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="object"/> 数组。</param>
        [Conditional("TRACE")]
        public static void TraceWarning(string format, params object[] args)
        {
            if(args == null || args.Length == 0) Trace.TraceWarning(format);
            else Trace.TraceWarning(format, args);
        }

        /// <summary>
        /// 使用指定的对象数组和格式设置信息向 <see cref="Diagnostics.Trace.Listeners"/> 集合中的跟踪侦听器中写入信息性消息。
        /// </summary>
        /// <param name="format">包含零个或多个格式项的格式字符串，这些项与 <paramref name="args"/> 数组中的对象相对应。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="object"/> 数组。</param>
        [Conditional("TRACE")]
        public static void TraceInformation(string format, params object[] args)
        {
            if(args == null || args.Length == 0) Trace.TraceInformation(format);
            else Trace.TraceInformation(format, args);
        }

        /// <summary>
        /// 释放并关闭所有线程上下文的上下文对象。非【主线程】的其他线程一单使用其下列对象，就应该的调用此方法进行释放:
        /// <para><see cref="Db.Context"/></para>
        /// <para><see cref="Log.Context"/></para>
        /// <para><see cref="Aoite.Redis.RedisManager.Context"/></para>
        /// </summary>
        public static void ResetContexts()
        {
            Aoite.Redis.RedisManager.ResetContext();
            Db.ResetContext();
            Log.ResetContext();
        }

        #endregion

        #region Interlocked

        /// <summary>
        /// 原子操作形式判断 <paramref name="localtion"/> 是否与 <paramref name="value"/> 匹配。
        /// </summary>
        /// <param name="localtion">要加载的 64 位值。</param>
        /// <param name="value">要判断的 64 位值。</param>
        /// <returns>如果匹配则返回 true，否则返回 false。</returns>
        public static bool LEquals(ref long localtion, long value)
        {
            return Interlocked.Read(ref localtion) == value;
        }

        /// <summary>
        /// 返回一个以原子操作形式加载的 64 位值。
        /// </summary>
        /// <param name="localtion">要加载的 64 位值。</param>
        /// <returns>加载的值。</returns>
        public static long LRead(ref long localtion)
        {
            return Interlocked.Read(ref localtion);
        }

        /// <summary>
        /// 以原子操作的形式，将 64 位值设置为指定的值并返回原始 64 位值。
        /// </summary>
        /// <param name="localtion">要设置为指定值的变量。</param>
        /// <param name="value">参数被设置为的值。</param>
        /// <returns>原始值。</returns>
        public static long LWrite(ref long localtion, long value)
        {
            return Interlocked.Exchange(ref localtion, value);
        }
        /// <summary>
        /// 以原子操作的形式递增指定变量的值并存储结果。
        /// </summary>
        /// <param name="localtion">其值要递增的变量。</param>
        /// <returns>递增的值。</returns>
        public static long LIncrement(ref long localtion)
        {
            return Interlocked.Increment(ref localtion);
        }
        /// <summary>
        /// 以原子操作的形式递减指定变量的值并存储结果。
        /// </summary>
        /// <param name="localtion">其值要递减的变量。</param>
        /// <returns>递减的值。</returns>
        public static long Decrement(ref long localtion)
        {
            return Interlocked.Decrement(ref localtion);
        }

        #endregion

        #region Compare

        private static CompareResult Compare(string name, Type type, object t1, object t2)
        {
            type = type.GetNullableType();
            switch(Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    if(t1 == null || t2 == null) goto default;
                    if(type.IsSubclassOf(DefineTypes.Exception) || type == DefineTypes.Exception)
                    {
                        t1 = t1.ToString();
                        t2 = t2.ToString();
                        goto default;
                    }
                    if(type.IsArray)
                    {
                        var a1 = t1 as Array;
                        var a2 = t2 as Array;
                        if(a1 == null || a2 == null) goto default;
                        if(a1.Length != a2.Length) return new CompareResult
                        {
                            Name = "数组长度",
                            Value1 = a1.Length,
                            Value2 = a2.Length
                        };
                        for(int i = 0; i < a1.Length; i++)
                        {
                            var r = Compare(a1.GetValue(i), a2.GetValue(i));
                            if(r != null) return r;
                        }
                    }
                    else if(type.IsSubclassOf(DefineTypes.IDictionary))
                    {
                        var a1 = t1 as IDictionary;
                        var a2 = t2 as IDictionary;
                        if(a1 == null || a2 == null) goto default;
                        if(a1.Count != a2.Count) return new CompareResult
                        {
                            Name = "字典大小",
                            Value1 = a1.Count,
                            Value2 = a2.Count
                        };
                        foreach(DictionaryEntry item in a1)
                        {
                            if(!a2.Contains(item.Key)) return new CompareResult
                            {
                                Name = "字典键",
                                Value1 = item.Key,
                                Value2 = null
                            };
                            var r = Compare(item.Value, a2[item.Key]);
                            if(r != null) return r;
                        }
                    }
                    else if(type.IsSubclassOf(DefineTypes.IEnumerable))
                    {
                        var a1 = new ArrayList();
                        foreach(var item in t1 as IEnumerable) a1.Add(item);
                        var a2 = new ArrayList();
                        foreach(var item in t2 as IEnumerable) a2.Add(item);

                        if(a1.Count != a2.Count) return new CompareResult
                        {
                            Name = "枚举大小",
                            Value1 = a1.Count,
                            Value2 = a2.Count
                        };
                        for(int i = 0; i < a1.Count; i++)
                        {
                            var r = Compare(a1[i], a2[i]);
                            if(r != null) return r;
                        }
                    }
                    var mp = TypeMapper.Create(type);
                    foreach(var p in mp.Properties)
                    {
                        try
                        {
                            var v1 = p.GetValue(t1, false);
                            var v2 = p.GetValue(t2, false);
                            var r = Compare(p.Property.Name, p.Property.PropertyType, v1, v2);
                            if(r != null) return null;
                        }
                        catch(Exception)
                        {
                            throw;
                        }
                    }
                    break;
                default:
                    if(!Equals(t1, t2))
                    {
                        return new CompareResult
                        {
                            Name = name,
                            Value1 = t1,
                            Value2 = t2
                        };
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// 深度比较两个对象。
        /// </summary>
        /// <typeparam name="T">对象的数据类型。</typeparam>
        /// <param name="t1">第一个对象的实例。</param>
        /// <param name="t2">第二个对象的实例。</param>
        /// <returns>两个对象的比较结果。</returns>
        public static CompareResult Compare<T>(T t1, T t2) where T : class
        {
            var type = typeof(T);
            return Compare(type.Name, type, t1, t2);
        }
        /// <summary>
        /// 深度比较两个对象，如果发生不匹配则抛出异常。
        /// </summary>
        /// <typeparam name="T">对象的数据类型。</typeparam>
        /// <param name="t1">第一个对象的实例。</param>
        /// <param name="t2">第二个对象的实例。</param>
        public static void CompareThrown<T>(T t1, T t2) where T : class
        {
            var type = typeof(T);
            Compare(type.Name, type, t1, t2).ThrowIfExists();
        }

        #endregion

        #region Locking

        /// <summary>
        /// 采用默认的超时时间（1分钟），锁定指定种子。
        /// </summary>
        /// <typeparam name="TSeed">种子的数据类型。</typeparam>
        /// <param name="seed">生成锁对象实例的种子，将采用默认的 <see cref="EqualityComparer{TSeed}"/> 匹配种子。</param>
        /// <returns>可解锁的对象。</returns>
        public static IDisposable Lock<TSeed>(TSeed seed)
        {
            return Lock(seed, RedisExtensions.DefaultLockTimeout);
        }
        /// <summary>
        /// 给定超时时间，锁定指定种子。
        /// </summary>
        /// <typeparam name="TSeed">种子的数据类型。</typeparam>
        /// <param name="seed">生成锁对象实例的种子，将采用默认的 <see cref="EqualityComparer{TSeed}"/> 匹配种子。</param>
        /// <param name="timeout">锁的超时时间。</param>
        /// <returns>可解锁的对象。</returns>
        public static IDisposable Lock<TSeed>(TSeed seed, TimeSpan timeout)
        {
            if(Equals(seed, default(TSeed))) throw new ArgumentNullException(nameof(seed));

            var s = Seed<TSeed>.LockeableObjects.GetOrAdd(seed, key => new Seed(key));
            return s.LockSeed(timeout);
        }
#if !NET40
        /// <summary>
        /// 采用默认的超时时间（1分钟），异步锁定指定种子。
        /// </summary>
        /// <typeparam name="TSeed">种子的数据类型。</typeparam>
        /// <param name="seed">生成锁对象实例的种子，将采用默认的 <see cref="EqualityComparer{TSeed}"/> 匹配种子。</param>
        /// <returns>可解锁的异步操作。</returns>
        public static Threading.Tasks.Task<IDisposable> LockAsync<TSeed>(TSeed seed)
        {
            return LockAsync(seed, RedisExtensions.DefaultLockTimeout);
        }
        /// <summary>
        /// 给定超时时间，异步锁定指定种子。
        /// </summary>
        /// <typeparam name="TSeed">种子的数据类型。</typeparam>
        /// <param name="seed">生成锁对象实例的种子，将采用默认的 <see cref="EqualityComparer{TSeed}"/> 匹配种子。</param>
        /// <param name="timeout">锁的超时时间。</param>
        /// <returns>可解锁的异步操作。</returns>
        public static Threading.Tasks.Task<IDisposable> LockAsync<TSeed>(TSeed seed, TimeSpan timeout)
        {
            if(Equals(seed, default(TSeed))) throw new ArgumentNullException(nameof(seed));

            var s = Seed<TSeed>.LockeableObjects.GetOrAdd(seed, key => new Seed(key));
            return s.LockSeedAsync(timeout);
        }
#endif
        class Seed
        {
            public readonly object Key;
            readonly SemaphoreSlim _sema;
            readonly SimpleLockItem _releaseItem;
            public Seed(object key)
            {
                Key = key;
                _sema = new SemaphoreSlim(1, 1);
                _releaseItem = new SimpleLockItem(() => _sema.Release());
            }
            public IDisposable LockSeed(TimeSpan timeout)
            {
                if(!_sema.Wait(timeout)) SimpleLockItem.TimeoutError(Convert.ToString(this.Key), timeout);
                return _releaseItem;
            }
#if !NET40
            public async Threading.Tasks.Task<IDisposable> LockSeedAsync(TimeSpan timeout)
            {
                if(!await _sema.WaitAsync(timeout)) SimpleLockItem.TimeoutError(Convert.ToString(this.Key), timeout);
                return _releaseItem;
            }
#endif
        }
        class Seed<TSeed>
        {
            public readonly static ConcurrentDictionary<TSeed, Seed> LockeableObjects = new ConcurrentDictionary<TSeed, Seed>();
        }

        #endregion

        /// <summary>
        /// 加载指定程序集列表的程序集（避免程序集的延迟加载）。
        /// </summary>
        /// <param name="assemblies">程序集列表。</param>
        /// <returns>已加载的程序集列表。</returns>
        public static Assembly[] LoadAssemblies(string assemblies)
        {
            var asemblyList = new List<Assembly>();
            if(string.IsNullOrWhiteSpace(assemblies))
            {
                asemblyList.Add(Assembly.GetEntryAssembly());
            }
            else
            {
                foreach(var assemblyName in assemblies.Split(';'))
                {
                    Assembly assembly;
                    try
                    {
                        assembly = Assembly.Load(assemblyName);
                    }
                    catch(Exception)
                    {
                        if(!File.Exists(assemblyName)) throw new FileNotFoundException("程序集文件不存在。", assemblyName);
                        assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyName));
                    }
                    asemblyList.Add(assembly);
                }
            }
            return asemblyList.ToArray();
        }

        /// <summary>
        /// 创建一个指定常用数据类型的随机值。
        /// </summary>
        /// <typeparam name="TValue">常用的数据类型。</typeparam>
        /// <returns>如果返回默认值，表示不支持此类型的随机生成。</returns>
        public static TValue CreateMockValue<TValue>()
        {
            var value = CreateMockValue(typeof(TValue));
            if(value == null) return default(TValue);
            return (TValue)value;
        }

        /// <summary>
        /// 创建一个指定常用数据类型的随机值。
        /// </summary>
        /// <param name="type">常用的数据类型。</param>
        /// <returns>如果返回 null 值，表示不支持此类型的随机生成。</returns>
        public static object CreateMockValue(Type type)
        {
            if(type == null) throw new ArgumentNullException(nameof(type));
            type = type.GetNullableType();

            if(type == DefineTypes.Guid) return Guid.NewGuid();
            var random = FastRandom.Instance;

            if(type.IsEnum)
            {
                var values = Enum.GetValues(type);
                return values.GetValue(random.Next() % values.Length);
            }
            if(type == DefineTypes.String) return random.NextString(random.Next() % 30 + random.Next(3, 6));
            if(type == DefineTypes.Uri) return new Uri("www." + random.NextString(random.Next() % 10 + random.Next(3, 6)) + ".com");
            if(type == DefineTypes.TimeSpan) return TimeSpan.FromMinutes(random.NextDouble() % random.Next(124, 1025));
            if(type == DefineTypes.DateTime) return DateTime.Now.AddMinutes(random.NextDouble() % random.Next(124, 1025));
            if(type == DefineTypes.DateTimeOffset) return DateTimeOffset.Now.AddMinutes(random.NextDouble() % random.Next(124, 1025));
            if(type == DefineTypes.Boolean) return random.NextBool();

            var randomNumber = random.Next(1, 65535) * (random.NextDouble() + 2.0) + 65535;
            switch(Type.GetTypeCode(type))
            {
                case TypeCode.Decimal: return Math.Round((Decimal)((decimal)randomNumber % Decimal.MaxValue), 2);
                case TypeCode.Byte: return (Byte)(randomNumber % Byte.MaxValue);
                case TypeCode.Char: return (Char)(randomNumber % Char.MaxValue);
                case TypeCode.Double: return Math.Round((Double)(randomNumber % Double.MaxValue), 2);
                case TypeCode.Int16: return (Int16)(randomNumber % Int16.MaxValue);
                case TypeCode.Int32: return (Int32)(randomNumber % Int32.MaxValue);
                case TypeCode.Int64: return (Int64)(randomNumber % Int64.MaxValue);
                case TypeCode.SByte: return (SByte)(randomNumber % SByte.MaxValue);
                case TypeCode.Single: return Math.Round((Single)(randomNumber % Single.MaxValue), 2);
                case TypeCode.UInt16: return (UInt16)(randomNumber % UInt16.MaxValue);
                case TypeCode.UInt32: return (UInt32)(randomNumber % UInt32.MaxValue);
                case TypeCode.UInt64: return (UInt64)(randomNumber % UInt64.MaxValue);
            }
            return null;
        }
        /// <summary>
        /// 创建一个模拟对象。
        /// </summary>
        /// <typeparam name="TModel">对象的数据类型。</typeparam>
        /// <returns>要一个模拟的对象。</returns>
        public static TModel CreateMockModel<TModel>()
        {
            var mapper = TypeMapper.Instance<TModel>.Mapper;
            var m = Activator.CreateInstance<TModel>();
            foreach(var p in mapper.Properties)
            {
                var value = CreateMockValue(p.Property.PropertyType);
                if(value != null) p.SetValue(m, value, false);
            }
            return m;
        }
    }
}
