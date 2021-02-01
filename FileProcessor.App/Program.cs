using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FileProcessor.App
{
    class Program
    {
        static string GetConnectionString()
        {
            var userName = Environment.GetEnvironmentVariable("SQL_USER");
            var password = Environment.GetEnvironmentVariable("SQL_PASSWORD");
            var hostName = Environment.GetEnvironmentVariable("SQL_HOST");

            var sqlServer = $"Server={hostName};Database=Bunnings;User Id={userName};Password={password}";
            
            return sqlServer;
        }
        static PayLoad GetPayLoad(string filePath)
        {
            PayLoad payLoad = null;
            var errFileName = Path.Combine(errorFolder, Path.GetFileName(filePath));
            try
            {
                payLoad = JsonSerializer.Deserialize<PayLoad>(File.ReadAllText(filePath));
            }
            catch (Exception e)
            {
                Console.WriteLine($"invalid json file {filePath} {e.Message}");
                File.Move(filePath, errFileName, true);
                return null;
            }

            if (payLoad.IsInvalidQuantitySum)
            {
                Console.WriteLine($"Invalid Quantity Sum {Path.GetFileName(filePath)}");
                File.Move(filePath, errFileName, true);
                return null;
            }
            if (payLoad.IsInvalidRecordCount)
            {
                Console.WriteLine($"Invalid Record Count {Path.GetFileName(filePath)}");
                File.Move(filePath, errFileName, true);
                return null;
            }
            return payLoad;
        }
        static void ProcessFile(string filePath)
        {
            var payLoad = GetPayLoad(filePath);
            if (payLoad == null)
                return;
            Console.WriteLine($"Starting importing {filePath}");
            var contextOptions = new DbContextOptionsBuilder<AppDBContext>()
               .UseSqlServer(GetConnectionString())
               .Options;
            var dbContext = new AppDBContext(contextOptions);
            try
            {
                if(dbContext.ImportPayLoad(payLoad) == false) {
                    Console.WriteLine($"Skipping {filePath} as it is already processed");
                }
                var destFileName = Path.Combine(processedFolder, Path.GetFileName(filePath));
                File.Move(filePath, destFileName, true);
            }
            catch(DbUpdateException e)
            {
                Console.WriteLine($"Error while updating db, duplicate transmission id ? {e.Message}");
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error while updating db {e.Message}");
            }
            finally
            {
                Console.WriteLine($"Completd importing {Path.GetFileName(filePath)}");
                // GroupBy brings every thing in memory, it may be efficient to put all this in a stored procedure or a direct sql statement.
                var lst = dbContext.Products.AsEnumerable().GroupBy(x => x.CategoryL3).Select(x => new {
                    Category = x.Key,
                    Stores = x.GroupBy(y => y.Location).Select(z => new { StoreName = z.Key, StockQuantity = z.Sum(a => a.Quantity) })
                });
                foreach(var cat in lst)
                {
                    foreach(var store in cat.Stores)
                    {
                        Console.WriteLine($"{cat.Category} - {store.StoreName} - {store.StockQuantity}");
                    }
                }
            }
        }
        static void OnFileCreated(object source, FileSystemEventArgs e)
        {
            lock(lockObject)
            {
                if(File.Exists(e.FullPath)) // to make sure if the file  has not been picked up already
                    ProcessFile(e.FullPath);
            }
            
        }
        static void ProcessDirectory(string folderName)
        {
            lock (lockObject)
            {
                var files = Directory.GetFiles(folderName);
                foreach (var f in files)
                {
                    ProcessFile(f);
                }
            }
            
        }
        // ProcessDirectory and OnFileCreated do not need to compete with each other, other wise it might generate false errors when both try to process the same file
        static object lockObject = new object(); 
        static string processedFolder;
        static string errorFolder;
        static string inputFolder = "./in";
        static void Main(string[] args)
        {
            Console.WriteLine($"starting file system watcher on {inputFolder}");
            processedFolder = Path.Combine(inputFolder, "processed");
            errorFolder = Path.Combine(inputFolder, "err");
            Directory.CreateDirectory(processedFolder);
            Directory.CreateDirectory(errorFolder);
            FileSystemWatcher watcher = new FileSystemWatcher()
            {
                Path = inputFolder,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.DirectoryName
            | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            watcher.Created += OnFileCreated;
            while (true)
            {
                System.Threading.Thread.Sleep(60*1000);
                ProcessDirectory(inputFolder);
            }
        }
    }
}