using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDL_Converter_Classes.HDL_Structures;
using System.Collections.Generic;

namespace HDL_Converter_Test
{
    [TestClass]
    public class VerilogTests
    {
        [DataRow("mySig",".mySig(mySig)", ".mySig()")]
        [DataTestMethod]
        public void test_wire_instantiation(string name, string resultWith, string resultWithout)
        {
            VeriWire testWire = new VeriWire();
            Settings settings = new Settings();
            testWire.settings = settings;
            settings.emptyIOs = false;
            testWire.name = name;
            Assert.IsTrue(resultWith.Equals(testWire.generateInstantiationLine()), "Error Declaration with wrong");
            
            settings.emptyIOs = true;
            Assert.IsTrue(resultWithout.Equals(testWire.generateInstantiationLine()),"Error Declaration without wrong");
        }


        [DataRow("mySig","","wire mySig;")]
        [DataRow("myBus", "[7:0]", "wire [7:0] myBus;")]
        [DataTestMethod]
        public void test_wire_declaration(string name, string busSize, string result)
        {
            VeriWire testWire = new VeriWire();
            Settings settings = new Settings();
            testWire.name = name;
            testWire.settings = settings;
            testWire.busSize = busSize;
            Assert.IsTrue(result.Equals(testWire.generateWireDeclarationLine()));
        }

        [DataRow("myParam", "7", "localparam myParam = 7;")]
        [DataTestMethod]
        public void test_parameter_declaration(string name, string value, string result)
        {
            VeriParameter param = new VeriParameter();
            Settings sett = new Settings();
            param.name = name;
            param.value = value;
            param.settings = sett;
            string generated = param.generateWireDeclarationLine();
            Assert.IsTrue(result.Equals(generated));
        }

        [DataRow("myParam", ".myParam(myParam)", ".myParam()")]
        [DataTestMethod]
        public void test_parameter_instantiation(string name, string resultWith, string resultWithout)
        {
            VeriParameter param = new VeriParameter();
            Settings sett = new Settings();
            param.name = name;
            param.settings = sett;
            sett.emptyIOs = false;
            string generated = param.generateInstantiationLine();
            Assert.IsTrue(resultWith.Equals(generated));
            sett.emptyIOs = true;
            generated = param.generateInstantiationLine();
            Assert.IsTrue(resultWithout.Equals(generated));
        }

        [TestMethod]
        public void test_separate_elements()
        {
            string path = "@../../../../../Test_data/Verilog_Wire_Separate_Elements.txt";
            string[] fileContent = GeneralTests.load_testdata_from_file(path);
            for(int i = 0; i < fileContent.Length; i += 2)
            {
                List<string[]> computed = VeriDataProcessing.separateElements(fileContent[i]);
                string[] expected = fileContent[i + 1].Split('|');
                for(int j = 0; j < expected.Length; j++)
                {
                    string res = computed[j / 2][j % 2];
                    Assert.IsTrue(res.Equals(expected[j]));
                }
            }
        }

        [DataRow("Input wire myWire", "myWire", "", PortDirection.Input)]
        [DataRow("Input wire [7:0] myBus", "myBus", "[7:0]", PortDirection.Input)]
        [DataRow("output wire myWire", "myWire", "", PortDirection.Output)]
        [DataRow("InOut Wire[8:0] myWire", "myWire", "[8:0]", PortDirection.InOut)]
        [DataRow("output reg states", "states", "", PortDirection.Output)]
        [DataRow("(*mark_debug = \"true\" *) input wire[1:0] states", "states","[1:0]",PortDirection.Input)]
        [DataTestMethod]
        public void test_veriwire_initialize_from_codeline(string codeLine, 
            string name, string busSize, PortDirection direction)
        {
            VeriWire wire = new VeriWire();
            wire.initializeFromCodeLine(codeLine);
            Assert.IsTrue(direction == wire.direction);
            Assert.IsTrue(name == wire.name);
            Assert.IsTrue(busSize == wire.busSize);
        }

        [DataRow("parameter myParam = 42", "myParam", "42")]
        [DataRow("parameter my_parameter = 'h0A", "my_parameter", "'h0A")]
        [DataTestMethod]
        public void test_veriParameter_initialize_from_codeline(string codeLine,
            string name, string value)
        {
            VeriParameter param = new VeriParameter();
            param.initializeFromCodeLine(codeLine);
            Assert.IsTrue(name == param.name);
            Assert.IsTrue(value == param.value);
        }

        [TestMethod]
        public void test_initializeFormHDLCode_standalone()
        {
            string path = "@../../../../../Test_data/Verilog_InitFromHDL_Standalone.txt";
            string[] fileContent = GeneralTests.load_testdata_from_file(path);
            for (int i = 0; i < fileContent.Length; i += 3)
            { 
                PATCH_STANDALONEINIT_VeriModule test = new PATCH_STANDALONEINIT_VeriModule(fileContent[i], fileContent[i + 1], fileContent[i + 2]);
            }
        }

        [TestMethod]
        public void test_initializeFormHDLCode_full()
        {
            string path = "@../../../../../Test_data/Verilog_InitFromHDL_Standalone.txt";
            string moduleString = GeneralTests.load_testdata_from_file(path)[0];
            VeriModule module = new VeriModule(moduleString);
            Assert.IsTrue(module.wires.Count == 4);
            Assert.IsTrue(module.parameters.Count == 2);
            string[] busSizes = { "", "", "[7:0]","" };
            PortDirection[] directions = { PortDirection.Input, PortDirection.Input, PortDirection.Output, PortDirection.InOut };
            string[] wireNames = { "clock", "reset", "busOut", "someSignal" };
            string[] wireComments = { "", "Comment", "", "Comment" };
            for(int i = 0; i < 4; i++)
            {
                Assert.IsTrue(busSizes[i] == module.wires[i].busSize);
                Assert.IsTrue(directions[i] == module.wires[i].direction);
                Assert.IsTrue(wireNames[i] == module.wires[i].name);
                Assert.IsTrue(wireComments[i] == module.wires[i].comment);
            }
            string[] paramNames = { "myParam1", "myParam2" };
            string[] paramValues = { "12", "2" };
            for(int i = 0; i < 2; i++)
            {
                Assert.IsTrue(paramNames[i] == module.parameters[i].name);
                Assert.IsTrue(paramValues[i] == module.parameters[i].value);
            }
            Assert.IsTrue(module.name == "test");
        }

    }

    public class PATCH_STANDALONEINIT_VeriModule : VeriModule
    {
        string resultParam;
        string resultWire;
        public PATCH_STANDALONEINIT_VeriModule(string hdlCode, string resultParam, string resultWire)
        {
            this.resultParam = resultParam;
            this.resultWire = resultWire;
            this.initializeFormHDLCode(hdlCode);
        }

        protected override void initializeParameters(string hdlCode)
        {
            Assert.IsTrue(hdlCode == resultParam);
        }

        protected override void initializeWires(string hdlCode)
        {
            Assert.IsTrue(hdlCode == resultWire);
        }
    }
}
