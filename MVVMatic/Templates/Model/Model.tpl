using {Namespace}.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace {Namespace}.Model.{Folder}
{
    public class cls{ModelName}Model : clsCommonModelPropertiesBase, IDataErrorInfo
    {
{Properties}

        public override string ToString()
        {
            return {ToStringExpression};
        }

        public string {ModelName}DisplayName
        {
            get
            {
                return {DisplayNameExpression};
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
{ValidationRules}
                    default:
                        error = null;
                        return error;
                }
            }
        }
    }
}
