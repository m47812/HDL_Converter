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

        [TestMethod]
        public void test_separate_elements()
        {
            string path = "@../../../../../Test_data/VDHL_Example_Signals.txt";
            string[] fileContent = GeneralTests.load_testdata_from_file(path);
            for (int i = 0; i < fileContent.Length; i += 2)
            {
                List<string[]> computed = VHDLDataProcessing.separateElements(fileContent[i]);
                string[] expected = fileContent[i + 1].Split('|');
                for (int j = 0; j < expected.Length; j++)
                {
                    string res = computed[j / 2][j % 2];
                    Assert.IsTrue(res.Equals(expected[j]));
                }
            }
        }

        [TestMethod]
        public void test_init_from_hdl_code()
        {
            string path = "@../../../../../Test_data/VHDL_Example_Entity.txt";
            string[] fileContent = GeneralTests.load_testdata_from_file(path);
            Settings testSettings = new Settings();
            VHDLModule module = new VHDLModule(fileContent[0], testSettings);
            Assert.IsTrue(module.parameters.Count == 2);
            Assert.IsTrue(module.wires.Count == 6);
            Assert.IsTrue(module.parameters[0].name == "row_size");
            Assert.IsTrue(module.parameters[1].name == "colum_size");
            Assert.IsTrue(module.parameters[0].value == "5");
            Assert.IsTrue(module.parameters[0].comment == "Comment for constant");
            Assert.IsTrue(module.wires[0].busSize == "std_ulogic");
            Assert.IsTrue(module.wires[1].busSize == "std_ulogic_vector(7 downto 0)");
            Assert.IsTrue(module.wires[5].busSize == "integer range 0 to row_size");
            Assert.IsTrue(module.wires[0].direction == PortDirection.Input);
            Assert.IsTrue(module.wires[4].direction == PortDirection.InOut);
            Assert.IsTrue(module.wires[5].direction == PortDirection.Output);
            Assert.IsTrue(module.name == "test_module");
        }

        [TestMethod]
        public void test_wire_declaration()
        {
            string path = "@../../../../../Test_data/VHDL_Example_Entity.txt";
            string[] fileContent = GeneralTests.load_testdata_from_file(path);
            Settings testSettings = new Settings();
            VHDLModule module = new VHDLModule(fileContent[0], testSettings);
            Assert.IsTrue(module.generateWireDeclaration() == fileContent[1]);
        }

        [TestMethod]
        public void test_module_instantiation()
        {
            string path = "@../../../../../Test_data/VHDL_Example_Entity.txt";
            string[] fileContent = GeneralTests.load_testdata_from_file(path);
            Settings testSettings = new Settings();
            VHDLModule module = new VHDLModule(fileContent[0], testSettings);
            Assert.IsTrue(module.generateModuleInstantiation() == fileContent[2]);
        }

        [TestMethod]
        public void test_entity_generation()
        {
            string path = "@../../../../../Test_data/VHDL_Example_Entity.txt";
            string[] fileContent = GeneralTests.load_testdata_from_file(path);
            Settings testSettings = new Settings();
            VHDLModule module = new VHDLModule(fileContent[0], testSettings);
            string entity = module.generateModuleHeader();
            Assert.IsTrue(entity == fileContent[3]);
        }

        [TestMethod]
        public void test_component_generation()
        {
            string path = "@../../../../../Test_data/VHDL_Example_Entity.txt";
            string[] fileContent = GeneralTests.load_testdata_from_file(path);
            Settings testSettings = new Settings();
            VHDLModule module = new VHDLModule(fileContent[0], testSettings);
            string component = module.generateComponent();
            Assert.IsTrue(component == fileContent[4]);
        }

    }

    
}
