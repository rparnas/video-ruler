using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Shell32;
using System.IO;

[assembly: AssemblyTitle("video-ruler")]
[assembly: AssemblyProduct("video-ruler")]
[assembly: AssemblyCopyright("Copyright © Ryan Parnas 2016")]

namespace video_ruler
{
  class Program
  {
    static Shell Shell = (Shell)Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));
    static HashSet<string> TestedContainers = new HashSet<string> { "AVI", "MKV", "MOV", "MP4", "MPEG", "MPG", "MTS", "VOB", "WMV" };

    [STAThread]
    static void Main(string[] args)
    {
      if (args.Length != 1 || !Directory.Exists(args[0]))
      {
        Console.WriteLine("Usage: video-ruler <Directory>");
        return;
      }

      var fileCount = 0;
      var totalMinutes = 0.0;
      foreach (var file in GetFiles(new DirectoryInfo(args[0])))
      {
        var ext = file.Extension.ToUpperInvariant().Replace(".", "");
        if (TestedContainers.Contains(ext))
        {
          var length = GetLength(file).TotalMinutes;
          Console.WriteLine(file.Name + ": " + length);

          fileCount++;
          totalMinutes += length;
        }
      }
      Console.WriteLine(fileCount + " files, " + totalMinutes + " minutes.");
    }

    /// <summary>Returns all files in the given directory (and subdirectories).</summary>
    static List<FileInfo> GetFiles(DirectoryInfo directory)
    {
      var ret = new List<FileInfo>();

      foreach (var file in directory.GetFiles())
      {
        ret.Add(file);
      }

      foreach (var subdirectory in directory.GetDirectories())
      {
        ret.AddRange(GetFiles(subdirectory));
      }

      return ret;
    }

    /// <summary>Returns the length of the given video file.</summary>
    static TimeSpan GetLength(FileInfo file)
    {
      var folder = Shell.NameSpace(file.DirectoryName);
      var folderItem = folder.ParseName(file.Name);
      var length = TimeSpan.Parse(folder.GetDetailsOf(folderItem, 27));
      return length;
    }
  }
}
