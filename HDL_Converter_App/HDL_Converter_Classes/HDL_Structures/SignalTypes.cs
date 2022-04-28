using System;

namespace HDL_Converter_Classes.HDL_Structures
{
    /// <summary>
    /// Abstraction for a signal of any type and language
    /// </summary>
    public abstract class SignalType
    {
        public abstract void buildFromHDL();
        public abstract string buildHDLFormat();
    }

    //======================================= Verilog =======================================
    
    /// <summary>
    /// Abstraction for a Verilog signal of any type
    /// </summary>
    public abstract class VeriSignalType : SignalType
    {

        public static VeriSignalType createSignal(string busSize, bool signed)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// Contains the information for buses
    /// </summary>
    class VeriSignalTypeBus : VeriSignalType
    {
        public bool signed;
        public int lowerBit;
        public int upperBit;

        public override void buildFromHDL()
        {
            throw new NotImplementedException();
        }

        public override string buildHDLFormat()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Contains the information for simple wires
    /// </summary>
    class VeriSignalTypeBoolean : VeriSignalType
    {
        public override void buildFromHDL()
        {
            throw new NotImplementedException();
        }

        public override string buildHDLFormat()
        {
            throw new NotImplementedException();
        }
    }

    //======================================= VHDL =======================================

    /// <summary>
    /// Contains the information for a general vhdl signal.
    /// It is used for data that is not a std type like custom arrays
    /// </summary>
    public class VHDLSignalType : SignalType
    {
        public static VHDLSignalType createSignal(string busSize)
        {
            throw new NotImplementedException();
        }

        public override void buildFromHDL()
        {
            throw new NotImplementedException();
        }

        public override string buildHDLFormat()
        {
            throw new NotImplementedException();
        }

        public virtual VeriSignalType generateVerilogEquivalent()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Contains the information for a binary signal
    /// like std_logic or std_ulogic
    /// </summary>
    class VHDLSignalTypeBoolean : VHDLSignalType
    {
        public override void buildFromHDL()
        {
            throw new NotImplementedException();
        }

        public override string buildHDLFormat()
        {
            throw new NotImplementedException();
        }

        public override VeriSignalType generateVerilogEquivalent()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Contains the information for a bus signal with decimal size
    /// like integer or natural
    /// </summary>
    class VHDLSignalTypeDecimal : VHDLSignalType
    {
        public override void buildFromHDL()
        {
            throw new NotImplementedException();
        }

        public override string buildHDLFormat()
        {
            throw new NotImplementedException();
        }

        public override VeriSignalType generateVerilogEquivalent()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Contains the information for a binary signal
    /// like std_logic_vector or unsigned
    /// </summary>
    class VHDLSignalTypeBinary : VHDLSignalType
    {
        public override void buildFromHDL()
        {
            throw new NotImplementedException();
        }

        public override string buildHDLFormat()
        {
            throw new NotImplementedException();
        }

        public override VeriSignalType generateVerilogEquivalent()
        {
            throw new NotImplementedException();
        }
    }

}