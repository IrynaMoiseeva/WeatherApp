using System;
using MvxForms.Core;
using SQLite;
using Xamarin.Forms;

namespace MvxForms.Core
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);

        string GetFolderPath();
    }


    public class DataBaseService
    {
        #region Fields

        private readonly SQLiteAsyncConnection _database;

        #endregion
       

        public DataBaseService(string fileName)
        {
            // Resolve dependency from different platforms
            var databasePath = DependencyService.Get<IFileHelper>().GetLocalFilePath(fileName);

            _database = new SQLiteAsyncConnection(databasePath);

            //  _database.CreateTableAsync<SettingsModel>().Wait();

        }

    }
}
