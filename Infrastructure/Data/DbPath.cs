using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Infrastructure.Data
{
    public class DbPath
    {
        public static string GetPath(string nameDb)
        {
            string pathDbSqlite = string.Empty;

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                pathDbSqlite = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameDb);
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                pathDbSqlite = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameDb);

            }

            return pathDbSqlite;
        }
    }
}
