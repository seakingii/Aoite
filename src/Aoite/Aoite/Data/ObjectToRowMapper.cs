using System;
using System.Data;

namespace Aoite.Data
{
    class ObjectToRowMapper : MapperBase<DataRow>
    {
        public object FromValue;
        protected override void Fill()
        {
            PropertyMapper pMapper;
            string name;
            object rowValue, entityValue;

            foreach(DataColumn column in ToValue.Table.Columns)
            {
                name = column.ColumnName;
                rowValue = ToValue[column];
                pMapper = this.Mapper[name];
                if(pMapper == null)
                {
                    this.WarningNotFound(FromValue.GetType().FullName, name);
                    continue;
                }
                entityValue = pMapper.GetValue(FromValue, true) ?? DBNull.Value;
                if(object.Equals(entityValue, rowValue)) continue;
                ToValue[column] = entityValue;
            }
        }
    }
}
