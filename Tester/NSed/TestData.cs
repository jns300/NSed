using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester.NSed
{
    /// <summary>
    /// Creates folders and files leveraged during tests.
    /// </summary>
    public class TestData
    {
        private static readonly String MAIN_DIR = "scanTest";

        private static readonly String A_DIR = Path.Combine(MAIN_DIR, "abc");

        private static readonly String DEPTH_DIR = Path.Combine(MAIN_DIR, "depth");

        private static readonly String DEPTH_A_DIR = Path.Combine(DEPTH_DIR, "a");

        private static readonly String DEPTH_B_DIR = Path.Combine(DEPTH_A_DIR, "b");

        private static readonly String DEPTH_C_DIR = Path.Combine(DEPTH_B_DIR, "c");

        private static readonly String DEPTH_D_DIR = Path.Combine(DEPTH_C_DIR, "d");

        public void CreateFiles()
        {
            Directory.CreateDirectory(MAIN_DIR);
            Directory.CreateDirectory(A_DIR);

            Directory.CreateDirectory(DEPTH_DIR);
            File.WriteAllText(Path.Combine(DEPTH_DIR, "0.txt"), String.Empty);

            Directory.CreateDirectory(DEPTH_A_DIR);
            File.WriteAllText(Path.Combine(DEPTH_A_DIR, "1.txt"), String.Empty);

            Directory.CreateDirectory(DEPTH_B_DIR);
            File.WriteAllText(Path.Combine(DEPTH_B_DIR, "2.txt"), String.Empty);

            Directory.CreateDirectory(DEPTH_C_DIR);
            File.WriteAllText(Path.Combine(DEPTH_C_DIR, "3.txt"), String.Empty);

            Directory.CreateDirectory(DEPTH_D_DIR);
            File.WriteAllText(Path.Combine(DEPTH_D_DIR, "4.txt"), String.Empty);

        }

        public void CleanUp()
        {
            Directory.Delete(MAIN_DIR, true);
        }
    }
}
