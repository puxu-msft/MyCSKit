using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    class Program
    {
        [DllImport("dbghelp.dll", CharSet = CharSet.Unicode)]
        public static extern uint SymSetOptions(uint options);

        public const uint SYMOPT_DEBUG = 0x80000000;

        [DllImport("dbghelp.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymInitialize(
            IntPtr hProcess,
            [MarshalAs(UnmanagedType.LPTStr)] string? UserSearchPath,
            [MarshalAs(UnmanagedType.Bool)] bool fInvadeProcess);

        [DllImport("dbghelp.dll")]
        public static extern bool SymCleanup(IntPtr hProcess);

        internal const int SSRVOPT_DWORD = 0x0002;
        internal const int SSRVOPT_DWORDPTR = 0x004;
        internal const int SSRVOPT_GUIDPTR = 0x0008;

        [DllImport("dbghelp.dll", CharSet = CharSet.Unicode)]
        public static extern bool SymFindFileInPath(
            IntPtr hProcess,
            [MarshalAs(UnmanagedType.LPWStr)] string SearchPath,
            [MarshalAs(UnmanagedType.LPWStr)] string FileName,
            IntPtr id,
            Int32 two,
            Int32 three,
            Int32 flags,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder filePath,
            IntPtr callback,
            IntPtr context);

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool SymFindFileInPathProc(
            [MarshalAs(UnmanagedType.LPTStr)] string fileName,
            IntPtr context);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.Error.WriteLine("Usage: exe <SymbolPath> <PDBFileName> <GUID> <Age>");
                return;
            }
            // Console.WriteLine(args[0] + " " + args[1] + " " + args[2] + " " + args[3]);

            var symbolPath = args[0];
            var pdbFileName = args[1];
            var guid = new Guid(args[2]);
            var age = int.Parse(args[3]);
            var hCurrentProcess = System.Diagnostics.Process.GetCurrentProcess().Handle;
            // Console.WriteLine("SSSearch for {0} {1} {2}", pdbFileName, guid.ToString(), age);

            SymSetOptions(SYMOPT_DEBUG);
            if (!SymInitialize(hCurrentProcess, null, false))
            {
                Console.Error.WriteLine("SymInitialize failed with error {0}", GetLastError());
                return;
            }

            var hGuid = GCHandle.Alloc(guid, GCHandleType.Pinned);
            var fileLoc = new StringBuilder(256);
            try
            {
                if (!SymFindFileInPath(
                    hCurrentProcess,
                    symbolPath,
                    pdbFileName,
                    hGuid.AddrOfPinnedObject(),
                    age,
                    0,
                    SSRVOPT_GUIDPTR,
                    fileLoc,
                    IntPtr.Zero,
                    IntPtr.Zero
                    ))
                {
                    Console.Error.WriteLine("SymFindFileInPath failed with error {0}", GetLastError());
                    return;
                }
            }
            finally
            {
                hGuid.Free();
                SymCleanup(hCurrentProcess);
            }

            Console.WriteLine(fileLoc.ToString());
        }
    }
}
