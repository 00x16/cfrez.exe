namespace Biesi.CfRez
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    delegate int RezCompiler(string cmd, string rezFile, string targetDirectory, bool lithRez, string extension);

    class Program
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        private static void DisplayHelp()
        {
            Console.WriteLine("CFREZ 1.0 (Dec-18-2015) Copyright (C) 2015 Nobody");
            Console.WriteLine("Usage: CFREZ <commands> <rez file name> [parameters]");
            Console.WriteLine("Commands: c <rez file name> <root directory to read> [extension[;]] - Create");
            Console.WriteLine("          v <rez file name>                          - View");
            Console.WriteLine("          x <rez file name> <directory to output to> - Extract");
            Console.WriteLine("Option:   v                                          - Verbose");
            Console.WriteLine("Option:   z                                          - Warn zero len");
            Console.WriteLine("Option:   l                                          - Lower case ok\n");
            Console.WriteLine("Example: lithrez cv foo.rez c:\\foo *.ltb;*.dat;*.dtx");
            Console.WriteLine("          (would create rez file foo.rez from the contents of the");
            Console.WriteLine("          directory \"c:\\foo\" where files with extensions ltb dat and");
            Console.WriteLine("          dtx are added, the verbose option would be turned on)\n");
        }

        static int Main(string[] args)
        {
            try
            {
                IntPtr formatBase = LoadLibrary("cfrezformat.dll");
                if (formatBase == IntPtr.Zero)
                {
                    Console.WriteLine("ERROR! Unable to load cfrezformat.dll");
                    return 1;
                }

                if (args.Count() < 2)
                {
                    DisplayHelp();
                    return 1;
                }

                char modeParam = char.ToUpper(args[0][0]);
                if ((args.Count() < 3) && (modeParam == 'X' || modeParam == 'C'))
                {
                    DisplayHelp();
                    return 1;
                }

                string rezFile = args[1].ToUpperInvariant();
                string extension = args.Count() < 4 ? "*.*" : args[3];

                RezCompiler rezCompiler = Marshal.GetDelegateForFunctionPointer<RezCompiler>(IntPtr.Add(formatBase, 0xA730));
                rezCompiler(args[0], rezFile, args[2], true, extension);

                return 0;
            }
            catch (Exception ex)
            {
                Console.Write("ERROR! ");
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}