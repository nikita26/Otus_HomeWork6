namespace Otus
{
    static class Programm
    {
        public static void Main(string[] args)
        {
            #region №1 передача делегата в виде параметра

            var users = new List<User>() { 
                new User(){Id = 1, Name ="Nikita", Score = 43.4f},
                new User(){Id = 2, Name ="Vasiliy", Score = 23},
                new User(){Id = 3, Name ="Klim", Score = 53.65f},
                new User(){Id = 4, Name ="Vladimir", Score = 24.53f},
                new User(){Id = 5, Name ="Ivan", Score = 12.4f},
            };

            var max = users.GetMax(GetScore);
            var name = typeof(User);
            Console.ForegroundColor= ConsoleColor.Yellow;
            Console.WriteLine($"Максимальный результат у {max.Name} (Id {max.Id})\n");
            Console.ForegroundColor = ConsoleColor.White;

            #endregion



            #region №2 Сканер файлов
            var fileCount = 0;
            var scanner = new FileSystemScanner(@"C:\");
            scanner.FileFound += (s, e) => {
                var args = (FileArgs)e;
                Console.WriteLine(args.Name);
                fileCount++;
            };
            scanner.Start();
            
            Thread.Sleep(1500); //дать сканеру поработать перед остановкой
            
            scanner.Stop();
            #endregion


        }

        #region №1 передача делегата в виде параметра

        /// <summary>
        /// Получение числового поля для сравнения из объекта класса User
        /// </summary>
        public static float GetScore(User u) => u.Score;

        /// <summary>
        /// Поиск максимума
        /// </summary>
        /// <typeparam name="T">Тип с которым бдует работать метод</typeparam>
        /// <param name="e">Коллекция для поиска</param>
        /// <param name="getParam">Делегат для получения числового значения для сравения из класса</param>
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
        /// <summary>
        /// Событие нахождения нового файла
        /// </summary>
        public event EventHandler FileFound;

        private string startDirectory;
        private CancellationTokenSource source;
        private Task worker;

        public FileSystemScanner(string startDirectory)
        {
            this.startDirectory = startDirectory;
        }

        /// <summary>
        /// Запуск сканера
        /// </summary>
        public Task Start()
        {
            if(worker == null || worker?.Status != TaskStatus.Running)
            {
                source = new CancellationTokenSource();
                var token = source.Token;
                worker = Task.Run(() => { Scann(startDirectory); }, token);
            }
            return worker;
        }
        /// <summary>
        /// Остановка сканера
        /// </summary>
        public void Stop()
        {
            source?.Cancel();
        }

        /// <summary>
        /// Рекурсивный метод сканирование папки
        /// </summary>
        /// <param name="path">путь к текущей папке с которой идет работа</param>
        private void Scann(string path)
        {
            Thread.Sleep(200);  //искуственное торможение сканера, чтобы не успел найти все файлы до принудительной остановки
            if (Directory.Exists(path))
            {
                try
                {
                    var file = Directory.GetFiles(path);
                    foreach (var f in file)
                    {
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
