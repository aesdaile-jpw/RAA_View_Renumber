#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace RAA_View_Renumber
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // first check if active view is a sheet

            View activeView = doc.ActiveView;
            if (activeView.ViewType != ViewType.DrawingSheet)
            {
                TaskDialog.Show("Error", "Please run this command from a Sheet View.\n\nIf you have a Sheet View Activated, please deactivate it.");
                return Result.Failed;
            }
            // open form
            SelectForm currentForm = new SelectForm(doc, null, true)
            {
                Width = 800,
                Height = 550,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            List<Reference> refList = new List<Reference>();
            bool flag = true;
            while (flag)
            {
                try
                {
                    Reference tempRef = uidoc.Selection.PickObject(ObjectType.Element, "Please select an element. Press 'Esc' to stop selecting.");
                    refList.Add(tempRef);
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    flag = false;
                }
            }

            if (refList.Count == 0)
            {
                TaskDialog.Show("Info", "No elements were selected.");
                return Result.Failed;
            }

            List<string> viewList = new List<string>();
            foreach (Reference r in refList)
            {
                Element e = doc.GetElement(r);
                if (e is Viewport)
                {
                    Viewport vp = e as Viewport;
                    ElementId viewId = vp.ViewId;
                    View v = doc.GetElement(viewId) as View;
                    string viewName = v.Name;
                    viewList.Add(viewName);
                }
            }

            int viewsOnSheet = GetViewCountOnActiveSheet(uidoc);
            if (viewsOnSheet != viewList.Count)
            {
                TaskDialog.Show("Error", "The number of selected views does not match the number of views on the active sheet.\n\nPlease try again.");
                return Result.Failed;
            }


            SelectForm currentForm2 = new SelectForm(doc, viewList, false)
            {
                Width = 800,
                Height = 550,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm2.ShowDialog();

            string startValue = currentForm2.GetComboBoxValue();

            int renumberStart = int.Parse(startValue);

            SetViewportNumbers(uidoc, refList, renumberStart);

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }

        public static int GetViewCountOnActiveSheet(UIDocument uiDoc)
        {
            // Get the active view
            ViewSheet activeSheet = uiDoc.ActiveView as ViewSheet;

            if (activeSheet == null)
            {
                TaskDialog.Show("Error", "The active view is not a sheet.");
                return 0;
            }

            // Get all the viewport elements on the sheet
            Document doc = uiDoc.Document;
            var viewports = new FilteredElementCollector(doc, activeSheet.Id)
                            .OfClass(typeof(Viewport))
                            .ToElements();

            // Return the count of viewports (each viewport corresponds to a view)
            return viewports.Count;
        }

        public static void SetViewportNumbers(UIDocument uiDoc, List<Reference> refList, int startNumber)
        {
            Document doc = uiDoc.Document;
            List<string> viewList = new List<string>();
            List<string> numberList = new List<string>();
            List<string> viewIDList = new List<string>();

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Renumber Viewports");
                int currentNumber = startNumber;
                ViewSheet activeSheet = uiDoc.ActiveView as ViewSheet;
                foreach (Reference r in refList)
                {
                    Element e = doc.GetElement(r);
                    if (e is Viewport)
                    {
                        Viewport vp = e as Viewport;
                        ElementId viewId = vp.ViewId;
                        View v = doc.GetElement(viewId) as View;
                        string viewName = v.Name;
                        vp.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER).Set(currentNumber.ToString() + "X");
                        currentNumber++;
                    }
                }
                currentNumber = startNumber;
                foreach (Reference r in refList)
                {
                    Element e = doc.GetElement(r);
                    if (e is Viewport)
                    {
                        Viewport vp = e as Viewport;
                        ElementId viewId = vp.ViewId;
                        View v = doc.GetElement(viewId) as View;
                        string viewName = v.Name;
                        viewList.Add(viewName);
                        numberList.Add(currentNumber.ToString());
                        viewIDList.Add(viewId.ToString());
                        vp.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER).Set(currentNumber.ToString());
                        currentNumber++;
                    }
                }
                tx.Commit();
                tx.Dispose();

                ResultForm currentForm3 = new ResultForm(doc, viewList, viewIDList, numberList)
                {
                    Width = 800,
                    Height = 550,
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                    Topmost = true,
                };

                currentForm3.ShowDialog();
            }
        }
    }

    public class DataGridRow
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
    }

    public class DataGridViewModel
    {
        public List<DataGridRow> Rows { get; set; }

        public DataGridViewModel(List<string> list1, List<string> list2, List<string> list3)
        {
            Rows = new List<DataGridRow>();

            int maxCount = new[] { list1.Count, list2.Count, list3.Count }.Max();

            for (int i = 0; i < maxCount; i++)
            {
                Rows.Add(new DataGridRow
                {
                    Column1 = i < list1.Count ? list1[i] : string.Empty,
                    Column2 = i < list2.Count ? list2[i] : string.Empty,
                    Column3 = i < list3.Count ? list3[i] : string.Empty
                });
            }
        }
    }
}
