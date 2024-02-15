class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите путь к папке:");
        string folderPath = Console.ReadLine();

        try
        {
            long initialSize = GetFolderSize(folderPath);
            Console.WriteLine($"Исходный размер папки: {initialSize} байт");

            CleanUnusedFilesAndFolders(folderPath);

            long finalSize = GetFolderSize(folderPath);
            long freedSpace = initialSize - finalSize;
            Console.WriteLine($"Освобождено: {freedSpace} байт");
            Console.WriteLine($"Текущий размер папки: {finalSize} байт");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Ошибка доступа: {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine($"Указанная папка не существует: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    static void CleanUnusedFilesAndFolders(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Папка не найдена: {folderPath}");
        }

        DateTime threshold = DateTime.Now.AddMinutes(-30);

        CleanUnusedFilesAndFoldersRecursively(folderPath, threshold);
    }

    static void CleanUnusedFilesAndFoldersRecursively(string folderPath, DateTime threshold)
    {
        DirectoryInfo directory = new DirectoryInfo(folderPath);

        foreach (FileInfo file in directory.GetFiles())
        {
            if (file.LastAccessTime < threshold)
            {
                try
                {
                    file.Delete();
                    Console.WriteLine($"Удален файл: {file.FullName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при удалении файла {file.FullName}: {ex.Message}");
                }
            }
        }

        foreach (DirectoryInfo subDirectory in directory.GetDirectories())
        {
            CleanUnusedFilesAndFoldersRecursively(subDirectory.FullName, threshold);
        }

        if (directory.GetFiles().Length == 0 && directory.GetDirectories().Length == 0)
        {
            try
            {
                directory.Delete();
                Console.WriteLine($"Удалена пустая папка: {directory.FullName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении пустой папки {directory.FullName}: {ex.Message}");
            }
        }
    }

    static long GetFolderSize(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Папка не найдена: {folderPath}");
        }

        long size = 0;

        foreach (string file in Directory.GetFiles(folderPath))
        {
            size += new FileInfo(file).Length;
        }

        foreach (string subDirectory in Directory.GetDirectories(folderPath))
        {
            size += GetFolderSize(subDirectory);
        }

        return size;
    }
}