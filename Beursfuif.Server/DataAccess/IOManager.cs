using Newtonsoft.Json;
using System;
using System.IO;
using Beursfuif.Server.Entity;

namespace Beursfuif.Server.DataAccess
{
    public class IOManager:IIOManager
    {
        public T Load<T>(string path) where T : class
        {
           if (File.Exists(path))
           {
               string json = File.ReadAllText(path);
               if (!string.IsNullOrEmpty(json))
               {
                   T entity = JsonConvert.DeserializeObject<T>(json);
                   return entity;
               }
           }

            return default(T);        
        }

        public void Save<T>(string path, T entity) where T : class
        {
            try
            {
                string json = JsonConvert.SerializeObject(entity);
                File.WriteAllText(path, json);     
            }
            catch (Exception ex)
            {
                LogManager.AppendToLog(new BL.LogMessage()
                {
                    Message = $"Couldn't write to {ex.Message}",
                    Type = LogType.ERROR
                });
            }
   
        }
    }
}
