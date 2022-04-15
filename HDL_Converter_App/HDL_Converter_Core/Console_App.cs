using System;
using System.Collections.Generic;
using System.Text;
using HDL_Converter_Classes;
using HDL_Converter_Classes.HDL_Structures;

namespace HDL_Converter_Core
{
    class ConsoleApp
    {
        const string spacer = "==========================================";
        Settings settings;
        Converter converter;

        private string hdlInput = "";

        public ConsoleApp()
        {
            settings = new Settings();
            converter = new Converter(settings);
        }

        public void main()
        {
            while (true)
            {
                requestUserInput();
            }  
        }

        private void requestUserInput()
        {
            Console.WriteLine("Welcome to HDL Converter");
            printSection("HDL Input Console");            
            Console.WriteLine("- Paste a HDL Code bellow and enter \"-d\"");
            Console.WriteLine("- Enter \"-m\" to opem menu");
            Console.WriteLine("- Enter \"-c\" to clear Input");
            Console.WriteLine(System.Environment.NewLine);

            hdlInput = "";
            while (true)
            {
                string line = Console.ReadLine();
                if (line == "-m")
                {
                    openSettings();
                    break;
                }
                else if(line == "-d")
                {
                    menueHdlConvert(hdlInput);
                    break;
                }
                else if (line == "-c")
                {
                    Console.Clear();
                    break;
                }
                else
                {
                    hdlInput += (line + System.Environment.NewLine); 
                }
            }
            
        }

        private void menueHdlConvert(string hdlInput)
        {            
            converter.hdlInput = hdlInput;
            printHDLCreateMenu();
            try
            {
                while (true)
                {
                    Console.WriteLine("Enter d/i/t/e (declaration/instantiation/testbench/exit) to generate data or enter \"-m\" to open settings menu.");
                    Console.WriteLine("Use help x (x is the instruction. It can be d/i/t) to find out more about a command and available modifiers");
                    string userInput = Console.ReadLine();
                    Instruction instruction = new Instruction(userInput);
                    string result = instruction.execute(converter);
                    switch (result)
                    {
                        case "$ERROR$":
                            printString("Invalid Instruction", ConsoleColor.Red);
                            break;
                        case "$EXIT$":
                            Console.Clear();
                            return;
                        case "$MENU$":
                            openSettings();
                            printHDLCreateMenu();
                            break;
                        default:
                            printString(result, ConsoleColor.DarkCyan);
                            break;
                    }
                }
            }catch(FormatException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            catch (NotImplementedException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private void openSettings()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            printSettings();
            bool menuActive = true;
            while (menuActive)
            {
                string userInput = Console.ReadLine();
                string set = "";
                if (userInput == "") continue;
                if (userInput.Length > 1) set += userInput[userInput.Length - 1];
                switch (userInput[0])
                {
                    case '1':
                        Console.WriteLine("Type HDL Language to Use:");
                        Console.WriteLine("Available: Verilog, VHDL");
                        userInput = Console.ReadLine();
                        if(userInput.ToLower() == "vhdl")
                        {
                            settings.language = HDLLanguage.VHDL;
                        }
                        else if (userInput.ToLower() == "verilog"){
                            settings.language = HDLLanguage.Verilog;
                        }
                        printSettings();
                        break;
                    case '2':
                        binarySettingModify(ref settings.emptyIOs, set);
                        break;
                    case '3':
                        binarySettingModify(ref settings.includeInputComments, set);
                        break;
                    case '4':
                        binarySettingModify(ref settings.addWireDirectionComment, set);
                        break;
                    case '-':
                        if(userInput[1] == 'm')menuActive = false;
                        break;
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
        }

        private void binarySettingModify(ref bool setting, string input)
        {
            const string binarySettingPromt = "Enter t/f/e (true/false/exit) to configure or exit";
            Console.WriteLine(binarySettingPromt);
            bool active = true;
            while (active)
            {
                string userInput;
                if (input == "") userInput = Console.ReadLine();
                else userInput = input;

                if ("t" == userInput || "true" == userInput)
                {
                    setting = true;
                    active = false;
                }
                else if ("f" == userInput || "false" == userInput)
                {
                    setting = false;
                    active = false;
                }
                else if ("e" == userInput || "exit" == userInput)
                {
                    active = false;
                }
            }
            printSettings();
        }

        private void printString(string toPrint, ConsoleColor color)
        {
            Console.WriteLine();
            Console.ForegroundColor = color;
            Console.WriteLine(toPrint);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void printSection(string title)
        {
            Console.WriteLine(spacer);
            Console.WriteLine(title);
            Console.WriteLine(spacer);
        }

        private void printHDLCreateMenu()
        {
            Console.Clear();
            printSection("HDL Output Selection");
            Console.WriteLine("HDL Inputed:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(converter.hdlInput);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(System.Environment.NewLine);
            
        }

        private void printSettings()
        {
            Console.Clear();
            
            printSection("Settings");
            Console.WriteLine("The following settings are currently configured.");
            Console.WriteLine("To modify a setting enter the coresponding number in front of the setting bellow." +
                " To close menue enter \"-m\"");
            Console.WriteLine("Tip: For binary settings you can use short form eg. \"2f\" (set setting 2 false)");
            Console.WriteLine(System.Environment.NewLine);
            Console.WriteLine("Current Settings:");
            Console.WriteLine("1) HDL Language \t=>\t " + settings.language.ToString());
            Console.WriteLine("2) Empty IOs \t\t=>\t " + settings.emptyIOs.ToString());
            Console.WriteLine("3) User Comments \t=>\t " + settings.includeInputComments.ToString());
            Console.WriteLine("4) Direction Comments \t=>\t " + settings.addWireDirectionComment.ToString());
            Console.WriteLine(System.Environment.NewLine);
        }

    }
}
