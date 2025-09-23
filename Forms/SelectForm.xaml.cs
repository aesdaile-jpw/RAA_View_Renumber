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
    public partial class SelectForm : Window
    {
        public Document myDoc;
        public SelectForm(Document doc, List<string> viewList, bool selectButton)
        {
            InitializeComponent();
            myDoc = doc;
            if (viewList == null)
            {
                lbxViews.Items.Add("Select views on the sheet in the order you want to renumber them.");
            }
            else
            {
                foreach (string view in viewList)
                {
                    lbxViews.Items.Add(view);
                }
            }
            
            for (int i = 1; i <= 10; i++)
            {
                cbxViewNumber.Items.Add(i.ToString());
            }

            BtnSelect.IsEnabled = selectButton;
        }

        public string GetComboBoxValue()
        {
            return cbxViewNumber.SelectedItem.ToString();
        }

        public List<string> GetListBoxValues()
        {
            List<string> selectedValues = new List<string>();
            foreach (var item in lbxViews.SelectedItems)
            {
                selectedValues.Add(item.ToString());
            }
            return selectedValues;
        }
        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
