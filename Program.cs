namespace Otus
{
    static class Programm
    {
        public static void Main(string[] args)
        {
            #region №1 передача делегата в виде параметра

            var users = new List<User>() { 
                new User(){Id= 1,Name="Nikita",Score=43.4f},
                new User(){Id= 2,Name="Vasiliy",Score=23},
                new User(){Id= 3,Name="Klim",Score=53.65f},
                new User(){Id= 4,Name="Vladimir",Score=24.53f},
                new User(){Id= 5,Name="Ivan",Score=12.4f},
            };

            var max = users.GetMax(GetScore);
            var name = typeof(User);
            #endregion



            #region №2 Сканер файлов
            var scanner = new FileSystemScanner(@"C:\work\OtusHomeWork\Otus_HomeWork6");
            scanner.FileFound += (s, e) => {
                var args = (FileArgs)e;
                Console.WriteLine(args.Name);

                //Принудительная остановка при выполнения условия
                if(args.Name.Contains(".txt"))
                    scanner.Stop();
            };
            scanner.Start();
            #endregion


        }

        #region №1 передача делегата в виде параметра

        /// <summary>
        /// Получение числового поля для сравнения из объекта
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static float GetScore(User u) => u.Score;
        public static T GetMax<T>(this IEnumerable<T> e, Func<T, float> getParam) where T : class
        {
            var max = e.FirstOrDefault();
            foreach (var item in e)
            {
                var maxValue = getParam?.Invoke(max);
                var currentValue = getParam?.Invoke(item);
                if (maxValue < currentValue)
                    max = item;
            }
            return max;
        }
        #endregion

    }

    #region №1 передача делегата в виде параметра
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Score { get; set; }
    }

    #endregion


    #region №2 Сканер файлов
    /// <summary>
    /// Сканер файловой системы
    /// </summary>
    public class FileSystemScanner
    {
        public event EventHandler FileFound;

        private string startDirectory;
        private bool work;

        public FileSystemScanner(string startDirectory)
        {
            work = true;
            this.startDirectory = startDirectory;
        }

        /// <summary>
        /// Запуск сканера
        /// </summary>
        public void Start()
        {
            Scann(startDirectory);
        }
        public void Stop()
        {
            work = false;
        }

        private void Scann(string path)
        {
            if (Directory.Exists(path) && work)
            {
                try
                {
                    var file = Directory.GetFiles(path);
                    foreach (var f in file)
                    {
                        if (!work) break;

                        var newFile = new FileArgs()
                        {
                            Name = Path.GetFileName(f),
                            Path = f
                        };
                        FileFound?.Invoke(this, newFile);

                    }

                    var args = new EventArgs();
                    var dir = Directory.GetDirectories(path);
                    foreach (var d in dir)
                        Scann(d);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }

    public class FileArgs : EventArgs
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
    #endregion

}
