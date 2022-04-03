using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HDL_Converter_Classes.HDL_Structures;

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
        public void test_paranteses_detect(string openClose, string data, int res_open, int res_close)
        {
            ModuleTestPatch patch = new ModuleTestPatch();
            Tuple<int, int> index = patch.patchParanthesis(openClose[0], openClose[1], data);
            Assert.IsTrue(index.Item1 == res_open);
            Assert.IsTrue(index.Item2 == res_close);
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
    }
}
