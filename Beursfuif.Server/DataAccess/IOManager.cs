using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Beursfuif.Server.DataAccess
{
    public  class IOManager
    {
        public  T[] LoadArrayFromXml<T>(string path) where T : class
        {
            if (System.IO.File.Exists(path))
            {
                var arrayType = typeof(T).MakeArrayType();
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(arrayType);
                System.IO.StreamReader fileRead = new System.IO.StreamReader(path);
                var readedObjects = reader.Deserialize(fileRead);
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
                return readedObjects as ObservableCollection<T>;
            }
            return new ObservableCollection<T>();
        }


        public void SaveObservableCollectionToXml<T>(string path,ObservableCollection<T> objects) where T : class
        {        
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<T>));
            System.IO.StreamWriter file = new System.IO.StreamWriter(path,false);
            writer.Serialize(file, objects);
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

        public T[] LoadArrayFromBinary<T>(string path) where T : class
        {
            if (File.Exists(path))
            {
                try
                {
                    var formatter = new BinaryFormatter();
                    using (var stream = File.OpenRead(path))
                    {
                        return (T[])formatter.Deserialize(stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could read intervalstream, " + ex.Message);
                    return null;
                }
            }
            return null;
        }

        public ObservableCollection<T> LoadObservableCollectionFromBinary<T>(string path) where T : class
        {
            if (File.Exists(path))
            {
                try
                {
                    var formatter = new BinaryFormatter();
                    using (var stream = File.OpenRead(path))
                    {
                        return (ObservableCollection<T>)formatter.Deserialize(stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could read stream, " + ex.Message);
                    return null;
                }
            }
            return null;
        }

        public T LoadObjectFromBinary<T>(string path) where T : class
        {
            if (File.Exists(path))
            {
                try
                {
                    var formatter = new BinaryFormatter();
                    using (var stream = File.OpenRead(path))
                    {
                        return (T)formatter.Deserialize(stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could read stream, " + ex.Message);
                    return null;
                }
            }
            return null;
        }
    }
}
