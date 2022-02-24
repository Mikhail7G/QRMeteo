using QRMeteo.Service;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace QRMeteo
{
    public partial class App : Application
    {
        public const string dbName = "InventoryDB";
        private static SqlDBControllerService database;

        public static SqlDBControllerService Database {

            get
            {
                if(database==null)
                {
                    database = new SqlDBControllerService(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName));
                }
                return database;
            }

             private set => database = value; }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
