using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    enum FSIMode
    {
        DirectoryInfo = 1,
        FileInfo = 2
    }
    class FarManager
    {
        public int begin = 0;
        public int end = 10;
        public FileSystemInfo[] Content
        {
            get;
            set;
        }
        public int SelectedIndex
        {
            get;
            set;
        }
        public void Show()
        {
            Console.WriteLine("Open: Enter || Delete: Del || Rename: Tab || Back: BackSpace");
            Console.WriteLine();
            Console.Clear();
            Console.ResetColor();
            if (SelectedIndex < 0)
            {
                if(Content.Length < 10)
                {
                    SelectedIndex = end;
                }
                else
                {
                    begin = Content.Length - 10;
                    end = Content.Length;
                    SelectedIndex = Content.Length - 1;
                }
            }
            if(SelectedIndex >= end)
            {
                begin++;
                end++;
            }
            if(SelectedIndex < begin)
            {
                begin--;
                end--;
            }
            if(SelectedIndex >= Content.Length)
            {
                begin = 0;
                end = 10;
                SelectedIndex = 0;
            }
            for(int i = begin; i < Math.Min(end, Content.Length); ++i)
            {
                if(SelectedIndex == i)
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                if(Content[i].GetType() == typeof(DirectoryInfo))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                if(Content[i].GetType() == typeof(FileInfo))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.WriteLine(i + 1 + ".  " + Content[i].Name);
            }
            Console.BackgroundColor = ConsoleColor.Black;

        }
        public void Delete()
        {
            if(Content[SelectedIndex].GetType() == typeof(FileInfo))
            {
                File.Delete(Content[SelectedIndex].FullName);
            }
            else
            {
                Directory.Delete(Content[SelectedIndex].FullName);
            }
        }
        public void ReName()
        {
            Console.Write("Please, Write new name: ");
            string name = Console.ReadLine();
            string path = new DirectoryInfo(Content[SelectedIndex].FullName).Parent.FullName;

            if(Content[SelectedIndex].GetType() == typeof(FileInfo))
            {
                File.Move(Content[SelectedIndex].FullName, path + '/' + name);
            }
            else
            {
                Directory.Move(Content[SelectedIndex].FullName, path + '/' + name);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dir = new DirectoryInfo(Console.ReadLine());
            FarManager l = new FarManager
                {
                Content = dir.GetFileSystemInfos(),
                SelectedIndex = 0,
                };
            Stack<FarManager> history = new Stack<FarManager>();
            history.Push(l);
            bool esc = false;
            FSIMode curMode = FSIMode.DirectoryInfo;
            while (!esc)
            {
                if(curMode == FSIMode.DirectoryInfo)
                {
                    history.Peek().Show();
                }
                ConsoleKeyInfo KeyInfo = Console.ReadKey();
                switch (KeyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        history.Peek().SelectedIndex--;
                        break;
                    case ConsoleKey.DownArrow:
                        history.Peek().SelectedIndex++;
                        break;
                    case ConsoleKey.Enter:
                        int index = history.Peek().SelectedIndex;
                        FileSystemInfo fs = history.Peek().Content[index];
                        if(fs.GetType() == typeof(DirectoryInfo))
                        {
                            curMode = FSIMode.DirectoryInfo;
                            DirectoryInfo dr = fs as DirectoryInfo;
                            history.Push(new FarManager
                            {
                                Content = dr.GetFileSystemInfos(),
                                SelectedIndex = 0
                            });
                        }
                        else
                        {
                            curMode = FSIMode.FileInfo;
                            FileStream fsm = new FileStream(fs.FullName, FileMode.Open, FileAccess.Read);
                            StreamReader sr = new StreamReader(fsm);
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Clear();
                            Console.WriteLine(sr.ReadToEnd());
                        }
                        break;
                    case ConsoleKey.Backspace:
                        if(curMode == FSIMode.DirectoryInfo)
                        {
                            history.Pop();
                        }
                        else
                        {
                            curMode = FSIMode.DirectoryInfo;
                            Console.ResetColor();
                        }
                        break;
                    case ConsoleKey.Delete:
                        history.Peek().Delete();
                        break;
                    case ConsoleKey.Tab:
                        history.Peek().ReName();
                        break;
                    case ConsoleKey.Escape:
                        esc = true;
                        break;
                }

            }
        }
    }
}
