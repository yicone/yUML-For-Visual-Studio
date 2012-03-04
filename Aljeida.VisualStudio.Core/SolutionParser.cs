using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Aljeida.VisualStudio.Core
{
    public class SolutionParser
    {
        private DTE2 _dte;
        public SolutionParser()
        {
            _dte = (DTE2)Package.GetGlobalService(typeof(DTE)); ;
        }

        [Obsolete("当前未使用. 原考虑从程序集中提取信息生成yUML代码")]
        private string GetSelectedProjectAssemblyPath()
        {
            Project proj = GetSelectedProject();

            string projPath = proj.Properties.Item("LocalPath").Value.ToString();
            string outputFileName = proj.Properties.Item("OutputFileName").Value.ToString();
            string outputPath = proj.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
            var assemblyPath = Path.Combine(Path.Combine(projPath, outputPath), outputFileName);
            return assemblyPath;
        }

        private Project GetSelectedProject()
        {
            Project project = null;
            UIHierarchyItem selectedItem = null;
            UIHierarchy se = _dte.ToolWindows.SolutionExplorer;
            object[] items = se.SelectedItems as object[];

            if (items != null || items.Length > 0)
            {
                selectedItem = items[0] as UIHierarchyItem;
            }

            Debug.Assert(selectedItem != null && selectedItem.IsSelected);
            // 获取选中项
            ProjectItem projectItem = selectedItem.Object as ProjectItem;
            // 获取选中项所在的项目
            project = projectItem.ContainingProject;

            //Debug.Write("-------------------------华丽的分割线---------------------------");
            //for (int i = 1; i <= proj.Properties.Count; i++)
            //{
            //    Property p = proj.Properties.Item(i);
            //    try
            //    {
            //        Debug.WriteLine(p.Name + ", " + p.Value);
            //    }
            //    catch (Exception ex)
            //    {
            //        //Debug.Write(ex);
            //    }
            //}

            return project;
        }

        /// <summary>
        /// 获得VS中选定的代码项(SubType为Code的ProjectItem),
        /// 如果同时选中多个, 只返回第一个
        /// </summary>
        /// <param name="subType"></param>
        /// <returns></returns>
        public ProjectItem GetSelectedFirstCodeProjectItem()
        {
            ProjectItem projectItem = null;

            UIHierarchyItem selectedUIHierarchyItem = null;
            UIHierarchy se = _dte.ToolWindows.SolutionExplorer;
            object[] items = se.SelectedItems as object[];

            // 如果Solution Explorer中有多个元素同时被选中, 则当前只处理第一个选中的元素
            if (items != null || items.Length > 0)
            {
                selectedUIHierarchyItem = items[0] as UIHierarchyItem;
            }

            Debug.Assert(selectedUIHierarchyItem.IsSelected);

            // 如果选中项没有子元素
            if (selectedUIHierarchyItem.UIHierarchyItems.Count == 0)
            {
                ProjectItem pi = selectedUIHierarchyItem.Object as ProjectItem;
                Property p = pi.Properties.Item("SubType");
                if (string.Equals(p.Value, "Code"))
                {
                    projectItem = pi;
                }
            }
            else
            {
                for (int i = 1; i <= selectedUIHierarchyItem.UIHierarchyItems.Count; i++)
                {
                    UIHierarchyItem temp = selectedUIHierarchyItem.UIHierarchyItems.Item(i);
                    ProjectItem pi = temp.Object as ProjectItem;
                    Property p = pi.Properties.Item("SubType");
                    if (string.Equals(p.Value, "Code"))
                    {
                        projectItem = pi;
                        //Debug.WriteLine(pi.Properties.Item("IsDependentFile").Value == true);
                    }
                }
            }

            return projectItem;
        }

        public List<CodeElement> GetCodeElementList(ProjectItem codeProjectItem)
        {
            // TODO: 增加对codeProjectItem 参数的检查, 以确保其对应的是代码文件
            List<CodeElement> codeElementList = new List<CodeElement>();

            Debug.Assert(codeProjectItem != null);
            Debug.Assert(codeProjectItem.FileCodeModel != null);

            CodeElements ces = codeProjectItem.FileCodeModel.CodeElements;
            for (int i = 1; i <= ces.Count; i++)
            {
                CodeElement ce = ces.Item(i);

                for (int j = 1; j <= ce.Children.Count; j++)
                {
                    CodeElement ce2 = ce.Children.Item(j);
                    codeElementList.Add(ce2);
                }
            }

            return codeElementList;
        }
    }
}
