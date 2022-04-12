using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDL_Converter_Classes.HDL_Structures;
using System.Collections.Generic;

namespace HDL_Converter_Test
{
    [TestClass]
    public class VHDLTest
    {
        [DataRow("clk : in std_logic","clk", PortDirection.Input, "std_logic")]
        [DataRow("readdata : out std_logic_vector(31 downto 0)", "readdata", PortDirection.Output, "std_logic_vector(31 downto 0)")]
        [DataRow("bidir_sig : InOut STD_ulogic_vector(some_param downto 0)", "bidir_sig", PortDirection.InOut, "STD_ulogic_vector(some_param downto 0)")]
        [DataRow("read_colum_cnt : in integer range 0 to colum_size", "read_colum_cnt", PortDirection.Input, "integer range 0 to colum_size")]
        [DataTestMethod]
        public void test_signal_init_from_line(string input, string name, PortDirection dir, string dataType)
        {
            VHDLWire signal = new VHDLWire();
            signal.initializeFromCodeLine(input);
            Assert.IsTrue(name == signal.name);
            Assert.IsTrue(dir == signal.direction);
            Assert.IsTrue(dataType == signal.busSize);
        }

        [DataRow("row_size: integer", "row_size", "integer", "")]
        [DataRow("some_gener: std_ulogic := \'1\'", "some_gener", "std_ulogic", "\'1\'")]
        [DataTestMethod]
        public void test_generic_init_from_line(string input, string name, string dataType, string value)
        {
            VHDLParameter generic = new VHDLParameter();
            generic.initializeFromCodeLine(input);
            Assert.IsTrue(name == generic.name);
            Assert.IsTrue(dataType == generic.dataType);
            Assert.IsTrue(value == generic.value);
        }
    }
}
