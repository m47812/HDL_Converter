using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HDL_Converter_Classes.HDL_Structures;
using System.IO;
using System;

namespace HDL_Converter_Test
{
    [TestClass]
    public class GeneralTests
    {
        [DataRow("()", "Ha(ll)o", 2,5)]
        [DataRow("{}", "Ha{(ll}o",2,6)] //Test that ( is Ignored
        [DataRow("()", "H(al()lo)",1,8)]
        [DataRow("()", "(Ha)l(lo)",0,3)]
        [DataTestMethod]
        public void test_parantheses_detect(string openClose, string data, int res_open, int res_close)
        {
            ModuleTestPatch patch = new ModuleTestPatch();
            Tuple<int, int> index = patch.patchParanthesis(openClose[0], openClose[1], data);
            Assert.IsTrue(index.Item1 == res_open);
            Assert.IsTrue(index.Item2 == res_close);
        }

        [DataRow("(Hall))", true)]
        [DataRow("(Hall)", false)]
        [DataRow("Ha(ll(o)Welt", true)]
        [DataRow("Hal(Hall)We", false)]
        [DataTestMethod]
        public void test_parentheses_error(string data, bool errorExpected)
        {
            ModuleTestPatch patch = new ModuleTestPatch();
            Tuple<int, int> index;
            if (errorExpected)
            {
                try
                {
                    index = patch.patchParanthesis('(', ')', data);
                    throw new AssertFailedException(); //Exception expected but not thrown
                }
                catch(FormatException)
                {
                    //Exception successfully thrown
                }
            }
            else
            {
                index = patch.patchParanthesis('(', ')', data);
            }            
        }

        /// <summary>
        /// Reads longer test data Strings from File "$" is used as separation character between test 
        /// data strings. The odd indexes are Results even are test data inputs. Results are
        /// separated by "|".
        /// </summary>
        /// <param name="path">Relative path to the file</param>
        /// <returns>string of all test data sets in file separated with "$"</returns>
        public static string[] load_testdata_from_file(string path)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string absPath = System.IO.Path.Combine(appPath, path);
            string fullPath = Path.GetFullPath(absPath);
            string Text = File.ReadAllText(fullPath);
            return Text.Split('$');
        }
    }

    class ModuleTestPatch : HDLModule
    {
        public Tuple<int, int> patchParanthesis(char open, char close, string data)
        {
            return this.getTopParenteses(open, close, data);
        }
        protected override void initializeFormHDLCode(string hdlCode)
        {
            throw new System.NotImplementedException();
        }

        protected override void initializeParameters(string hdlCode)
        {
            throw new System.NotImplementedException();
        }

        protected override void initializeWires(string hdlCode)
        {
            throw new System.NotImplementedException();
        }

        public override string generateModuleInstantiation()
        {
            throw new NotImplementedException();
        }

        public override string generateWireDeclaration()
        {
            throw new NotImplementedException();
        }
    }
}
