using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Services
{
    public interface IStorageService
    {
        Task<T> Get_Value<T>(string key);

        Task Save_Value<T>(string Key, T ValueToSave);

        void Delete_Value(string Key);
    }
}
