using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using Path = System.IO.Path;

namespace MVVMatic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            KolommenGrid.ItemsSource = kolommen;
        }


        public class KolomDefinitie
        {
            public string KolomNaam { get; set; }
            public string Type { get; set; }
            public bool IsNullable { get; set; } = true;
        }
        private string _databaseName = "MyDatabase";
        public string DatabaseName
        {
            get => _databaseName;
            set
            {
                _databaseName = value;
                OnPropertyChanged();
            }
        }

        private string _schema = "dbo";
        public string Schema
        {
            get => _schema;
            set
            {
                _schema = value;
                OnPropertyChanged();
            }
        }
        private string _folder = "Personen";
        public string Folder
        {
            get => _folder;
            set
            {
                _folder = value;
                OnPropertyChanged();
            }
        }

        private string _table = "Personen";
        public string Table
        {
            get => _table;
            set
            {
                _table = value;
                OnPropertyChanged();
            }
        }
        private string _model = "Persoon";
        public string Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }

        private string _solution = string.Empty;
        public string Solution
        {
            get => _solution;
            set
            {
                _solution = value;
                OnPropertyChanged();
            }
        }

        private string _dataBaseString = string.Empty;
        public string DataBaseString
        {
            get => _dataBaseString;
            set
            {
                _dataBaseString = value;
                OnPropertyChanged();
            }
        }

        private string _namespace = "TestMVVM";
        public string Namespace
        {
            get => _namespace;
            set
            {
                _namespace = value;
                OnPropertyChanged();
            }
        }





        public static List<string> SupportedTypes { get; } = new()
        {
            "string", "int", "bool", "DateTime", "decimal"
        };

        private ObservableCollection<KolomDefinitie> kolommen = new();

        private void BtnVoegKolomToe_Click(object sender, RoutedEventArgs e)
        {
            kolommen.Add(new KolomDefinitie { KolomNaam = "", Type = "string" });
        }



        private string MapToCSharpType(string sqlType)
        {
            return sqlType switch
            {
                "string" => "string",
                "int" => "int",
                "bool" => "bool",
                "DateTime" => "DateTime",
                "decimal" => "decimal",
                "byte[]" => "byte[]",
                _ => "string"
            };
        }
        private string MapToSqlType(string csharpType)
        {
            return csharpType switch
            {
                "string" => "NVARCHAR(100)",
                "int" => "INT",
                "bool" => "BIT",
                "DateTime" => "DATETIME",
                "decimal" => "DECIMAL(18,2)",
                _ => "NVARCHAR(100)"
            };
        }


        private void BtnGenereerAllCodePreview_Click(object sender, RoutedEventArgs e)
        {

            string createTableSql = GenereerCreateTable(Schema, Table, kolommen.ToList());
            string insertProcedureSql = GenereerInsertStoredProcedure(DatabaseName, Schema, Table, Table, kolommen.ToList());
            string selectProcedureSql = GenereerSelectStoredProcedure(DatabaseName, Schema, Table, Table, kolommen.ToList());
            string updateProcedureSql = GenereerUpdateStoredProcedure(DatabaseName, Schema, Table, Table, kolommen.ToList());
            string deleteProcedureSql = GenereerDeleteStoredProcedure(DatabaseName, Schema, Table, Table);


            string modelClass = GenereerModelClass();

            string repositoryInterface = GenereerRepositoryInterface();

            string repositoryClass = GenereerRepositoryClass();
            string dataServiceInterface = GenereerDataServiceInterface();
            string dataServiceClass = GenereerDataServiceClass();
            string viewModelClass = GenereerViewModel();
            string viewUserControl = GenereerXamlUserControl();
            string viewCodeBehind = GenereerXamlCodeBehind();


            OutputTextBox.Text = string.Empty; // Clear previous output

            OutputTextBox.Text += createTableSql;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += insertProcedureSql;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += selectProcedureSql;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += updateProcedureSql;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += deleteProcedureSql;

            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += modelClass;

            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += repositoryInterface;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += repositoryClass;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += dataServiceInterface;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += dataServiceClass;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += viewModelClass;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += viewUserControl;
            OutputTextBox.Text += "\n\n";
            OutputTextBox.Text += viewCodeBehind;





        }


        private string GenereerCreateTable(string schema, string tableName, List<KolomDefinitie> kolommen)
        {
            string template = File.ReadAllText("Templates/DB/CreateTable.tpl"); // Zorg dat deze in de build zit!

            var sb = new StringBuilder();
            for (int i = 0; i < kolommen.Count; i++)
            {
                var kolom = kolommen[i];
                string sqlType = MapToSqlType(kolom.Type);
                string nullStr = kolom.IsNullable ? "NULL" : "NOT NULL";



                sb.AppendLine($"    {kolom.KolomNaam} {sqlType} {nullStr},");
            }


            return template
                .Replace("{Schema}", schema)
                .Replace("{TableName}", tableName)
                .Replace("{Columns}", sb.ToString().TrimEnd('\n', '\r'));
        }

        private string GenereerInsertStoredProcedure(string database, string schema, string tableName, string procedureName, List<KolomDefinitie> kolommen)
        {
            string templatePath = "Templates/DB/InsertProcedure.tpl";

            string template = File.ReadAllText(templatePath);

            // Parameters (@Naam nvarchar(100), ...)
            var parametersSb = new StringBuilder();
            for (int i = 0; i < kolommen.Count; i++)
            {
                var kolom = kolommen[i];
                string sqlType = MapToSqlType(kolom.Type);
                string comma = ",";
                parametersSb.AppendLine($"    @{kolom.KolomNaam} {sqlType}{comma}");
            }

            // Kolomnamen in INSERT INTO (bv. Naam, Leeftijd, ...)
            var insertColsSb = new StringBuilder();
            foreach (var kolom in kolommen)
            {
                insertColsSb.AppendLine($"        {kolom.KolomNaam},");
            }

            // Waarden in VALUES (bv. @Naam, @Leeftijd, ...)
            var insertValsSb = new StringBuilder();
            foreach (var kolom in kolommen)
            {
                insertValsSb.AppendLine($"        @{kolom.KolomNaam},");
            }

            return template
                .Replace("{Database}", database)
                .Replace("{Schema}", schema)
                .Replace("{ProcedureName}", procedureName)
                .Replace("{TableName}", tableName)
                .Replace("{Parameters}", parametersSb.ToString().TrimEnd('\r', '\n'))
                .Replace("{InsertColumns}", insertColsSb.ToString())
                .Replace("{InsertValues}", insertValsSb.ToString());
        }

        private string GenereerSelectStoredProcedure(string database, string schema, string tableName, string procedureName, List<KolomDefinitie> kolommen)
        {
            string template = File.ReadAllText("Templates/DB/SelectProcedure.tpl");

            var selectSb = new StringBuilder();
            for (int i = 0; i < kolommen.Count; i++)
            {
                string comma = i < kolommen.Count - 1 ? "," : "";
                selectSb.AppendLine($"        {kolommen[i].KolomNaam}{comma}");
            }

            return template
                .Replace("{Database}", database)
                .Replace("{Schema}", schema)
                .Replace("{ProcedureName}", procedureName)
                .Replace("{TableName}", tableName)
                .Replace("{SelectColumns}", selectSb.ToString());
        }

        private string GenereerUpdateStoredProcedure(string database, string schema, string tableName, string procedureName, List<KolomDefinitie> kolommen)
        {
            string template = File.ReadAllText("Templates/DB/UpdateProcedure.tpl");

            var parametersSb = new StringBuilder();
            var assignmentsSb = new StringBuilder();

            for (int i = 0; i < kolommen.Count; i++)
            {
                var kolom = kolommen[i];

                string sqlType = MapToSqlType(kolom.Type);

                parametersSb.AppendLine($"    @{kolom.KolomNaam} {sqlType},");

                assignmentsSb.AppendLine($"        {kolom.KolomNaam} = @{kolom.KolomNaam},");
            }

            return template
                .Replace("{Database}", database)
                .Replace("{Schema}", schema)
                .Replace("{ProcedureName}", procedureName)
                .Replace("{TableName}", tableName)
                .Replace("{Parameters}", parametersSb.ToString().TrimEnd('\n', '\r'))
                .Replace("{UpdateAssignments}", assignmentsSb.ToString().TrimEnd('\n', '\r'));
        }

        private string GenereerDeleteStoredProcedure(string database, string schema, string tableName, string procedureName)
        {
            string template = File.ReadAllText("Templates/DB/DeleteProcedure.tpl");

            return template
                .Replace("{Database}", database)
                .Replace("{Schema}", schema)
                .Replace("{ProcedureName}", procedureName)
                .Replace("{TableName}", tableName);
        }

        private string GenereerModelClass()
        {
            string template = File.ReadAllText("Templates/Model/Model.tpl");

            var propsSb = new StringBuilder();
            var valSb = new StringBuilder();

            foreach (var kolom in kolommen)
            {
                // property
                string csharpType = MapToCSharpType(kolom.Type);
                propsSb.AppendLine($@"
                    private {csharpType} _{kolom.KolomNaam};
                    public {csharpType} {kolom.KolomNaam}
                    {{
                        get => _{kolom.KolomNaam};
                        set
                        {{
                            _{kolom.KolomNaam} = value;
                            OnPropertyChanged();
                        }}
                    }}");

                // eenvoudige validatie voor string
                if (csharpType == "string")
                {
                    valSb.AppendLine($@"
                    case nameof({kolom.KolomNaam}):
                        if (string.IsNullOrWhiteSpace({kolom.KolomNaam}))
                        {{
                            error = ""{kolom.KolomNaam} is een verplicht veld."";
                            if (!ErrorList.Contains(nameof({kolom.KolomNaam}))) ErrorList.Add(nameof({kolom.KolomNaam}));
                        }}
                        else
                        {{
                            if (ErrorList.Contains(nameof({kolom.KolomNaam}))) ErrorList.Remove(nameof({kolom.KolomNaam}));
                        }}
                        return error;");
                }
            }

            // fallback voor ToString en DisplayName
            string toStringExpr = string.Join(" + \", \" + ", kolommen.Take(2).Select(k => k.KolomNaam));
            string displayExpr = $"$\"{string.Join(" ", kolommen.Take(2).Select(k => "{" + k.KolomNaam + "}"))}\"";

            return template
                .Replace("{Namespace}", Namespace)
                .Replace("{Folder}", Folder)
                .Replace("{ModelName}", Model)
                .Replace("{Properties}", propsSb.ToString())
                .Replace("{ToStringExpression}", toStringExpr)
                .Replace("{DisplayNameExpression}", displayExpr)
                .Replace("{ValidationRules}", valSb.ToString());
        }
        private string GenereerRepositoryInterface()
        {
            string template = File.ReadAllText("Templates/Repository/RepositoryInterface.tpl");

            return template
                .Replace("{Namespace}", Namespace)
                .Replace("{Folder}", Folder)
                .Replace("{ModelName}", Model);
        }

        private string GenereerRepositoryClass()
        {
            string template = File.ReadAllText("Templates/Repository/RepositoryImplementation.tpl");

            var propertyLines = new StringBuilder();
            var insertParams = new StringBuilder();
            var updateParams = new StringBuilder();

            for (int i = 0; i < kolommen.Count; i++)
            {
                var k = kolommen[i];
                string line = $"                    {k.KolomNaam} = reader[{k.KolomNaam}] != DBNull.Value ? ({MapToCSharpType(k.Type)})reader[{i}] : default,";

                propertyLines.AppendLine(line);

                // insert
                string insert = $"                clsDAL.Parameter(\"{k.KolomNaam}\", entity.{k.KolomNaam}),";
                if (k.Type == "byte[]")
                {
                    insert = $"                clsDAL.Parameter(\"{k.KolomNaam}\", entity.{k.KolomNaam} != null ? (object)entity.{k.KolomNaam} : DBNull.Value, SqlDbType.VarBinary),";
                }
                insertParams.AppendLine(insert);

                // update
                string update = insert; // zelfde als insert
                updateParams.AppendLine(update);
            }

            insertParams.AppendLine("                clsDAL.Parameter(\"@ReturnValue\", 0)");
            updateParams.AppendLine("                clsDAL.Parameter(\"ControlField\", entity.ControlField),");
            updateParams.AppendLine("                clsDAL.Parameter(\"@ReturnValue\", 0)");

            return template
                .Replace("{Namespace}", Namespace)
                .Replace("{Folder}", Folder)
                .Replace("{ModelName}", Model)
                .Replace("{PropertyAssignments}", propertyLines.ToString().TrimEnd('\n', '\r'))
                .Replace("{InsertParameters}", insertParams.ToString().TrimEnd(',', '\n', '\r'))
                .Replace("{UpdateParameters}", updateParams.ToString().TrimEnd(',', '\n', '\r'))
                .Replace("{LastIndex}", kolommen.Count.ToString());
        }

        private string GenereerDataServiceInterface()
        {
            string template = File.ReadAllText("Templates/DataService/DataServiceInterface.tpl");

            return template
                .Replace("{Namespace}", Namespace)
                .Replace("{Folder}", Folder)
                .Replace("{ModelName}", Model);
        }
        private string GenereerDataServiceClass()
        {
            string template = File.ReadAllText("Templates/DataService/DataServiceClass.tpl");

            return template
                .Replace("{Namespace}", Namespace)
                .Replace("{Folder}",Folder)
                .Replace("{ModelName}", Model);
        }

        private string GenereerViewModel()
        {
            string template = File.ReadAllText("Templates/ViewModel/ViewModel.tpl");

            return template
                .Replace("{Namespace}", Namespace)
                .Replace("{Folder}", Folder)
                .Replace("{ModelName}", Model);
        }

        private string GenereerXamlUserControl()
        {
            string template = File.ReadAllText("Templates/View/ViewUserControl.tpl");

            return template
                .Replace("{Namespace}", $"{Namespace}.View.{Folder}")
                .Replace("{ClassName}", $"uc{Model}")
                .Replace("{ViewModelBinding}", $"{Model}ViewModel")
                .Replace("{Title}", Model);
        }

        private string GenereerXamlCodeBehind()
        {
            string template = File.ReadAllText("Templates/View/UserControlCodeBehind.tpl");

            return template
                .Replace("{Namespace}", Namespace)
                .Replace("{Folder}", Folder)
                .Replace("{ModelName}", Model);
        }




        private void BtnSelecteerSolution(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Solution files (*.sln)|*.sln",
                InitialDirectory = Directory.GetCurrentDirectory(),
                Title = "Selecteer een solution file"
            };

            if (dialog.ShowDialog() == true)
            {
                Solution = Path.GetDirectoryName(dialog.FileName);
                //Solution = "C:\\Users\\ronny\\source\\repos\\TestMVVM";

                string solutionFolder = Solution; // <-- die komt uit je OpenFileDialog
                string connString = SettingsReader.FindConnectionString(solutionFolder);
                string dbName = SettingsReader.ExtractDatabaseName(connString);


                DataBaseString = connString;
                DatabaseName = dbName;
            }
        }




        private void SchrijfModelClassNaarBestand()
        {
            string modelCode = GenereerModelClass();

            // Bepaal folderpad en bestandsnaam
            string folder = Path.Combine(Solution, Namespace + ".Model", Folder);
            string bestandspad = Path.Combine(folder, $"cls{Model}Model.cs");

            // Maak de folder aan indien nodig
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Schrijf het bestand weg
            File.WriteAllText(bestandspad, modelCode, Encoding.UTF8);
        }
        private void SchrijfRepositoryInterfaceNaarBestand()
        {
            string interfaceCode = GenereerRepositoryInterface();

            // Folderstructuur: bijv. Solution + Namespace.DAL + Folder
            string folder = Path.Combine(Solution, Namespace + ".DAL", Folder);
            string bestandspad = Path.Combine(folder, $"I{Model}Repository.cs");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
                  

            // Schrijf weg
            File.WriteAllText(bestandspad, interfaceCode, Encoding.UTF8);
       }
        private void SchrijfRepositoryClassNaarBestand()
        {
            string interfaceCode = GenereerRepositoryClass();

            // Folderstructuur: bijv. Solution + Namespace.DAL + Folder
            string folder = Path.Combine(Solution, Namespace + ".DAL", Folder);
            string bestandspad = Path.Combine(folder, $"cls{Model}Repository.cs");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            // Schrijf weg
            File.WriteAllText(bestandspad, interfaceCode, Encoding.UTF8);
       }

        private void SchrijfDataServiceInterfaceNaarBestand()
        {
            string interfaceCode = GenereerDataServiceInterface();

            // Folderstructuur: bijv. Solution + Namespace.DAL + Folder
            string folder = Path.Combine(Solution, Namespace, "DataService", Folder);
            string bestandspad = Path.Combine(folder, $"I{Model}DataService.cs");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            // Schrijf weg
            File.WriteAllText(bestandspad, interfaceCode, Encoding.UTF8);
        }
        private void SchrijfDataServiceClassNaarBestand()
        {
            string interfaceCode = GenereerDataServiceClass();

            // Folderstructuur: bijv. Solution + Namespace.DAL + Folder
            string folder = Path.Combine(Solution, Namespace, "DataService", Folder);
            string bestandspad = Path.Combine(folder, $"cls{Model}DataService.cs");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            // Schrijf weg
            File.WriteAllText(bestandspad, interfaceCode, Encoding.UTF8);
        }
        private void SchrijfViewModelNaarBestand()
        {
            string interfaceCode = GenereerViewModel();

            // Folderstructuur: bijv. Solution + Namespace.DAL + Folder
            string folder = Path.Combine(Solution, Namespace, "ViewModel", Folder);
            string bestandspad = Path.Combine(folder, $"cls{Model}ViewModel.cs");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            // Schrijf weg
            File.WriteAllText(bestandspad, interfaceCode, Encoding.UTF8);
        }

        private void SchrijfViewNaarBestand()
        {
            // Genereer inhoud
            string xamlInhoud = GenereerXamlUserControl();
            string csInhoud = GenereerXamlCodeBehind();

            // Doelmap: Solution + Namespace + \View\{Folder}
            string folder = Path.Combine(Solution, Namespace, "View", Folder);
            string xamlPad = Path.Combine(folder, $"uc{Model}.xaml");
            string csPad = Path.Combine(folder, $"uc{Model}.xaml.cs");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Schrijf beide bestanden weg
            File.WriteAllText(xamlPad, xamlInhoud, Encoding.UTF8);
            File.WriteAllText(csPad, csInhoud, Encoding.UTF8);
       }



        private void BtnGenereerAllCode_Click(object sender, RoutedEventArgs e)
        {
            SchrijfModelClassNaarBestand();
            SchrijfRepositoryInterfaceNaarBestand();
            SchrijfRepositoryClassNaarBestand();
            SchrijfDataServiceInterfaceNaarBestand();
            SchrijfDataServiceClassNaarBestand();
            SchrijfViewModelNaarBestand();
            SchrijfViewNaarBestand();

        }





        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}