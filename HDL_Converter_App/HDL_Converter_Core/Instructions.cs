using System;
using System.Collections.Generic;
using System.Text;
using HDL_Converter_Classes;
using HDL_Converter_Classes.HDL_Structures;

namespace HDL_Converter_Core
{
    public class Modifier
    {
        public enum MODTYPE
        {
            emptySignals = 0,
            addDirectionComment = 1,
            addUserComment = 2,
            verify = 3,
            toplevel = 4
        }
        public MODTYPE type;
        public bool enabled;

        public Modifier()
        {
            
        }

        public Modifier(Modifier.MODTYPE type)
        {
            this.enabled = false;
            this.type = type;
        }

        public Modifier(Modifier.MODTYPE type, bool enable)
        {
            this.enabled = enable;
            this.type = type;
        }

        public void manipulateSettings(Settings settings, ref bool verify, ref bool top)
        {
            switch (type)
            {
                case MODTYPE.emptySignals:
                    settings.emptyIOs = !enabled; //Because of different formulation of modifier
                    break;
                case MODTYPE.addDirectionComment:
                    settings.addWireDirectionComment = enabled;
                    break;
                case MODTYPE.addUserComment:
                    settings.includeInputComments = enabled;
                    break;
                case MODTYPE.verify:
                    verify = true;
                    break;
                case MODTYPE.toplevel:
                    top = true;
                    break;
            }
        }

        public static List<Modifier> decodeModifiers(string comandLineInput)
        {
            List<Modifier> retList = new List<Modifier>();
            int modFound = 0;
            string dataString = comandLineInput;
            while(modFound != -1)
            {
                int index = dataString.IndexOf('-');
                if (index == -1) break;
                Modifier modifier = new Modifier();
                switch (dataString[index + 1])
                {
                    case 's':
                        modifier.type = MODTYPE.emptySignals;
                        break;
                    case 'd':
                        modifier.type = MODTYPE.addDirectionComment;
                        break;
                    case 'c':
                        modifier.type = MODTYPE.addUserComment;
                        break;
                    case 'v':
                        modifier.type = MODTYPE.verify;
                        break;
                    case 't':
                        modifier.type = MODTYPE.toplevel;
                        break;
                }
                if (modifier.type != MODTYPE.toplevel && modifier.type != MODTYPE.verify)
                {
                    switch (dataString[index + 2])
                    {
                        case '1':
                            modifier.enabled = true;
                            break;
                        case '0':
                            modifier.enabled = false;
                            break;
                        default:
                            modifier.enabled = false;
                            break;
                    }
                }
                retList.Add(modifier);
                dataString = dataString.Substring(index + 1);
            }
            return retList;
        }
    }
    public class Instruction
    {
        public enum ISTRUCTION_TYPE
        {
            Declaration = 0,
            Instantiation = 1,
            Testbench = 2,
            Exit = 3,
            Menue = 4,
            Help = 5
        }
        public ISTRUCTION_TYPE type;
        public List<Modifier> modifiers;
        private bool instructionValid;
        private Settings backupSettings;
        private string helptext;

        public Instruction(string userCommand)
        {
            string userInput = userCommand.ToLower();
            instructionValid = true;
            if (userInput.Contains("-m"))
            {
                this.type = ISTRUCTION_TYPE.Menue;
            }
            else if (userInput.Contains("help"))
            {
                this.type = ISTRUCTION_TYPE.Help;
                int indx = userInput.IndexOf("help")+4;
                string remaining = userInput.Substring(indx);
                if (remaining.Contains('i'))
                    this.helptext = HDL_Converter_Core.Properties.Resources.HELP_INSTANCIATION;
                else if (remaining.Contains('d'))
                    this.helptext = HDL_Converter_Core.Properties.Resources.HELP_DECLARATION;
                else if (remaining.Contains('t'))
                    this.helptext = HDL_Converter_Core.Properties.Resources.HELP_TESTBENCH;
                else
                    instructionValid = false;
            }
            else
            {
                string inst = userInput.Split("-")[0].Trim();               
                switch (inst)
                {
                    case "i":
                        type = ISTRUCTION_TYPE.Instantiation;
                        break;
                    case "d":
                        type = ISTRUCTION_TYPE.Declaration;
                        break;
                    case "t":
                        type = ISTRUCTION_TYPE.Testbench;
                        break;
                    case "e":
                        type = ISTRUCTION_TYPE.Exit;
                        break; 
                    default:
                        instructionValid = false;
                        break;
                }
                modifiers = Modifier.decodeModifiers(userInput);
            }
        }

        public string execute(Converter conv)
        {
            if (!instructionValid) return "$ERROR$";
            storeSettings(conv);
            bool en_verify = false, en_top = false;
            foreach (Modifier mod in this.modifiers)
            {
                mod.manipulateSettings(conv.settings, ref en_verify, ref en_top);
            }
            string retString = "";
            if (!en_verify && !en_top)
            {
                en_top = true;
                en_verify = true;
            }
            switch (this.type)
            {
                case ISTRUCTION_TYPE.Declaration:
                    retString = conv.generateWireDeclaration();
                    break;
                case ISTRUCTION_TYPE.Instantiation:
                    retString = conv.generateModuleInstantiation();
                    break;
                case ISTRUCTION_TYPE.Testbench:
                    if (en_top) retString += conv.generateTestbenchTopLevel();
                    retString += System.Environment.NewLine;
                    if (en_verify) retString += conv.generateTestbenchVerify();
                    break;
                case ISTRUCTION_TYPE.Exit:
                    retString = "$EXIT$";
                    break;
                case ISTRUCTION_TYPE.Menue:
                    retString ="$MENU$";
                    break;
                case ISTRUCTION_TYPE.Help:
                    retString = helptext;
                    break;
            }
            restoreSettings(conv);
            return retString;
        }

        private void storeSettings(Converter conv)
        {
            this.backupSettings = new Settings();
            backupSettings.emptyIOs = conv.settings.emptyIOs;
            backupSettings.addWireDirectionComment = conv.settings.addWireDirectionComment;
            backupSettings.includeInputComments = conv.settings.includeInputComments;
        }
        private void restoreSettings(Converter conv)
        {
            conv.settings.emptyIOs = backupSettings.emptyIOs;
            conv.settings.addWireDirectionComment = backupSettings.addWireDirectionComment;
            conv.settings.includeInputComments = backupSettings.includeInputComments;
        }
    }
}
