using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace RAA_View_Renumber
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class ResultForm : Window
    {
        public Document myDoc;
        public ResultForm(Document doc, List<string> viewList, List<string> viewID, List<string> viewNumber)
        {
            InitializeComponent();
            myDoc = doc;
            //for (int i = 0; i < viewList.Count; i++)
            //{
            //    lbxViewName.Items.Add(viewList[i] + "\t\t\t\t\t\t\t\t " + viewNumber[i] + "\t\t " + viewID[i]);
            //}
            DataContext = new DataGridViewModel(viewList, viewNumber, viewID);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
