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
            set {
            _databaseName = value;
                OnPropertyChanged();
            }
        }

        private string _schemaFolder = "dbo";
        public string SchemaFolder
        {
            get => _schemaFolder;
            set => _schemaFolder = value;
        }

        private string _tableModel = "MyTable";
        public string TableModel
        {
            get => _tableModel;
            set => _tableModel = value;
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

            string createTableSql = GenereerCreateTable(SchemaFolder, TableModel, kolommen.ToList());
            string insertProcedureSql = GenereerInsertStoredProcedure(DatabaseName, SchemaFolder, TableModel, TableModel, kolommen.ToList());
            string selectProcedureSql = GenereerSelectStoredProcedure(DatabaseName, SchemaFolder, TableModel, TableModel, kolommen.ToList());
            string updateProcedureSql = GenereerUpdateStoredProcedure(DatabaseName, SchemaFolder, TableModel, TableModel, kolommen.ToList());
            string deleteProcedureSql = GenereerDeleteStoredProcedure(DatabaseName, SchemaFolder, TableModel, TableModel);


            string modelClass = GenereerModelClass("MyNamespace", SchemaFolder, TableModel, kolommen.ToList());

            string repositoryInterface = GenereerRepositoryInterface("MyNamespace", SchemaFolder, TableModel);


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

        private string GenereerModelClass(string namespaceName, string schema, string modelName, List<KolomDefinitie> kolommen)
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
                .Replace("{Namespace}", namespaceName)
                .Replace("{Folder}", schema)
                .Replace("{ModelName}", modelName)
                .Replace("{Properties}", propsSb.ToString())
                .Replace("{ToStringExpression}", toStringExpr)
                .Replace("{DisplayNameExpression}", displayExpr)
                .Replace("{ValidationRules}", valSb.ToString());
        }

        private string GenereerRepositoryInterface(string namespaceName, string schema, string modelName)
        {
            string template = File.ReadAllText("Templates/Repository/RepositoryInterface.tpl");

            return template
                .Replace("{Namespace}", namespaceName)
                .Replace("{Folder}", schema)
                .Replace("{ModelName}", modelName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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

                string solutionFolder = Solution; // <-- die komt uit je OpenFileDialog
                string connString = SettingsReader.FindConnectionString(solutionFolder);
                string dbName = SettingsReader.ExtractDatabaseName(connString);


                DataBaseString = connString;
                DatabaseName = dbName;



            }
        }
        












        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}