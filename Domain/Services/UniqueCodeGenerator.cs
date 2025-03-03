using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Domain.Services
{
    public static class UniqueCodeGenerator
    {
        public static string GenerateCustomCode( )
        {
            string shortGuid = GenerateShortCode();
            return $"{"USR"}-{shortGuid}";
        }

        private static string GenerateShortCode()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray())
                .Replace("=", "")
                .Replace("+", "")
                .Replace("/", "");

            return base64.Substring(0, 6);
        }
    }
}

