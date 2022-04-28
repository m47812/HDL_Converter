using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDL_Converter_Classes.HDL_Structures;

namespace HDL_Converter_Test
{
    [TestClass]
    public class SignalTypeTest
    {
        [DataRow("", false, "")]
        [DataRow("", true, "")]
        [DataRow("[7:0]",false, "[7:0]")]
        [DataRow("[8:1]", true, "signed [8:1]")]
        [DataRow("[high:lower]", true, "signed [high:lower]")]
        [DataRow("[high:lower]", false, "[high:lower]")]
        [TestMethod]
        public void test_verilog_datatype_storage(string busSize, bool signed, string result)
        {
            VeriSignalType signal = VeriSignalType.createSignal(busSize, signed);
            Assert.IsTrue(signal.buildHDLFormat() == result);
        }
    }
}
