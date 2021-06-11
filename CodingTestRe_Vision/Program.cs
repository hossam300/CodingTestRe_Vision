using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingTestRe_Vision
{
    class Program
    {
        static void Main(string[] args)
        {
            // Call IDataStore implementation here
            // 
            // Adding DI for DataStore and Logger 
            var serviceProvider = new ServiceCollection()
           .AddSingleton<IDataStore<MemoryData>, DataStore>()
           //  .AddSingleton<MemoryData, DataStroe>()
           .BuildServiceProvider();
            var db = new List<MemoryData>();
            DataStore data = new DataStore(db);
            // Create 
            data.Create(new MemoryData { key = "testkey", data = "testdata" });
            // Read with notExsit key
            data.TryRead("testkey2", out object data1);
            // Read with Exsit key
            data.TryRead("testkey", out object data2);
            // Update
            data.Update(new MemoryData { key = "testkey", data = "testdata2" });
            // Remove
            data.Delete("testkey");
            Console.ReadLine();
        }
    }
    public class MemoryData
    {
        public string key { get; set; }
        public string data { get; set; }
    }
    public interface IDataStore<T> where T : class
    {
        bool Create(T entity);
        bool TryRead(object key, out object data);
        bool Update(T entity);
        bool Delete(object key);
    }
    public class DataStore : IDataStore<MemoryData>, ILogger
    {
        public readonly List<MemoryData> _db;

        public DataStore(List<MemoryData> db)
        {
            _db = db;
        }

        public bool Create(MemoryData entity)
        {
            try
            {
                _db.Add(entity);
                Write($"Entry with key '{entity.key}' and data '{entity.data}' created");
                return true;
            }
            catch (OutOfMemoryException e)
            {
                Write($"Exception: '{e.Message}' ");
                return false;
            }
            catch (Exception ex)
            {
                Write($"Exception: '{ex.Message}' ");
                return false;
            }
        }

        public bool Delete(object key)
        {
            try
            {
                _db.Remove(_db.Find(x => x.key == key));
                Write($" Entry with key '{key}' removed");
                return true;
            }
            catch (OutOfMemoryException e)
            {
                Write($"Exception: '{e.Message}' ");
                return false;
            }
            catch (Exception ex)
            {
                Write($"Exception: '{ex.Message}' ");

                return false;
            }
        }

        public bool TryRead(object key, out object data)
        {
            try
            {
                var dataItem = _db.FirstOrDefault(x => x.key == key);
                if (dataItem == null)
                {
                    data = null;
                    Write($" Entry with key '{key}' does not exists");
                }
                else
                {
                    data = dataItem.data;
                    Write($" Entry with key '{key}' returned");
                }

                return true;
            }
            catch (OutOfMemoryException e)
            {
                Write($"Exception: '{e.Message}' ");
                data = null;
                return false;
            }
            catch (Exception ex)
            {
                data = "";
                Write($"Exception: '{ex.Message}' ");
                return false;
            }
        }
        public bool Update(MemoryData entity)
        {
            try
            {
                var element = _db.FirstOrDefault(x => x.key == entity.key);
                element.data = entity.data;
                Write($"Entry with key '{entity.key}' update with data '{entity.data}'");
                return true;
            }
            catch (OutOfMemoryException e)
            {
                Write($"Exception: '{e.Message}' ");
                return false;
            }
            catch (Exception ex)
            {
                Write($"Exception: '{ex.Message}' ");
                return false;
            }
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
    public interface ILogger
    {
        void Write(string message);
    }

}