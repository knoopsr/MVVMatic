using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {ModelNamespace};
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace {RepositoryNamespace}
{
    public class cls{ModelName}Repository : I{ModelName}Repository
    {
        private ObservableCollection<cls{ModelName}Model> MijnCollectie;

        public cls{ModelName}Repository()
        {
        }

        public bool Delete(cls{ModelName}Model entity)
        {
            (DataTable DT, bool OK, string Boodschap) =
                clsDAL.ExecuteDataTable(Properties.Resources.D_{ModelName},
                clsDAL.Parameter("{ModelName}ID", entity.{ModelName}ID),
                clsDAL.Parameter("ControlField", entity.ControlField),
                clsDAL.Parameter("@ReturnValue", 0));
            if (!OK)
            {
                entity.ErrorBoodschap = Boodschap;
            }
            return OK;
        }

        public cls{ModelName}Model Find()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<cls{ModelName}Model> GetAll()
        {
            GenerateCollection();
            return MijnCollectie;
        }

        private void GenerateCollection()
        {
            SqlDataReader reader = clsDAL.GetData(Properties.Resources.S_{ModelName});
            MijnCollectie = new ObservableCollection<cls{ModelName}Model>();

            while (reader.Read())
            {
                cls{ModelName}Model e = new cls{ModelName}Model()
                {
{PropertyAssignments}
                    ControlField = reader[{LastIndex}]
                };
                MijnCollectie.Add(e);
            }
            reader.Close();
        }

        public cls{ModelName}Model GetById(int id)
        {
            if (MijnCollectie == null)
            {
                GenerateCollection();
            }
            return MijnCollectie.FirstOrDefault(e => e.{ModelName}ID == id);
        }

        public cls{ModelName}Model GetFirst()
        {
            if (MijnCollectie == null)
            {
                GenerateCollection();
            }
            return MijnCollectie.FirstOrDefault();
        }

        public bool Insert(cls{ModelName}Model entity)
        {
            (DataTable DT, bool OK, string Boodschap) =
                clsDAL.ExecuteDataTable(Properties.Resources.I_{ModelName},
{InsertParameters}
                );
            if (!OK)
            {
                entity.ErrorBoodschap = Boodschap;
            }
            return OK;
        }

        public bool Update(cls{ModelName}Model entity)
        {
            (DataTable DT, bool OK, string Boodschap) =
                clsDAL.ExecuteDataTable(Properties.Resources.U_{ModelName},
{UpdateParameters}
                );
            if (!OK)
            {
                entity.ErrorBoodschap = Boodschap;
            }
            return OK;
        }
    }
}
