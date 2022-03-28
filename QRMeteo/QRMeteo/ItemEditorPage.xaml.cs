using QRMeteo.DBExcel;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRMeteo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemEditorPage : ContentPage
    {
        private InventoryObject selectedObject;//Выбранный объект
        public ItemEditorPage()
        {
            InitializeComponent();
            SetTapper();

            selectedObject = new InventoryObject();
        }

        /// <summary>
        /// Отображение в окне информацию о выбранном объекте путем привязки к компонентам
        /// </summary>
        public void SetItemObject(InventoryObject obj)
        {
            if (obj != null) 
            {
                selectedObject = obj;
                StepperInv.Value = Convert.ToDouble(selectedObject.Quantity);
                this.BindingContext = selectedObject;//привязывем объект к контенту страницы
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

        /// <summary>
        /// Открываем ссылку на бд в гугл таблице
        /// </summary>
        private async void OpenURL()
        {
            string uri = selectedObject.TargetHttpPosString;
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
                if (selectedObject != null)
                    App.Database.SaveItem(selectedObject);
            }
            catch(Exception ex)
            {

            }
        }

        private void TextEntryChanged(object sender, TextChangedEventArgs e)
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