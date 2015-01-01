using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UPythonCompiler
{
    enum AssignType
    {
        Variable,
        Function
    }
    class SymbolEntry
    {
        #region Properties
        public string name; 
        
        public string type;
        
        public uint scope;

        public uint size;
        #endregion
        public SymbolEntry(string name, string type, uint scope, uint size)
        {
            this.name = name;
            this.type = type;
            this.scope = scope;
            this.size = size;
        }
    }
    class SemanticAnalyzer
    {
        public uint scopeCounter;
        public List<SymbolEntry> Table = new List<SymbolEntry>();
        public Stack<uint> ScopeStack = new Stack<uint>();
        
        public SemanticAnalyzer()
        {
            this.scopeCounter = 0;
            this.Table = new List<SymbolEntry>();
            this.ScopeStack = new Stack<uint>();
        }

        public uint CreateScope()
        {
            ScopeStack.Push(scopeCounter);
            return scopeCounter++;
        }
        public uint DeleteScope()
        {
            ScopeStack.Pop();
            return ScopeStack.Peek();
        }
        
        public string LookUpFunc(string Name, uint S)
        {
            foreach (var entry in Table)
            {
                if (entry.scope == S && entry.name == Name)
                {
                    return "FUNC_PARAM";
                }
            }
            return null;
        }

        //Lookup Looks up in the table. if the element exists
        public string LookUp(string Name,Stack<uint> scopeStack)
        {
            uint[] temperoryIterator;
            temperoryIterator = new uint[scopeStack.Count];

            scopeStack.CopyTo(temperoryIterator, 0);
            foreach (var i in scopeStack)
            {
                var isThereAnEntry = from entry in Table
                                     where entry.scope == i &&
                                     entry.name == Name
                                     select entry;
                if (isThereAnEntry.Count() > 0)
                {
                    string RT = isThereAnEntry.First().type;
                    return RT;
                    
                }
            }
            
            return null;
        }
        
        #region insert
        // To Insert A Variables
        public  void Insert(string Name, string Type, uint Scope, uint Size)
        {
            Table.Add(new SymbolEntry(Name, Type, Scope, Size));
        }
        // This function is specifically used for insertion Function
        public  void InsertFunction(string Name, string Type, uint Scope, uint NoOfParameters)
        {
            Table.Add(new SymbolEntry(Name, NoOfParameters + "->" + Type, Scope, 1));
        }
        #endregion

        // Check Compatibility for Binary operations
        public string Compatible(string T1,string T2, string Op)
        {
            switch(Op)
            {
                case "+":
                    {
                        // if both types are string
                        if (T1 == "STRING_CONST" && T2 == "STRING_CONST")
                        {
                            return "STRING_CONST";
                        }
                        // if T1 is INT_CONST
                        else if (T1 == "INT_CONST")
                        {
                            if (T2 == "FLOAT_CONST")
                            {
                                return "FLOAT_CONST";
                            }
                            else if (T2 == "INT_CONST")
                            {
                                return "INT_CONST";
                            }
                            else
                            {
                                return null;
                            }
                        }
                        // if T1 is FLAOT_CONST
                        else if (T1 == "FLOAT_CONST")
                        {
                            if (T2 == "INT_CONST" || T2 == "FLOAT_CONST")
                            {
                                return "FLOAT_CONST";
                            }
                        }

                        break;
                    }
                case "-":
                    {
                        if (T1 == "INT_CONST")
                        {
                            if (T2 == "FLOAT_CONST")
                            {
                                return "FLOAT_CONST";
                            }
                            else if (T2 == "INT_CONST")
                            {
                                return "INT_CONST";
                            }
                            else
                            {
                                return null;
                            }
                        }
                        // if T1 is FLAOT_CONST
                        else if (T1 == "FLOAT_CONST")
                        {
                            if (T2 == "INT_CONST" || T2 == "FLOAT_CONST")
                            {
                                return "FLOAT_CONST";
                            }
                        }
                        break;
                    }
                case "*":
                case "/":
                    {
                        if (T1 == "FLOAT_CONST" || T2 == "FLOAT_CONST")
                        {
                            if (T1 == "INT_CONST" || T2 == "INT_CONST" || T1 == "FLOAT_CONST" || T2 == "FLOAT_CONST")
                            {
                                return "FLOAT_CONST";
                            }

                        } 
                        else if (T1 == "INT_CONST" && T2 == "INT_CONST")
                        {
                            return "INT_CONST";
                        }
                        break;
                    }
                    
                case "**":
                    {
                        //if T1 is INT_CONST
                        if (T1 == "INT_CONST")
                        {
                            if (T2 == "FLOAT_CONST")
                            {
                                return "FLOAT_CONST";
                            }
                            else if (T2 == "INT_CONST")
                            {
                                return "INT_CONST";
                            }
                            else
                            {
                                return null;
                            }
                        }
                        // if T1 is FLAOT_CONST
                        else if (T1 == "FLOAT_CONST")
                        {
                            if (T2 == "INT_CONST" || T2 == "FLOAT_CONST")
                            {
                                return "FLOAT_CONST";
                            }
                        }
                        break;
                    }
                    
                case "//":
                    {
                        if (T1 == "INT_CONST" || T1 == "FLOAT_CONST")
                        {
                            if (T2 == "FLOAT_CONST" || T2 == "INT_CONST")
                            {
                                return "FLOAT_CONST";
                            }
                        }
                        break;
                    }

                case "EqualityOperator":
                case "ComparisionOperator":
                    {
                        if (T1 == "INT_CONST" || T1 == "FLOAT_CONST")
                        {
                            if (T2 == "FLOAT_CONST" || T2 == "INT_CONST")
                            {
                                return "INT_CONST";
                            }
                        }
                        break;
                    }
                case "or":
                case "and":
                    {
                        if (T1 == "INT_CONST" || T1 == "FLOAT_CONST" || T1 == "STRING_CONST" || T1 == "LIST" || T1 == "DICTIONARY")
                        {
                            if (T2 == "INT_CONST" || T2 == "FLOAT_CONST" || T2 == "STRING_CONST" || T2 == "LIST" || T2 == "DICTIONARY")
                            {
                                return "INT_CONST";
                            }
                        }
                        break;
                    }
            }
            return null;
        }

        public string Compatible (string T1, string Op )
        {
            if (Op == "~")
            {
                if (T1 == "INT_CONST")
                {
                    return "INT_CONST";
                }
            }
            else if (Op== "+" || Op == "-" )
            {
                if (T1 == "INT_CONST" || T1 == "FLOAT_CONST")
                {
                    return T1;
                }
            }
            return null;
        }

        public bool Assign(string Name, string Type, uint Scope, uint Param_Size, Stack<uint> ScopeStack,AssignType AT)
        {
            uint[] temperoryIterator;
            temperoryIterator = new uint[ScopeStack.Count];

            ScopeStack.CopyTo(temperoryIterator, 0);
            foreach (var s in ScopeStack)
            {
                for (int i=0 ; i < Table.Count ; i++)
                {
                    if (Table[i].scope == s)
                    {
                        if (Table[i].name == Name)
                        {
                            Table.Remove(Table[i]);
                            if (AT == AssignType.Function)
                            {
                                InsertFunction(Name, Type, Scope, Param_Size);
                            }
                            else if (AT == AssignType.Variable)
                            {
                                Insert(Name, Type, Scope, Param_Size);
                            }
                            return true;
                        }
                        else if ( s == ScopeStack.First())
                        {
                            if (AT == AssignType.Function)
                            {
                                InsertFunction(Name, Type, Scope, Param_Size);
                            }
                            else if (AT == AssignType.Variable)
                            {
                                Insert(Name, Type, Scope, Param_Size);
                            }
                        }
                        
                    }
                    
                }
            }

            return false;
        }
    }
    class SyntaxAnalyzer
    {
        private static SemanticAnalyzer SA = new SemanticAnalyzer();
        private static List<RawWords> Tokens;
        private static int index;
        public SyntaxAnalyzer(List<RawWords> ListofTokens)
        {
            Tokens = ListofTokens;
            index = 0;
        }
        public static bool Compile(List<RawWords> ListofTokens)
        {
            Tokens = ListofTokens;
            index = 0;
            return START();
        }


        // <START> --> <BODY>
        private static bool START()
        {
            bool result;
            try
            {
                SA.scopeCounter = 0;
                // Creating Scope
                SA.Table.Clear();
                SA.ScopeStack.Clear();
                uint S = SA.CreateScope();
                result = BODY(S);
                //Deleting Scope
                //SA.DeleteScope();
            }
            catch (Exception e)
            {
                result = false;
            }
            if (result) 
                MessageBox.Show("No Error");
            else
                MessageBox.Show("Error");
            return result;
        }


        #region Non Function Area
        // <BODY> -> __Body_Start__<MULTI_ST>__Body_End__
        private static bool BODY(uint S)
        {
            if (Tokens[index].token == "BODY_START")
            {
                index++;
                
                // <MULTI_ST>
                if (MULTI_ST(S))
                {
                    if (Tokens[index].token == "BODY_END")
                    {
                        index++;
                        return true;
                    }
                }
                
            }
            return false;
        }

        // <MULTI_ST> -> <SINGLE_ST><MULTI_ST> |  Ɛ
        private static bool MULTI_ST(uint S)
        {

            if (Tokens[index].token == "BODY_END")
            {
                return true;
            }
            //<NEXT>
            if (SINGLE_ST(S))
            {
                // <SINGLE_ST>
                if(MULTI_ST(S))
                {
                    return true;
                }
            }
            
            return false;
        }


        // <SINGLE_ST>  ID <SINGLE_ST1> | <WHILE_ST> | <FOR_ST> | <IF_ELSE> | <VALUE> | <REPEAT_ST> | <FUNCTION_DEF> | pass 
        private static bool SINGLE_ST(uint S)
        {
            string T = "";
            uint length = 0;
            //ID <SINGLE_ST1> 
            if (Tokens[index].token == "ID")
            {
                uint paramCount_Size = 0;
                var N = Tokens[index].GetWord(); 
                index++;
                if (SINGLE_ST1(S,ref T,ref paramCount_Size))
                {
                    string T1 = "";
                    T1 = SA.LookUp(N, SA.ScopeStack);
                    if (T1 != null)
                    {
                        if (T1.Contains("->"))
                        {
                            if (paramCount_Size.ToString() != T1.Substring(0,T1.IndexOf("->")) )
                            {
                                MessageBox.Show(String.Format("Semantic: No Function with {0} Parameter(s)",paramCount_Size));
                            }
                        } 
                        else
                        {
                            SA.Assign(N, T, S, paramCount_Size, SA.ScopeStack, AssignType.Variable);
                        }
                    }
                    else
                    {
                        if (T != "" || T != null)
                        {
                            SA.Insert(N, T, S, 1);
                        }
                        else
                        {
                            MessageBox.Show(String.Format("Semantic: No such Function {0} ",N));
                        }
                    }
                    return true;
                }
            }

            // <WHILE_ST>
            else if (WHILE_ST(S))
            {
                return true;
            }
            // <FOR_ST>
            else if (FOR_ST(S))
            {
                return true;
            }

            // <IF_ELSE> 
            else if (IF_ELSE(S))
            {
                return true;
            }

            // <VALUE> 
            else if (VALUE(S,ref T, ref length))
            {
                return true;
            }
            // <REPEAT_ST>  
            else if (REPEAT_ST(S))
            {
                return true;
            }
            // <FUNCTION_DEF>
            else if (FUNCTION_DEF(S))
            {
                return true;
            }
  
            // pass 
            else if (Tokens[index].token == "pass")
            {
                index++;
                return true;
            }
            return false;
        }
        //<WHILE_ST> while <EXPRESSION> : <BODY><ELSE>
        private static bool WHILE_ST(uint S)
        {
            if (Tokens[index].token == "while")
            {
                string T = "";
                index++;
                if (EXPRESSION(S,ref T))
                {
                    if (Tokens[index].token == ":")
                    {
                        index++;
                        S = SA.CreateScope();
                        if (BODY(S))
                        {
                            S = SA.DeleteScope();
                            if (ELSE(S))
                            {
                                
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        //<FOR_ST>  for ID in <SEQUENCE>: <BODY><ELSE>
        private static bool FOR_ST(uint S) 
        {
            string N = "",T="ITERATOR";
            if (Tokens[index].token == "for")
            {
                index++;
                if (Tokens[index].token == "ID")
                {
                    N = Tokens[index].word;
                    if ((T=SA.LookUp(N,SA.ScopeStack))==null)
                    {
                        SA.Insert(N, T, S, 1);
                    }
                    else
                    {
                        // Not Sure what to do here
                        SA.Assign(N, T, S, 1, SA.ScopeStack, AssignType.Variable);
                    }
                    index++;
                    if (Tokens[index].token == "in")
                    {
                        index++;
                        uint size_param=0;
                        string T1 = "";
                        if (SEQUENCE(S,ref T1,ref size_param))
                        {
                            if (Tokens[index].token == ":")
                            {
                                index++;
                                if (BODY(S))
                                {
                                    if (ELSE(S))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        //<SEQUENCE>  <LIST><SEQUENCE1>|<DICTIONARY>|ID<SEQUENCE2>
        private static bool SEQUENCE(uint S, ref string T, ref uint length) 
        {
            //<LIST><SEQUENCE1>
            if (LIST(S,ref length))
            {

                if (SEQUENCE1(S, ref length))
                {
                    T = "LIST";
                    return true;
                }
            }
            // <DICTIONARY>
            else if (DICTIONARY(S,ref length))
            {
                T = "DICTIONARY";
                return true;
            }
            // ID<SEQUENCE2>
            else if (Tokens[index].token == "ID")
            {
                string N = Tokens[index].word;
                T= SA.LookUp(N, SA.ScopeStack);
                if (T == null)
                {
                    MessageBox.Show(String.Format("No such variable as {0}", N), "Semantic Error");
                }
                uint parameters = 0;
                if (SEQUENCE2(S, ref T, ref parameters)) // sequence is a list, this will contain size
                {
                    if (T.Contains("->"))
                    {
                        if (parameters.ToString() == T.Substring(0,T.IndexOf("->")))
                        {
                            // not sure what to do here   
                        }
                        else
                        {
                            MessageBox.Show(String.Format("No Function {0} with {1} parameters", N, parameters), "Semantic Error");
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        //<SEQUENCE1> PLUS<ADD_COLLECTION> | Ɛ
        private static bool SEQUENCE1(uint S, ref uint length)
        {
            if (Tokens[index].token == "+")
            {
                index++;
                if (ADD_COLLECTION(S, ref length))
                {
                    return true;
                }
            }
            // handling Null
            else if (Tokens[index].token == ":")
            {
                return true;
            }
            return false;
        }
        //<SEQUENCE2>  (<FC_PARAMETERS>) | <ADD_COLLECTION2>
        private static bool SEQUENCE2(uint S, ref string T, ref uint paramCount)
        {
            if (Tokens[index].token == "(")
            {
                index++;
                if (FC_PARAMETERS(S, ref paramCount))
                {
                    if (Tokens[index].token == ")")
                    {
                        index++;
                        return true;
                    }
                }
            } 
            else if (ADD_COLLECTION2(S,ref paramCount))
            {
                return true;
            }

            return false;
            
        }


        //<SINGLE_ST1> -> = <DECLARE2> | ASIGN_OP <EXPRESSION> | ,<NEXT_DECLARE>,<DEC_VALUES>| 
        //      (<FC_PARAMETERS>)| Ɛ
        private static bool SINGLE_ST1(uint S, ref string T,ref uint paramCount)
        {
            //= <DECLARE2> 
            if (Tokens[index].token == "=")
            {
                index++;
                if (DECLARE2(S, ref T,ref paramCount))
                {
                    return true;
                }
            }
            //ASIGN_OP <EXPRESSION>
            else if (Tokens[index].token == "AssignmentOperator")
            {
                index++;
                if (EXPRESSION(S, ref T))
                {
                    return true;
                }
            }
            // ,<NEXT_DECLARE>,<DEC_VALUES>
            else if (Tokens[index].token == ",")
            {
                index++;
                if (NEXT_DECLARE(S) )
                {
                    uint length = 0;
                    if (Tokens[index].token == ",")
                    {
                        index++;
                        if (DEC_VALUES(S,ref length))
                        {
                            return true;
                        }
                    }
                }
            } 
            //(<FC_PARAMETERS>)
            else if (Tokens[index].token == "(")
            {
                index++;
                if (FC_PARAMETERS(S, ref paramCount))
                {
                    if (Tokens[index].token == ")")
                    {
                        index++;
                        return true;
                    }
                }
            }
           //Follow (<SINGLE_ST1>) = {while, for, if, 
            //repeat, def, pass, {, [, 
            //STRING_CONST, INT_CONST, 
            //FLOAT_CONST, ID, Body_End }
            else if ( Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END") // handling null
            {
                return true;
            }
            return false;
        }

        //< FC_PARAMETERS> <EXPRESSION> <FC_NEXT_PARAM>| <LIST> | <DICTIONARY>  |  Ɛ
        private static bool FC_PARAMETERS(uint S, ref uint count)
        {
            string T = "";
            uint length = 0;
            if (EXPRESSION(S, ref T))
            {
                count++;
                if (FC_NEXT_PARAM(S,ref count))
                {
                    return true;
                }
            }
            else if (LIST(S,ref length))
            {
                count++;
                return true;
            }
            else if (DICTIONARY(S, ref length))
            {
                count++;
                return true;
            }
            //Follow(<FC_PARAMETER_ID>) { , , ) }
            // handling null
            else if (Tokens[index].token == ")" || Tokens[index].token == ",")
            {
                
                return true;
            }
            
            return false;
        }

        
        //< FC_NEXT_PARAM >  ,< FC_PARAMETERS > | Ɛ
        private static bool FC_NEXT_PARAM(uint S, ref uint count)
        {
            if (Tokens[index].token == ",")
            {
                index++;
                if (FC_PARAMETERS(S,ref count))
                {
                    return true;
                }
                
            }
            else if (Tokens[index].token == ")") // handling null
            {
                return true;
            }
            return false;
        }
        //<NEXT_DECLARE>  ID <NEXT_DECLARE1>
        private static bool NEXT_DECLARE(uint S)
        {
            if (Tokens[index].token == "ID")
            {
                index++;
                if (NEXT_DECLARE1(S))
                {
                    return true;
                }
            }
            return false;
        }

        //<NEXT_DECLARE1>  ,<NEXT_DECLARE> , < DEC _VALUES > | = <DEC_VALUES>
        private static bool NEXT_DECLARE1(uint S)
        {
            uint length = 0;

            //,<NEXT_DECLARE> , < DEC _VALUES >
            if (Tokens[index].token == ",")
            {
                index++;
                if (NEXT_DECLARE(S))
                {
                    if (Tokens[index].token == ",")
                    {
                        index++;
                        if (DEC_VALUES(S,ref length))
                        {
                            return true;
                        }
                    }
                }
            } //= <DEC_VALUES>
            else if ( Tokens[index].token == "=")
            {
                index++;
                if (DEC_VALUES(S,ref length))
                {
                    return true;
                }
            }

            return false;
        }

        // I have not completed it, i have to give type here as well in order to assign, but for now i am leaving this functionality, 
        //and it may not play a major role in code
        //<DEC _VALUES>  <EXPRESSION>|<LIST> | <DICTIONARY> 
        private static bool DEC_VALUES(uint S, ref uint length)
        {
            var T = "";
            if (EXPRESSION(S,ref T))
            {
                return true;
            }
            else if (LIST(S,ref length))
            {
                T = "LIST";
                return true;
            }
            else if (DICTIONARY(S, ref length))
            {
                T = "DICTIONARY";
                return true;
            }
            return false;
        }


        //<DECLARE2> --> <EXPRESSION> | <DICTIONARY> | <LIST> <LIST_DECLARE>
        private static bool DECLARE2(uint S, ref string T,ref uint size)
        {
            if (EXPRESSION(S, ref T))
            {
                size = 1;
                return true;
            }
            else if (DICTIONARY(S,ref size))
            {
                T = "DICTIONARY";
                return true;
            }
            else if (LIST(S, ref size))
            {
                if (LIST_DECLARE(S,ref size))
                {
                    T = "LIST";
                    return true;
                }
            }
            return false;
        }


        #region Dictionary
        //<DICTIONARY> -> {<DICTIONARY1>}
        private static bool DICTIONARY(uint S, ref uint size)
        {
            if (Tokens[index].token == "{")
            {
                index++;
                if (DICTIONARY1(S, ref size))
                {
                    size++;
                    if (Tokens[index].token == "}")
                    {
                        index++;
                        return true;
                    }
                }
            }
            return false;
        }

        //<DICTIONARY1>-> <KEY> : < ALL_VALUE> <DICTIONARY2> | Ɛ
        private static bool DICTIONARY1(uint S, ref uint size)
        {
            string T = "";
            if (KEY(S))
            {
                size++;
                if (Tokens[index].token == ":")
                {
                    index++;
                    if (ALL_VALUE(S,ref T,ref size))
                    {
                        if (DICTIONARY2(S,ref size))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (Tokens[index].token == "}") // handling NULL
            {
                return true;
            }
            return false;
        }


        //<DICTIONARY2> -> , <DICTIONARY1> | Ɛ
        private static bool DICTIONARY2(uint S, ref uint size)
        {
            if (Tokens[index].token == ",")
            {
                index++;
                size++;
                if(DICTIONARY1(S, ref size))
                {
                    return true;
                }
            }
            // Follow (<DICTIONARY2>)  { } }
            else if (Tokens[index].token == "}") // handling null with FOllow
            {
                return true;
            }

            return false;
        }

        //<ALL_VALUES> --> ID | <VALUE>
        private static bool ALL_VALUE(uint S, ref string T,ref uint length)
        {
            if (Tokens[index].token == "ID")
            {
                length++;
                string Name = Tokens[index].word;
                T = SA.LookUp(Name, SA.ScopeStack);
                if (T == null)
                {
                    MessageBox.Show("Semantic Error: {0} can't be found in current tree of scope", Name);
                }
                index++;
                return true;
            }
            else if (VALUE(S, ref T, ref length))
            {
                return true;
            }

            return false;
        }
        //<KEY> -> <CONST> | ID
        private static bool KEY(uint S)
        {
            string T = "";
            if(CONST(ref T))
            {
                return true;
            }
            else if (Tokens[index].token == "ID")
            {
                string Name = Tokens[index].word;
                T = SA.LookUp(Name, SA.ScopeStack);
                if (T == null)
                {
                    MessageBox.Show("Semantic Error: {0} can't be found in current tree of scope", Name);
                }
                index++;
                return true;
            }

            return false;
        }
        //<CONST> -> FLOAT_CONST | INT_CONST | STRING_CONST
        private static bool CONST(ref string T)
        {
            if (Tokens[index].token == "STRING_CONST" || Tokens[index].token == "FLOAT_CONST" || Tokens[index].token == "INT_CONST")
            {
                T = Tokens[index].token;
                index++;
                
                return true;
            }
            return false;
        }
#endregion
        //<LIST_DECLARE> -> PLUS <ADD_COLLECTION> | Ɛ
        private static bool LIST_DECLARE(uint S,ref uint length )
        {
            if (Tokens[index].token == "+")
            {
                index++;
                if (ADD_COLLECTION(S,ref length))
                {
                    return true; 
                }
            }
                // handling Null
            else if (Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                return true;
            }

            return false;
        }

        //<ADD_COLLECTION> -> ID <ADD_COLLECTION2> | <LIST> <ADD_COLLECTION2> 
        private static bool ADD_COLLECTION(uint S, ref uint length)
        {
            string T1 = "";
            if (Tokens[index].token == "ID")
            {
                var N = Tokens[index].word;
                if ( (T1=SA.LookUp(N,SA.ScopeStack)) == null)
                {
                    MessageBox.Show(String.Format("{0} is Undeclared",N),"Semantic Error");
                }
                else if (T1 != "LIST")
                {
                    MessageBox.Show(String.Format("{0} is not a List", N), "Semantic Error");

                }
                index++;
                if (ADD_COLLECTION2(S, ref length))
                {
                    return true;
                }
                return true;
            }

            else if (LIST(S, ref length))
            {
                if (ADD_COLLECTION2(S, ref length))
                {

                    return true;
                }
            }
            return false;
        }

        //<ADD_COLLECTION2> -> PLUS <ADD_COLLECTION> | Ɛ
        private static bool ADD_COLLECTION2(uint S,  ref uint length )
        {
            if (Tokens[index].token == "+")
            {
                index++;
                if (ADD_COLLECTION(S,ref length))
                {
                    return true;
                }
            }
            return false;
        }
        //<LIST> --> [<LIST1>]
        private static bool LIST(uint S, ref uint length)
        {
            if (Tokens[index].token == "[")
            {
                index++;
                if (LIST1(S,ref length))
                {
                    if (Tokens[index].token == "]")
                    {
                        index++;
                        return true;
                    }
                }
            }
            return false;
        }

        //< LIST1>  <ALL_VALUE><MORE_VALUES> | Ɛ
        private static bool LIST1(uint S, ref uint length)
        {
            string T = "";
            if(ALL_VALUE(S,ref T, ref length))
            {
                if( MORE_VALUES(S,ref length))
                {
                    return true;
                }
            }
                // handling Null
            else if(Tokens[index].token == "]")
            {
                return true;
            }
            return false;
        }

        //<MORE_VALUES> -> ,< ALL_VALUE><MORE_VALUES> | Ɛ 
        private static bool MORE_VALUES(uint S, ref uint length)
        {
            string T = "";
            if (Tokens[index].token == ",")
            {
                
                index++;
                if (ALL_VALUE(S,ref T, ref length))
                {
                    if(MORE_VALUES(S, ref length))
                    {
                        return true;
                    }
                }
            } 
                //Follow(<MORE_VALUES>) -> { ] }
            else if (Tokens[index].token == "]")// handling null
            {
                return true;
            }
            return false;
        }

        //<EXPRESSION>  <EXP_OR>
        private static bool EXPRESSION(uint S, ref string T)
        {
            if (EXP_OR(S,ref T))
            {
                return true;
            }
            return false;
        }
        //<EXP_OR>  <EXP_AND> <EXP_OR1>
        private static bool EXP_OR(uint S, ref String T)
        {
            string LT = "";
            if (EXP_AND(S, ref LT) )
            {
                if (EXP_OR1(S, LT, ref T))
                {
                    return true;
                }
            }
            return false;
        }
        
        //<EXP_OR1>  OR <EXP_AND><EXP_OR1> | Ɛ
        private static bool EXP_OR1(uint S, string LT, ref string T)
        {
            string op, RT = "", T1;
            if (Tokens[index].token == "or")
            {
                op = "or";
                index++;
                if (EXP_AND(S, ref RT) )
                {
                    T1 = SA.Compatible(LT, RT, op);
                    if (T1 == null)
                    {
                        MessageBox.Show(String.Format("Semantic Error: Cannot \"{2}\" {0}, {1}",LT,RT,op));
                    }
                    if (EXP_OR1(S,T1, ref T))
                    {
                        return true;
                    }
                }
                    
            }// handling null
            else if (Tokens[index].token == "or" ||
                Tokens[index].token == ":" ||
                Tokens[index].token == ")" ||
                Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                T = LT;
                return true;
            }
            return false;
        }


        //<EXP_AND>  <EXP_EQUALITY> <EXP_AND1>
        private static bool EXP_AND(uint S, ref String T)
        {
            string LT = "";
            if (EXP_EQUALITY(S,ref LT) )
            {
                if (EXP_AND1(S,LT,ref T))
                {
                    return true;
                }
            }
            return false;
        }

        //<EXP_AND1>  AND <EXP_EQUALITY><EXP_AND1> | Ɛ
        private static bool EXP_AND1(uint S, string LT, ref string T)
        {
            string op,RT="",T1;
            if (Tokens[index].token == "and")
            {
                op = "and";
                index++;
                if ( EXP_EQUALITY(S, ref RT) )
                {
                    T1 = SA.Compatible(LT, RT, op);
                    if (T1 == null)
                    {
                        MessageBox.Show(String.Format("Semantic Error: Cannot \"{2}\" {0}, {1}", LT, RT, op));
                    }
                    if (EXP_AND1(S, T1, ref T))
                    {
                        return true;
                    }
                }
            } // handling null
            //Follow (<EXP_AND1>){AND, : , ) , while, for, if, repeat, def, pass, {, 
            //[, STRING_CONST, INT_CONST, FLOAT_CONST, ID, Body_End, , }
            else if (Tokens[index].token == "or" ||
                Tokens[index].token == ":" ||
                Tokens[index].token == ")" ||
                Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                T = LT;
                return true;
            }
            
            return false;
        }

        //<EXP_EQUALITY>  <EXP_COMPARISION><EXP_EQUALITY1>
        private static bool EXP_EQUALITY(uint S, ref string T)
        {
            string  LT="";
            if (EXP_COMPARISION(S, ref LT) )
            {
                if (EXP_EQUALITY1(S,LT,ref T))
                {
                    return true;
                }
            }
            return false;
        }
        
        //<EXP_EQUALITY1>  EQUALITY_OP <EXP_COMPARISION> 
                        //<EXP_EQUALITY1> | Ɛ
        private static bool EXP_EQUALITY1(uint S, string LT, ref string T)
        {
            string op, RT = "", T1;
            if (Tokens[index].token == "EqualityOperator")
            {
                op = "EqualityOperator";
                index++;
                
                if (EXP_COMPARISION(S,ref RT))
                {
                    T1 = SA.Compatible(LT,RT,op);
                    if (T1 == null)
                    {
                        MessageBox.Show(String.Format("Semantic Error: Cannot \"{2}\" {0}, {1}",LT,RT,op));
                    }
                    if (EXP_EQUALITY1(S, T1, ref T))
                    {
                        return true;
                    }
                }
                    
            }
            //handling null
            //Follow (<EXP_EQUALITY1>) {EQUALITY_OP, AND, 
            //OR, : , ) , while, for, if, repeat, def, pass, {, 
            //[, STRING_CONST, INT_CONST, FLOAT_CONST, ID, Body_End, , }
            else if ( Tokens[index].token == "EqualityOperator" || 
                Tokens[index].token == "and" ||
                Tokens[index].token == "or" ||
                Tokens[index].token == ":" ||
                Tokens[index].token == ")" ||
                Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                T = LT;
                return true;
            }

            return false;
        }

        //<EXP_COMPARISION>  <EXP_P_M><EXP_COMPARISION1>
        private static bool EXP_COMPARISION(uint S, ref string T)
        {
            string LT = "";
            if (EXP_P_M(S,ref LT))
            {
                if (EXP_COMPARISION1(S, LT, ref T))
                {
                    return true;
                }
            }

            return false;
        }

        //<EXP_COMPARISION1>  COMPARISION_OP <EXP_P_M> <EXP_COMPARISION1> | Ɛ
        private static bool EXP_COMPARISION1(uint S, string LT, ref string T)
        {
            string op, RT = "", T1;
            if (Tokens[index].token == "ComparisionOperator" )
            {
                op = "ComparisionOperator";
                index++;
                if (EXP_P_M(S, ref RT))
                {
                    if ((T1=(SA.Compatible(LT,RT,op)))==null )
                    {
                        MessageBox.Show(String.Format("Semantic Error: Cannot \"{2}\" {0}, {1}", LT, RT, op));
                    }
                    if (EXP_COMPARISION1(S,T1,ref T))
                    {
                        return true;
                    }
                }
            } // handling null
            //Follow(<EXP_COMPARISION1>) {COMPARISION_OP, EQUALITY_OP, 
            //AND, OR, : , ) , while, for, if, repeat, 
            //def, pass, {, [, STRING_CONST, INT_CONST, 
            // FLOAT_CONST, ID, Body_End, , }
            else if ( Tokens[index].token == "ComparisionOperator" ||
                Tokens[index].token == "EqualityOperator" ||
                Tokens[index].token == "and" ||
                Tokens[index].token == "or" ||
                Tokens[index].token == ":" ||
                Tokens[index].token == ")" ||
                Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                T = LT;
                return true;
            }

            return false;
        }

        //<EXP_P_M>  < EXP_M_D_M_F> <EXP_P_M1>
        private static bool EXP_P_M(uint S, ref string T)
        {
            string LT = "";
            if (EXP_M_D_M_F(S,ref LT))
            {
                if (EXP_P_M1(S, LT, ref T))
                {
                    return true;
                }
            }
            return false;
        }

        //<EXP_P_M1>  PLUS <EXP_M_D_M_F> <EXP_P_M1>| MINUS <EXP_M_D_M> <EXP_P_M1> | Ɛ  
        private static bool EXP_P_M1(uint S, string LT, ref string T)
        {
            string op, RT = "", T1;
            if ( Tokens[index].token == "+" || Tokens[index].token == "-")
            {
                op = Tokens[index].token; // be it plus or minus
                index++;
                if (EXP_M_D_M_F(S, ref RT))
                {
                    T1 = SA.Compatible(LT, RT, op);
                    if (EXP_P_M1(S,T1,ref T))
                    {
                        return true;
                    }
                }
            }
            // handling null
            else if (Tokens[index].token == "+" ||
                Tokens[index].token == "-" ||
                Tokens[index].token == "ComparisionOperator" ||
                Tokens[index].token == "EqualityOperator" ||
                Tokens[index].token == "and" ||
                Tokens[index].token == "or" ||
                Tokens[index].token == ":" ||
                Tokens[index].token == ")" ||
                Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                T = LT;
                return true;
            }

            return false;
        }

        //<EXP_M_D_M_F>  < EXP_COMP_UNARY > <EXP_M_D_M_F1>
        private static bool EXP_M_D_M_F(uint S, ref string T)
        {
            string LT = "";
            if (EXP_COMP_UNARY(S,ref LT))
            {

                if (EXP_M_D_M_F1(S,LT,ref T))
                {
                    return true;
                }
            }
            return false;
        }

        //<EXP_M_D_M_F1>  M_D_M_F < EXP_COMP_UNARY > <EXP_M_D_M_F1> | Ɛ
        private static bool EXP_M_D_M_F1(uint S, string LT, ref string T)
        {
            string op, RT = "", T1;
            if (Tokens[index].token == "*" || Tokens[index].token == "/" || Tokens[index].token == "%" || Tokens[index].token == "//")
            {
                op = Tokens[index].token;
                index++;
                if (EXP_COMP_UNARY(S, ref RT))
                {
                    T1 = SA.Compatible(LT,RT, op);
                    if ( T1 == null)
                    {
                        MessageBox.Show(String.Format("Semantic Error: Cannot \"{2}\" {0}, {1}", LT, RT, op));
                    }
                    if (EXP_M_D_M_F1(S,T1, ref T))
                    {
                        return true;
                    }
                }
            }
                // handling null
            else if (
                Tokens[index].token == "*" ||
                Tokens[index].token == "/" ||
                Tokens[index].token == "//" ||
                Tokens[index].token == "%" ||
                Tokens[index].token == "+" ||
                Tokens[index].token == "-" ||
                Tokens[index].token == "ComparisionOperator" ||
                Tokens[index].token == "EqualityOperator" ||
                Tokens[index].token == "and" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "or" ||
                Tokens[index].token == ":" ||
                Tokens[index].token == ")" ||
                Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                T = LT;
                return true;
            }
            return false;
        }

        //<EXP_COMP_UNARY> -> ~ <EXP_EXPONENTIAL> | <EXP_EXPONENTIAL>
        private static bool EXP_COMP_UNARY(uint S, ref string T)
        {
            string op, RT= "";
            if (Tokens[index].token == "~" || Tokens[index].token == "+" || Tokens[index].token == "-")
            {
                index++;
                op = Tokens[index].token;
                if (EXP_EXPONENTIAL(S,ref RT))
                {
                    T = SA.Compatible(RT, op);
                    if (T==null)
                    {
                        MessageBox.Show(String.Format("Semantic Error: Cannot apply \"{1}\" to {0}", RT, op));
                    }
                    return true;
                }
            }
            else if (EXP_EXPONENTIAL(S, ref T))
            {
                return true;
            }

            return false;
        }
        
        //<EXP_EXPONENTIAL> -> <EXP_CONST> <EXP_EXPONENTIAL1> 
        private static bool EXP_EXPONENTIAL(uint S, ref string T)
        {
            string LT = "";
            if (EXP_CONST(S,ref LT))
            {
                if (EXP_EXPONENTIAL1(S, LT,ref T))
                {
                    return true;
                }
            }
            return false;
        }

        //<EXP_EXPONENTIAL1> -> ** <EXP_CONST><EXPONENTIAL1> | Ɛ
        private static bool EXP_EXPONENTIAL1(uint S, string LT, ref string T)
        {
            string op, RT = "", T1;
            if (Tokens[index].token == "**")
            {
                op = "**";
                index++;
                if (EXP_CONST(S, ref RT))
                {
                    T1 = SA.Compatible(LT, RT, op);
                    if (T1 == null)
                    {
                        MessageBox.Show(String.Format("Semantic Error: Cannot \"{2}\" {0}, {1}", LT, RT, op));
                    }
                    if (EXP_EXPONENTIAL1(S, T1,ref T))
                    {
                        return true;
                    }
                }
            }
            // Handling Null 
            //Follow(<EXP_EXPONENTIAL1>) { M_D_M_F, PLUS, MINUS, COMPARISION_OP, EQUALITY_OP, AND, OR, : , ) 
            // , while, for, if, repeat, def, pass, {, [, 
            //STRING_CONST, INT_CONST, FLOAT_CONST, ID, Body_End, , }
            else if (Tokens[index].token == "/" ||
                Tokens[index].token == "*" ||
                Tokens[index].token == "//" ||
                Tokens[index].token == "-" ||
                Tokens[index].token == "+" ||
                Tokens[index].token == "ComparisionOperator" ||
                Tokens[index].token == "EqualityOperator" ||
                Tokens[index].token == "and" ||
                Tokens[index].token == "or" ||
                Tokens[index].token == ":" ||
                Tokens[index].token == ")" ||
                Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END" )
            {
                T = LT;
                return true;
            }
            return false;
        }

        //<EXP_CONST>  ID <EXP_CONST1>| <CONST> | (<EXPRESSION>)
        private static bool EXP_CONST(uint S, ref string T)
        {
            if (Tokens[index].token == "ID")
            {
                string Name =Tokens[index].word;
                string TempType = SA.LookUp(Name, SA.ScopeStack);
                uint ParamCount = 0;
                index++;
                if (EXP_CONST1(S, ref ParamCount))
                {
                    if (TempType==null)
                    {
                        MessageBox.Show(String.Format("Semantic Error: No Such identifier as {0}.", Name));
                    }
                    else
                    {
                        if (TempType.Contains("->"))
                        {
                            if (ParamCount.ToString() == TempType.Substring(0,TempType.IndexOf("->")))
                            {
                                int startIndex = TempType.IndexOf("->")+2;
                                int lastIndex = TempType.Length - startIndex;
                                T = TempType.Substring(startIndex, lastIndex);
                            }
                        }
                        else 
                        {
                            T = TempType;
                        }
                    }
                    return true;
                }
            }
            else if (CONST( ref T))
            {
                return true;
            }
            else if ( Tokens[index].token == "(")
            {
                if (EXPRESSION(S, ref T))
                {
                    if (Tokens[index].token == ")")
                    {
                        index++;
                        return true;
                    }
                }
            }
            return false;
        }

        //<EXP_CONST1>  (<FC_PARAMETERS>) | Ɛ
        private static bool EXP_CONST1 (uint S, ref uint ParamCount)
        {
            if (Tokens[index].token == "(")
            {
                index++;
                if (FC_PARAMETERS(S,ref ParamCount))
                {
                    if (Tokens[index].token == ")")
                    {
                        index++;
                        return true;
                    }
                }
            }
                //handling null 
            else if (Tokens[index].token == "**" ||
                Tokens[index].token == "*" ||
                Tokens[index].token == "/" ||
                Tokens[index].token == "//" ||
                Tokens[index].token == "-" ||
                Tokens[index].token == "+" ||
                Tokens[index].token == "ComparisionOperator" ||
                Tokens[index].token == "EqualityOperator" ||
                Tokens[index].token == "and" ||
                Tokens[index].token == "or" ||
                Tokens[index].token == ":" ||
                Tokens[index].token == ")" ||
                Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                return true;
            }

            return false;
        }
        

        // <VALUE> -> <CONST> | <DICTIONARY> | <LIST> 
        private static bool VALUE(uint S, ref string T, ref uint length)
        {
            if (CONST(ref T))
            {
                length++;
                return true;
            }
            else if (DICTIONARY(S,ref length))
            {
                T = "DICTIONARY";
                return true;
            }
            else if (LIST(S, ref length))
            {
                T = "LIST";
                return true;
            }
            // Follow(<VALUE>){ while, for, if, repeat, def, pass, {, [, STRING_CONST, INT_CONST, FLOAT_CONST, ID, Body_End , , , ]  , } }
            //else if (
            //    Tokens[index].token == "while" ||
            //    Tokens[index].token == "for" ||
            //    Tokens[index].token == "if" ||
            //    Tokens[index].token == "repeat" ||
            //    Tokens[index].token == "def" ||
            //    Tokens[index].token == "pass" ||
            //    Tokens[index].token == "{" ||
            //    Tokens[index].token == "[" ||
            //    Tokens[index].token == "STRING_CONST" ||
            //    Tokens[index].token == "INT_CONST" ||
            //    Tokens[index].token == "FLOAT_CONST" ||
            //    Tokens[index].token == "ID" ||
            //    Tokens[index].token == "]" ||
            //    Tokens[index].token == "}" ||
            //    Tokens[index].token == "BODY_END")
            //{
            //    return true;
            //}
            return false;
        }

        //<IF_ELSE>  if <EXPRESSION> : <BODY><N_IF_ELSE>
        private static bool IF_ELSE(uint S)
        {
            string T = "";
            if (Tokens[index].token == "if")
            {
                index++;

                if (EXPRESSION(S,ref T))
                {
                    if (Tokens[index].token == ":")
                    {
                        index++;
                        if (BODY(S))
                        {
                            if (N_IF_ELSE(S))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        //<N_IF_ELSE>  <ELSE> | <ELIF> | Ɛ
        private static bool N_IF_ELSE(uint S)
        {
            if (ELSE(S))
            {
                return true;
            }
            else if (ELIF(S))
            {
                return true;
            }
            // {while, for, if, repeat, def, 
            //pass, {, [, STRING_CONST, INT_CONST, 
            //FLOAT_CONST, ID, Body_End }
            else if (Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END") // handle null here
            {
                return true;
            }
            return false;
        }

        // <ELIF> elif<EXPRESSION> : <BODY> <ELIF><ELSE> | Ɛ
        private static bool ELIF(uint S)
        {
            string T = "";
            if ( Tokens[index].token =="elif")
            {
                index++;
                if (EXPRESSION(S, ref T))
                {
                    if (Tokens[index].token == ":")
                    {
                        index++;
                        if (BODY(S))
                        {
                            if (ELIF(S))
                            {
                                if (ELSE(S))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            //Follow(<ELIF>) {while, for, 
            //if, repeat,else, def, pass,
            //{, [, STRING_CONST, INT_CONST, 
            // FLOAT_CONST, ID, Body_End }
            else if (Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "else" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")// handling null
            {
                return true;
            }
            return false;
        }

        //<ELSE> -> else: <BODY> | Ɛ
        private static bool ELSE(uint S) 
        {
            if (Tokens[index].token == "else")
            {
                index++;
                if (Tokens[index].token == ":")
                {
                    index++;
                    if (BODY(S))
                    {
                        return true;
                    }
                }
                
            } 
            else if (Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                return true;
            }
            return false;
        }

        //<REPEAT_ST>  repeat: <BODY> until<EXPRESSION>:
        private static bool REPEAT_ST(uint S)
        {
            string  T = "";
            if (Tokens[index].token == "repeat")
            {
                index++;

                if (Tokens[index].token == ":")
                {
                    index++;
                    if (BODY(S))
                    {
                        if (Tokens[index].token == "until")
                        {
                            index++;
                            if (EXPRESSION(S,ref T ))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        #region Function Area

        //<FUNCTION_DEF>  def ID (<FD_PARAMETER>) : <FUNC_BODY>

        private static bool FUNCTION_DEF(uint S)
        {
            uint param_count = 0;
            string N = "",T;
            string returnType = "VOID";
            if (Tokens[index].token == "def")
            {
                index++;
                if (Tokens[index].token == "ID")
                {
                    N = Tokens[index].word;
                    index++;
                    if (Tokens[index].token == "(")
                    {
                        index++;
                        S = SA.CreateScope();
                        if (FD_PARAMETER(S, ref param_count))
                        {                            
                            if (Tokens[index].token == ")")
                            {
                                index++;
                                if (Tokens[index].token == ":")
                                {
                                    index++;
                                    if (FUNC_BODY(S, ref returnType))
                                    {
                                        S = SA.DeleteScope();
                                        T = SA.LookUp(N, SA.ScopeStack);
                                        if (T== null)
                                        {
                                            SA.InsertFunction(N, returnType, S, param_count);
                                        }
                                        else
                                        {
                                            SA.Assign(N, returnType, S, param_count, SA.ScopeStack, AssignType.Function);
                                        }
                                        
                                        return true;
                                    }
                                }
                            }
                            
                        }
                    }
                }
            }
            return false;
        }

        //<FD_PARAMETER>  ID < FD_N_PARAMETER> | Ɛ
        private static bool FD_PARAMETER(uint S, ref uint count)
        {
            string T = "";
            if (Tokens[index].token == "ID")
            {
                string N = Tokens[index].word;
                T = SA.LookUpFunc(N, S);
                if (T == "FUNC_PARAM")
                {
                    MessageBox.Show(String.Format("Already used Identifier {0} before", N), "Semantic Error");
                }
                else
                {
                    SA.Insert(N, "", S, 1);
                }
                count++;
                index++;
                if (FD_N_PARAMETER(S,ref count))
                {
                    return true;
                }
            }
                // handling null
                //Follow(<FD_PARAMETERS>) { ) }
            else if ( Tokens[index].token == ")")
            {
                return true;
            }
            return false;
        }

        //<FD_N_PARAMETER>, < FD_PARAMETER> | Ɛ
        private static bool FD_N_PARAMETER(uint S, ref uint count)
        {
            if (Tokens[index].token == ",")
            {
                index++;
                if (FD_PARAMETER(S, ref count))
                {
                    return true;
                }
            }
                // handling null
                //Follow(<FD_N_PARAMETERS>)  { ) }
            else if (Tokens[index].token == ")")
            {
                return true;
            }
            return false;
        }

        //<FUNC_BODY>  Body_Start<FUNC_MULTI_ST>Body_End

        private static bool FUNC_BODY(uint S, ref string type)
        {
            if (Tokens[index].token == "BODY_START")
            {
                index++;
                if (FUNC_MULTI_ST(S, ref type))
                {
                    if (Tokens[index].token == "BODY_END")
                    {
                        index++;
                        return true;
                    }
                }
            }
            return false;
        }
        //<FUNC_MULTI_ST>  <FUNC_SINGLE_ST><FUNC_NEXT> 
        private static bool FUNC_MULTI_ST(uint S, ref string RT)
        {
            //<NEXT>
            if (FUNC_SINGLE_ST(S, ref RT))
            {
                // <SINGLE_ST>
                if (FUNC_NEXT(S, ref RT))
                {
                    return true;
                }
            }
            return false;
        }

        //<FUNC_NEXT>  <FUNC_SINGLE_ST> <FUNC_NEXT> | Ɛ
        private static bool FUNC_NEXT(uint S, ref string RT)
        {
            // handling null
            if (Tokens[index].token == "BODY_END")
            {
                return true;
            }
            else if (FUNC_SINGLE_ST(S, ref RT))
            {
                if (FUNC_NEXT(S,ref RT))
                {
                    return true;
                }
            }
            
            return false;
        }

        //<FUNC_SINGLE_ST>  ID <SINGLE_ST1> | < FUNC_WHILE_ST> | < FUNC_FOR_ST> | < FUNC_IF_ELSE> |    
        //            <VALUE> |  <FUNC_REPEAT_ST> | <FUNCTION_DEF> | <RETURN> |pass 
        private static bool FUNC_SINGLE_ST(uint S, ref string type)
        {
            string T = "";
            uint paramCount_Size = 0;

            //ID <SINGLE_ST1> 
            if (Tokens[index].token == "ID")
            {
                string N = Tokens[index].word;

                index++;
                if (SINGLE_ST1(S,ref T,ref paramCount_Size))
                {
                    string T1 = "";
                    T1 = SA.LookUp(N, SA.ScopeStack);
                    if (T1 != null)
                    {
                        if (T1.Contains("->"))
                        {
                            if (paramCount_Size.ToString() != T1.Substring(0, T1.IndexOf("->")))
                            {
                                MessageBox.Show(String.Format("Semantic: No Function with {0} Parameter(s)", paramCount_Size));
                            }
                        }
                        else
                        {
                            SA.Assign(N, T, S, paramCount_Size, SA.ScopeStack, AssignType.Variable);
                        }
                    }
                    else
                    {
                        if (T != "" || T != null)
                        {
                            SA.Insert(N, T, S, 1);
                        }
                        else
                        {
                            MessageBox.Show(String.Format("Semantic: No such Function {0} ", N));
                        }
                    }
                    return true;
                }
            }

            // <FUNC_WHILE_ST> 
            if (FUNC_WHILE_ST(S,ref type))
            {
                return true;
            }
            // <FUNC_FOR_ST>
            if (FUNC_FOR_ST(S, ref type))
            {
                return true;
            }

            // <FUNC_IF_ELSE> 
            if (FUNC_IF_ELSE(S, ref type))
            {
                return true;
            }

            // <VALUE> 
            if (VALUE(S, ref T, ref paramCount_Size ))
            {
                return true;
            }
            // <FUNC_REPEAT_ST>  
            if (FUNC_REPEAT_ST(S, ref type))
            {
                return true;
            }
            // <FUNCTION_DEF>
            if (FUNCTION_DEF(S))
            {
                return true;
            }
            // <RETURN> 
            if (RETURN(S,ref type))
            {
                
                return true;
            }
            // pass 
            if (Tokens[index].token == "pass")
            {
                index++;
                return true;
            }
            return true;
        }

        //<RETURN >  return <RET_VALUE>
        private static bool RETURN(uint S, ref string T)
        {
            uint count = 0;
            if (Tokens[index].token == "return")
            {
                index++;
                if (RET_VALUE(S,ref T,ref count))
                {
                    return true;
                }
            }
            return false;
        }
        //<RET_VALUE>  <EXPRESSION> | <LIST> | <DICTIONARY>
        private static bool RET_VALUE(uint S, ref string T,ref uint count)
        {
            
            if (EXPRESSION(S, ref T))
            {
                return true;
            }
            else if (LIST(S, ref count))
            {
                return true;
            }
            else if (DICTIONARY(S,ref count))
            {
                return true;
            }
            return false;
        }

        //<FUNC_ELSE>  else: < FUNC_BODY> | Ɛ
        private static bool FUNC_ELSE(uint S, ref string RT)
        {
            if (Tokens[index].token == "else")
            {
                index++;
                if (Tokens[index].token == ":")
                {
                    index++;
                    if (FUNC_BODY(S, ref RT))
                    {
                        return true;
                    }
                }
            }
                //Follow(<FUNC_ELSE>){  ID, while, for, if,
            //FLOAT_CONST, INT_CONST, STRING_CONST,
            //{, [, repeat, def, return, pass , Body_End }
                // handling null
            else if (Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")
            {
                return true;
            }
            return false;
        }

        //<FUNC_FOR_ST> for ID in <SEQUENCE> < FUNC_BODY><FUNC_ELSE>
        private static bool FUNC_FOR_ST(uint S, ref string RT)
        {
            var T = "";
            uint length = 0;
            if (Tokens[index].token == "for")
            {
                index++;
                if (Tokens[index].token == "ID")
                {
                    index++;
                    if (Tokens[index].token == "in")
                    {
                        index++;
                        if (SEQUENCE(S,ref T,ref length ))
                        {
                            if (Tokens[index].token == ":")
                            {
                                index++;
                                if (FUNC_BODY(S, ref RT) && FUNC_ELSE(S, ref RT))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                
            }

            return false;
        }


        //<FUNC_WHILE_ST> while <EXPRESSION> : < FUNC_BODY>< FUNC_ELSE>
        private static bool FUNC_WHILE_ST(uint S, ref string RT)
        {
            string T = "";
            if (Tokens[index].token == "while")
            {
                index++;
                if (EXPRESSION(S,ref T))
                {
                    if (Tokens[index].token == ":")
                    {
                        index++;

                        if (FUNC_BODY(S, ref RT) && FUNC_ELSE(S,ref RT))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // <FUNC_REPEAT_ST>  repeat: < FUNC_BODY> until <EXPRESSION>:
        private static bool FUNC_REPEAT_ST(uint S,ref string RT)
        {

            string T = "";
            if (Tokens[index].token == "repeat")
            {
                index++;

                if (Tokens[index].token == ":")
                {
                    index++;
                    if (FUNC_BODY(S, ref RT))
                    {
                        if (Tokens[index].token == "until")
                        {
                            index++;
                            if (EXPRESSION(S,ref T))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        
        
        //<FUNC_ELSE>  else: < FUNC_BODY> | Ɛ

        //<FUNC_IF_ELSE>  if<EXPRESSION> : < FUNC_BODY>< FUNC_N_IF_ELSE>
        private static bool FUNC_IF_ELSE(uint S,ref string RT)
        {
            string T = "";
            if (Tokens[index].token == "if")
            {
                index++;

                if (EXPRESSION(S, ref T))
                {
                    if (Tokens[index].token == ":")
                    {
                        index++;
                        if (FUNC_BODY(S,ref RT))
                        {
                            if (FUNC_N_IF_ELSE(S,ref RT))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        //<FUNC_N_IF_ELSE>  < FUNC_ELSE> | < FUNC_ELIF> | Ɛ
        private static bool FUNC_N_IF_ELSE(uint S, ref string RT)
        {
            if (FUNC_ELSE(S, ref RT))
            {
                return true;
            }
            else if (FUNC_ELIF(S,ref RT))
            {
                return true;
            }
            // {while, for, if, repeat, def, 
            //pass, {, [, STRING_CONST, INT_CONST, 
            //FLOAT_CONST, ID, Body_End }
            else if (Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END") // handle null here
            {
                return true;
            }
            return false;
        }

        //<FUNC_ELIF> elif <EXPRESSION>: < FUNC_BODY> <FUNC_ELIF>< FUNC_ELSE> | Ɛ
        private static bool FUNC_ELIF(uint S, ref string RT)
        {
            string T = "";
            if (Tokens[index].token == "elif")
            {
                index++;
                if (EXPRESSION(S, ref T ))
                {
                    if (Tokens[index].token == ":")
                    {
                        index++;
                        if (FUNC_BODY(S, ref RT))
                        {
                            if (FUNC_ELIF(S, ref RT))
                            {
                                if (FUNC_ELSE(S, ref RT))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            //Follow(<FUNC_N_IF_ELSE>) {  ID, while, for, if, 
            //FLOAT_CONST, INT_CONST, STRING_CONST, else, 
            // {, [, repeat, def, return, pass , Body_End }
            else if (Tokens[index].token == "while" ||
                Tokens[index].token == "for" ||
                Tokens[index].token == "if" ||
                Tokens[index].token == "repeat" ||
                Tokens[index].token == "def" ||
                Tokens[index].token == "pass" ||
                Tokens[index].token == "else" ||
                Tokens[index].token == "{" ||
                Tokens[index].token == "[" ||
                Tokens[index].token == "return" ||
                Tokens[index].token == "STRING_CONST" ||
                Tokens[index].token == "INT_CONST" ||
                Tokens[index].token == "FLOAT_CONST" ||
                Tokens[index].token == "ID" ||
                Tokens[index].token == "BODY_END")// handling null
            {
                return true;
            }
            return false;
        }

       
        #endregion

    }
}
