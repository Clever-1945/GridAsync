using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsyncUI;

public static class AsyncApp
{
    /// <summary>
    /// Полный путь до файла приложения
    /// </summary>
    public static string ApplicationFileName { get; } = Environment.ProcessPath;

    /// <summary>
    /// Папка где расположено приложение
    /// </summary>
    public static string ApplicationDirectory { get; } = Path.GetDirectoryName(ApplicationFileName);
}
