using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using EnvDTE;

namespace Aljeida.VisualStudio.Core
{
    public class YUMLURLBuilder
    {
        const string classExpression = "[{0}]";
        const string interfaceExpression = "[<<{0}>>]";
        const string associationExpression = "[{0}]->[{1}]";
        const string cardinalityExpression = "[{0}]{1}-{2}[{3}]";
        const string compositionExpression = "[{0}]++->[{1}]";
        const string inheritanceExpression = "[{0}]^-[{1}]";
        const string interfaceInheritanceExpression = "[<<{0}>>]^-.-[{1}]";
        const string dependenciesExpression = "[{0}]-.->[{1}]";

        public IList<CodeElement> CodeElements { get; set; }
        public bool FirstPass { get; set; }
        public List<Relationship> Relationships = new List<Relationship>();

        private StringBuilder _sb = new StringBuilder();

        public YUMLURLBuilder(IList<CodeElement> codeClasses)
        {
            FirstPass = false;
            this.CodeElements = codeClasses;
        }

        public string Build()
        {
            foreach (var codeElement in this.CodeElements)
            {
                if (codeElement != null)
                {
                    Interfaces(codeElement);

                    if (codeElement is CodeClass)
                    {
                        BaseClasses(codeElement as CodeClass);
                    }

                    AssosiatedTypes(codeElement);

                    if (_sb == null)
                    {
                        if (codeElement is CodeClass)
                        {
                            AppendExpression(string.Format(classExpression, codeElement.Name));
                        }
                        else if (codeElement is CodeInterface)
                        {
                            AppendExpression(string.Format(interfaceExpression, codeElement.Name));
                        }
                    }
                }
            }

            return _sb.ToString();
        }

        private void Interfaces(CodeElement codeElement)
        {
            CodeElements ces = null;

            if (codeElement is CodeClass)
            {
                ces = (codeElement as CodeClass).ImplementedInterfaces;
            }
            else if (codeElement is CodeInterface)
            {
                ces = (codeElement as CodeInterface).Bases;
            }
            else
            {
                return;
            }

            foreach (CodeElement ce in ces)
            {
                CodeInterface codeInterface = ce as CodeInterface;
                if (codeInterface.Name != null)
                {
                    string i = string.Format(interfaceInheritanceExpression, codeInterface.Name, codeElement.Name);
                    AppendExpression(i);
                }
            }
        }

        private void BaseClasses(CodeClass codeClass)
        {
            if (codeClass.Bases == null) return;

            foreach (CodeElement ce in codeClass.Bases)
            {
                CodeClass cc = ce as CodeClass;
                if (cc != null)
                {
                    Interfaces(ce);
                    BaseClasses(cc);

                    string i = string.Format(inheritanceExpression, ce.Name, codeClass.Name);
                    AppendExpression(i);
                }
            }
        }

        private void AssosiatedTypes(CodeElement codeElement)
        {
            CodeElements ces = null;

            if (codeElement is CodeClass)
            {
                ces = (codeElement as CodeClass).Members;
            }
            else if (codeElement is CodeInterface)
            {
                ces = (codeElement as CodeInterface).Members;
            }
            else { return; }

            foreach (CodeElement property in ces)
            {
                CodeProperty prop = property as CodeProperty;
                if (prop != null)
                {
                    Debug.WriteLine(prop.Type.CodeType);
                    string i = string.Format(associationExpression, codeElement.Name,
                                                        prop.Type.CodeType.Name);
                    AppendExpression(i);
                }
            }
        }

        private void AppendExpression(string expression)
        {
            if (expression == "")
                return;

            if (_sb == null)
                _sb = new StringBuilder();

            if (_sb.ToString() == "")
            {
                _sb.Append(expression);
            }
            else
            {
                _sb.AppendFormat(",{0}", expression);
            }
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
