using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HDL_Converter_Classes.HDL_Structures;

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
    }
}
