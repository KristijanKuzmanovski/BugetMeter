
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.Drawing;

namespace BudgetMeter.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowRecordsPage : ContentPage
    {
        public List<Models.Record> list;
        public ObservableCollection<Models.Record> Items { get; set; }
        public ShowRecordsPage(List<Models.Record> _list = null,string _range = "")
        {
            InitializeComponent();

            range.Text = _range;
            list = _list;
            calc_sums();
        }
       private void calc_sums()
        {
            var income_sum = 0.0;
            var expense_sum = 0.0;
            foreach(var r in list)
            {
                if (r.type.Equals("income"))
                {
                    income_sum += r.amount;
                }
                else
                {
                    expense_sum += r.amount;
                }
            }
            expense.Text = $"Expense: {expense_sum.ToString()}";
            income.Text = $"Income: {income_sum.ToString()}";
        }
        private void MyListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.Record model = e.CurrentSelection.FirstOrDefault() as Models.Record;

            if(model != null)
            {
                Navigation.PushAsync(new Pages.EditRecordPage(model,this));
            }

            //Deselect Item
            ((CollectionView)sender).SelectedItem = null;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            IEnumerable<Models.Record> query = list.OrderByDescending(r => r.date);
            Items = new ObservableCollection<Models.Record>(query);
            MyListView.ItemsSource = Items;
            calc_sums();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Create a new PDF document.
                PdfDocument doc = new PdfDocument();
                //Add a page.
                PdfPage page = doc.Pages.Add();
                //Create a PdfGrid.
                PdfGrid pdfGrid = new PdfGrid();

                //Create PDF graphics for the page
                PdfGraphics graphics = page.Graphics;

                //Set the standard font
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);

                //Draw the text
                graphics.DrawString($"{income.Text} {expense.Text}", font, PdfBrushes.Black, new PointF(20, 0));

                var list2 = new List<Models.PDFrecord>();
                foreach(Models.Record r in list)
                {
                    
                    list2.Add(new Models.PDFrecord() { category = r.category != null ? r.category.name:"other", type = r.type, amount = r.amount, description = r.description, date = r.date});
                }
                //Add list to IEnumerable
                IEnumerable<object> dataTable = list2;
                //Assign data source.
                pdfGrid.DataSource = dataTable;
                pdfGrid.AllowRowBreakAcrossPages = true;

                PdfGridStyle gridStyle = new PdfGridStyle();
                //Set cell padding, which specifies the space between border and content of the cell
                gridStyle.CellPadding = new PdfPaddings(6, 6, 6, 6);
                pdfGrid.Style = gridStyle;
                //Draw grid to the page of PDF document.
                pdfGrid.Draw(page, new PointF(20, 20));
               
                //Save the PDF document to stream.
                MemoryStream stream = new MemoryStream();
                doc.Save(stream);
                //Close the document.
                doc.Close(true);


                //Save the stream as a file in the device and invoke it for viewing
                string filename = $"budgetmeter_report_{range.Text.Replace("/","-")}.pdf"; 
                Xamarin.Forms.DependencyService.Get<ISave>().SaveAndView(filename, "application / pdf", stream);
            }catch(Exception ex)
            {
                Console.WriteLine($"ERROR: {ex}");
            }
        }

    }
}