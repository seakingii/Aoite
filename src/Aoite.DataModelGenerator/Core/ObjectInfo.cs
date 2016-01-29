using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Aoite.Data;

namespace Aoite.DataModelGenerator
{
    public class ObjectInfo
    {
        private string _Name;
        public string Name { get { return this._Name; } set { this._Name = value; } }


        private string _Comments;
        public string Comments
        {
            get
            {
                if(string.IsNullOrEmpty(this._Comments)) return this.Name;
                return this._Comments;
            }
            set { this._Comments = value; }
        }

        private string _ClassName;
        public string ClassName
        {
            get
            {
                if(string.IsNullOrEmpty(this._ClassName)) this._ClassName = this._Name.Replace(' ', '_').Replace('-', '_');
                return this._ClassName;
            }
            set { this._ClassName = value; }
        }

        public ObjectType Type { get; set; }

        public override string ToString()
        {
            if(string.IsNullOrEmpty(this._Comments))
                return this.Name;
            return this.Name + " - " + this.Comments;
        }

        public string GenerateCode(IDbEngine engine, SettingInfo setting, string sql)
        {
            List<ColumnInfo> columns = new List<ColumnInfo>();
            engine.Execute(sql).ToReader(reader =>
            {
                reader.Read();
                for(int i = 0; i < reader.FieldCount; i++)
                {
                    var column = new ColumnInfo()
                    {
                        Name = reader.GetName(i),
                    };

                    column.SetType(reader.GetFieldType(i));
                    columns.Add(column);
                }
            });

            var code = this.GenerateCode(columns, setting, "自定义语句");
            code = "/*********** SQL **********\r\n\r\n"
                     + sql
                     + "\r\n\r\n********** SQL ***********/\r\n\r\n"
                     + code;
            return code;
        }
        private string GenerateCode(List<ColumnInfo> columns, SettingInfo setting, string owner)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using Aoite.Data;");
            builder.AppendLine();
            builder.Append("namespace ");
            builder.AppendLine(setting.Namespace);
            builder.AppendLine("{");
            builder.AppendLine("    /// <summary>");
            builder.Append("    /// ");
            if(!string.IsNullOrEmpty(this._Comments))
            {
                if(!this._Comments.StartsWith("表示一个")) builder.Append("表示一个");
                builder.Append(this._Comments);
                if(this._Comments.Last() != '。') builder.Append("。");
            }
            builder.AppendLine();
            //builder.Append(owner);
            //builder.Append("“");
            //builder.Append(this.Name);
            //builder.AppendLine("”对应的实体类型。");
            builder.AppendLine("    /// </summary>");
            if(!string.Equals(this.Name, this.ClassName, StringComparison.CurrentCultureIgnoreCase))
            {
                builder.Append("    [Table(\"");
                builder.Append(this.Name);
                builder.AppendLine("\")]");
            }
            builder.Append("    public partial class ");
            builder.AppendLine(this.ClassName);
            builder.AppendLine("    {");
            builder.AppendLine();

            foreach(var item in columns)
            {
                builder.AppendLine(item.GenerateCode(this, setting, owner));
                builder.AppendLine();
            }
            builder.AppendLine("    }");
            builder.AppendLine("}");
            return builder.ToString();
        }
        public string GenerateCode(IDbEngine engine, SettingInfo setting)
        {
            var columnInfos = engine.Execute(@"
SELECT
a.Name,
t.name as DbType,
a.max_length as MaxLength,
IsPrimaryKey=case when exists(SELECT 1 FROM sys.objects where type='PK' and name in (
SELECT name FROM sys.indexes WHERE index_id in(
SELECT indid FROM sys.sysindexkeys WHERE id = a.object_id AND colid=a.column_id
))) then 1 else 0 end,
IsNullable=a.is_nullable,
Comments=isnull(g.[value],'')
FROM sys.columns a
inner join sys.objects d on a.object_id=d.object_id and d.name<>'dtproperties'
inner join sys.types t on a.user_type_id=t.user_type_id
left join sys.extended_properties g on a.object_id=g.major_id and a.column_id=g.minor_id
WHERE d.Name=@tableName
ORDER BY a.column_id", "@tableName", this.Name)
                                                               .ToEntities<ColumnInfo>();
            //var table = engine.Execute("SELECT * FROM " + this.Name + " WHERE 1=2").ToTable();
            //var columns = table.Columns;
            //foreach(var item in columnInfos)
            //{
            //    item.SetType(columns[item.Name].DataType);
            //}
            return GenerateCode(columnInfos, setting, this.Type == ObjectType.Table ? "数据库表对象" : "数据库视图对象");

        }
    }

