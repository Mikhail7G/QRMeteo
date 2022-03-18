using QRMeteo.DBExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRMeteo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemEditorPage : ContentPage
    {
        private InventoryObject SelectedObject;//Выбранный объект
        public ItemEditorPage()
        {
            InitializeComponent();
            SetTapper();
            SelectedObject = new InventoryObject();

        }

        public void SetItemObject(InventoryObject obj)
        {
            if (obj != null) 
            {
                SelectedObject = obj;
                StepperInv.Value = Convert.ToDouble(SelectedObject.Quantity);
                this.BindingContext = SelectedObject;//привязывем объект к контенту страницы
            }
        }

        private void SetTapper()        //открытие ссылки на бд объекта
        {
            TapGestureRecognizer Tapper = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 2
            };

            Tapper.Tapped += (s, e) =>
            {
                OpenURL();
            };
            ObjectLinkLbl.GestureRecognizers.Add(Tapper);
        }

        private async void OpenURL()
        {
            string uri = SelectedObject.TargetHttpPosString;
            try
            {
                if (uri.Length > 0)
                {
                    await Browser.OpenAsync(new Uri(uri), BrowserLaunchMode.SystemPreferred);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void UpdateItem()
        {
            try
            {
                if (SelectedObject != null)
                    App.Database.SaveItem(SelectedObject);
            }
            catch(Exception ex)
            {

            }
        }

        private void NameEtryTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateItem();
        }

        private void InventoryEntryChanged(object sender, TextChangedEventArgs e)
        {
            UpdateItem();
        }

        private void LocationEntryChanged(object sender, TextChangedEventArgs e)
        {
            UpdateItem();
        }

        private void QuantityEntryChanged(object sender, TextChangedEventArgs e)
        {
            UpdateItem();
        }

        private void StepperChanged(object sender, ValueChangedEventArgs e)
        {
            ObjectQuantityEnt.Text = e.NewValue.ToString();
            UpdateItem();
        }
    }
}