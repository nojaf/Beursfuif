using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Beursfuif.Server.DataAccess
{
    public class IOManager:IIOManager
    {
        /*
        #region Load
        public  T[] LoadArrayFromXml<T>(string path) where T : class
        {
            if (System.IO.File.Exists(path))
            {
                var arrayType = typeof(T).MakeArrayType();
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(arrayType);
                System.IO.StreamReader fileRead = new System.IO.StreamReader(path);
                var readedObjects = reader.Deserialize(fileRead);
                fileRead.Close();
                return readedObjects as T[];
            }
            return new T[0];
        }

        public  List<T> LoadListFromXml<T>(string path) where T : class
        {
            if (System.IO.File.Exists(path))
            {
                var arrayType = typeof(List<T>);
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(arrayType);
                System.IO.StreamReader fileRead = new System.IO.StreamReader(path);
                var readedObjects = reader.Deserialize(fileRead);
                fileRead.Close();
                return readedObjects as List<T>;
            }
            return new List<T>();
        }

        public  ObservableCollection<T> LoadObservableCollectionFromXml<T>(string path) where T : class
        {
            if (System.IO.File.Exists(path))
            {
                var arrayType = typeof(ObservableCollection<T>);
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(arrayType);
                System.IO.StreamReader fileRead = new System.IO.StreamReader(path);
                var readedObjects = reader.Deserialize(fileRead);
                fileRead.Close();
                return readedObjects as ObservableCollection<T>;
            }
            return new ObservableCollection<T>();
        }

        public T[] LoadArrayFromBinary<T>(string path) where T : class
        {
            if (File.Exists(path))
            {
                var formatter = new BinaryFormatter();
                using (var stream = File.OpenRead(path))
                {
                    return (T[])formatter.Deserialize(stream);
                }
            }
            return null;
        }

        public ObservableCollection<T> LoadObservableCollectionFromBinary<T>(string path) where T : class
        {
            if (File.Exists(path))
            {
                var formatter = new BinaryFormatter();
                using (var stream = File.OpenRead(path))
                {
                    return (ObservableCollection<T>)formatter.Deserialize(stream);
                }
            }
            return null;
        }

        public T LoadObjectFromBinary<T>(string path) where T : class
        {
            if (File.Exists(path))
            {
                var formatter = new BinaryFormatter();
                using (var stream = File.OpenRead(path))
                {
                    return (T)formatter.Deserialize(stream);
                }
            }
            return null;
        }

        public T LoadObjectFromXml<T>(string path) where T : class
        {
            if (File.Exists(path))
            {

                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StreamReader fileRead = new System.IO.StreamReader(path);
                var readedObject = reader.Deserialize(fileRead);
                fileRead.Close();
                return readedObject as T;
            }
            return null;
        }
        #endregion

        #region Save
        public void SaveObservableCollectionToXml<T>(string path,ObservableCollection<T> objects) where T : class
        {        
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<T>));
            System.IO.StreamWriter file = new System.IO.StreamWriter(path,false);
            writer.Serialize(file, objects);
            file.Close();
        }

        public void SaveObjectToXml<T>(string path, T toSaveObject) where T : class
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.StreamWriter file = new System.IO.StreamWriter(path, false);
            writer.Serialize(file, toSaveObject);
            file.Close();
        }

        public void SaveArrayToBinary<T>(string path, T[] objects) where T : class
        {
            var formatter = new BinaryFormatter();
            using (var stream = File.Create(path))
            {
                formatter.Serialize(stream, objects);
            }
        }

        public void SaveObjectToBinary<T>(string path, T toSaveObject) where T : class
        {
            var formatter = new BinaryFormatter();
            using (var stream = File.Create(path))
            {
                formatter.Serialize(stream, toSaveObject);
            }
        }

        public void SaveObservableCollectionToBinary<T>(string path, ObservableCollection<T> objects) where T : class
        {
            var formatter = new BinaryFormatter();
            using (var stream = File.Create(path))
            {
                formatter.Serialize(stream, objects);
            }
        }
        #endregion
        */

        public T Load<T>(string path) where T : class
        {
           if (System.IO.File.Exists(path))
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
            string json = JsonConvert.SerializeObject(entity);
            File.WriteAllText(path, json);
        }
    }
}