    public class ColumnInfo
    {
        private string _Name;
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        private string _PropertyName;
        public string PropertyName
        {
            get
            {
                if(string.IsNullOrEmpty(this._PropertyName)) this._PropertyName =
                this._Name.Replace(' ', '_').Replace('-', '_');
                return this._PropertyName;
            }
            set { this._PropertyName = value; }
        }

        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public string Comments { get; set; }

        public int MaxLength { get; set; }
        public string DbType { get; set; }

        public bool IsUnicodeCharacher { get; private set; }


        private string ParseDbType()
        {
            var type = (SqlDbType)Enum.Parse(typeof(SqlDbType), this.DbType, true);
            switch(type)
            {
                case SqlDbType.BigInt: return GetTypeName("long");

                case SqlDbType.Timestamp:
                case SqlDbType.Image:
                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                    return "byte[]";

                case SqlDbType.Bit: return GetTypeName("bool");

                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    this.IsUnicodeCharacher = true;
                    return "string";


                case SqlDbType.Char:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return "string";

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.DateTime2:
                    return GetTypeName("DateTime");

                case SqlDbType.Time: return GetTypeName("TimeSpan");

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return GetTypeName("decimal");

                case SqlDbType.Float: return GetTypeName("double");
                case SqlDbType.Int: return GetTypeName("int");

                case SqlDbType.Real: return GetTypeName("float");
                case SqlDbType.UniqueIdentifier: return GetTypeName("Guid");
                case SqlDbType.SmallInt: return GetTypeName("short");
                case SqlDbType.TinyInt: return GetTypeName("byte");

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return "object";

                case SqlDbType.Structured: return "DataTable";
                case SqlDbType.DateTimeOffset: return GetTypeName("DateTimeOffset");
                default:
                    throw new ArgumentOutOfRangeException("sqlType");
            }
        }
        private string _PropertyType;

        public string PropertyType
        {
            get
            {
                if(string.IsNullOrEmpty(_PropertyType))
                {
                    this._PropertyType = this.ParseDbType();
                }
                return _PropertyType;
            }
            set { _PropertyType = value; }
        }

        public string GetTypeName(string typeName)
        {
            if(IsNullable) return typeName + "?";
            return typeName;
        }

        private string GetType(string type)
        {

            if(type == Types.Boolean.Name) return GetTypeName("bool");
            else if(type == Types.Int16.Name) return GetTypeName("short");
            else if(type == Types.Int32.Name) return GetTypeName("int");
            else if(type == Types.Int64.Name) return GetTypeName("long");
            else if(type == Types.Single.Name) return GetTypeName("float");
            else if(type == Types.Double.Name) return GetTypeName("double");
            else if(type == Types.Decimal.Name) return GetTypeName("decimal");
            else if(type == Types.DateTime.Name) return GetTypeName("DateTime");
            else if(type == Types.String.Name) return "string";
            else if(type == Types.ByteArray.Name) return "byte[]";
            return type;
        }

        public void SetType(Type type)
        {
            this.PropertyType = this.GetType(type.Name);
        }

        const string Format1 = "        public {0} {1} {{ get; set; }}";

        private string GenerateColumnAttribute()
        {
            var eqName = string.Equals(this.PropertyName, this.Name, StringComparison.CurrentCultureIgnoreCase);
            if(eqName)
            {
                return this.IsPrimaryKey ? "Column(true)" : string.Empty;
            }
            return $"Column(\"{this.Name}\"{(this.IsPrimaryKey ? ",true" : string.Empty)})";
        }

        private string GenerateNotNullAttribute() => this.IsNullable ? string.Empty : "NotNull";

        private string GenerateStringLengthAttribute()
        {
            if(this.PropertyType == "string" && this.MaxLength > 0)
            {
                return $"StringLength({this.MaxLength},{this.IsUnicodeCharacher.ToString().ToLower()})";
            }
            return string.Empty;
        }


        private string GenerateAttributes()
        {
            var attrs = new string[] { this.GenerateColumnAttribute(), this.GenerateNotNullAttribute(), this.GenerateStringLengthAttribute() };

            return attrs.Join(start: "[", end: "]");
        }
        private string InnerGenerateCode(ObjectInfo objectInfo, SettingInfo setting)
        {
            var attrs = this.GenerateAttributes();
            var f = (attrs.Length > 0 ? ("        " + attrs + "\r\n") : string.Empty) + Format1;
            return string.Format(f, this.PropertyType, this.PropertyName);
        }

        public string GenerateCode(ObjectInfo objectInfo, SettingInfo setting, string owner)
        {
            var comments = this.Comments;
            if(!string.IsNullOrEmpty(comments))
            {
                if(!comments.StartsWith("设置或获取一个值，表示")) comments = ("设置或获取一个值，表示") + comments;
                if(comments.Last() != '。') comments += "。";
            }
            return @"        /// <summary>
        /// " + comments + @"
        /// </summary>
" + this.InnerGenerateCode(objectInfo, setting);
        }
    }
}
