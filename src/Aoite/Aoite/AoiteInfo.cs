namespace Aoite
{
    /// <summary>
    /// Aoite 框架的程序集描述。
    /// </summary>
    public static class AoiteInfo
    {
        //<开发阶段>
        //- Pre-alpha：功能不完整的版本。
        //- Alpha：仍然需要测试，其功能亦未完善。
        //- Beta：第一个对外公开的软件版本，是由公众参与的测试阶段。
        //- RC：最终产品的候选版本，如果未出现问题则可发布成为正式版本。
        //<发布阶段>
        //- RTM：可用于生产环境的版本，又称为正式版。
        //- Stable：修复问题，扩展功能，优化性能。
        internal const string Stage = "Beta";
        internal const string Name = "Aoite";
        internal const string DescriptionSuffix = "For .NET Framework " +
#if NET40
            "4.0"
#elif NET45
            "4.5"
#elif NET46
            "4.6"
#endif
            + " runtime. See http://www.aoite.com.";
        /// <summary>
        /// 获取包含密钥对的文件的路径。
        /// </summary>
        public const string SnkKeyFile = @"..\..\comm\aoite.snk";

        /// <summary> 
        /// 获取简写化的版本号。
        /// </summary>
        public const string Version = "3.16";
        /// <summary>
        /// 公司。
        /// </summary>
        public const string Company = Name + " Organization Co.,";
        /// <summary> 
        /// 版权。
        /// </summary>
        public const string Copyright = "Copyright ©" + Name + " 2016 All Right Reserved";

        /// <summary> 
        /// Aoite 核心套件。
        /// </summary>
        public static class Core
        {
            /// <summary> 
            /// 程序集的产品名称。
            /// </summary>
            public const string Product = Name + " Core " + Stage;
            /// <summary> 
            /// 程序集详细的版本。
            /// </summary>
            public const string AssemblyVersion = Version + ".1.6";
            /// <summary> 
            /// 程序集的简单描述。
            /// </summary>
            public const string Description = Name + " 核心套件。";
            /// <summary> 
            /// 程序集的唯一标识。
            /// </summary>
            public const string Guid = "5b0d8879-660c-4701-8117-c1499c9b65e7";
        }

        /// <summary> 
        /// Aoite Windows Forms 套件。
        /// </summary>
        public static class Windows
        {
            /// <summary>
            /// 程序集的产品名称。
            /// </summary>
            public const string Product = Name + " Windows " + Stage;
            /// <summary>
            /// 程序集的唯一标识。
            /// </summary>
            public const string Guid = "14d7026d-bd80-4191-8b87-56046b7ad731";
            /// <summary> 
            /// 程序集详细的版本。
            /// </summary>
            public const string AssemblyVersion = Version + ".0.0";
            /// <summary>
            /// 程序集的简单描述。
            /// </summary>
            public const string Description = Name + " Windows Forms 基础套件。";
        }

    }
}
