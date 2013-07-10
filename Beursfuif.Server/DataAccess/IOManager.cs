using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    }
}
