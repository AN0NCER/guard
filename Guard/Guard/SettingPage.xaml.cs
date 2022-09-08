using System;
using System.Collections.Generic;
using ZXing.QrCode;
using Xamarin.Forms;
using System.IO;
using ZXing.Mobile;
using ZXing.QrCode.Internal;
using Guard.CPopup;
using Xamarin.CommunityToolkit.Extensions;
using Guard.Library;
using Xamarin.CommunityToolkit.ObjectModel;
using Guard.Interface;
using System.Collections.ObjectModel;
using Guard.CData;
using Xamarin.Forms.PancakeView;
using System.Windows.Input;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;
using System.Reflection;

namespace Guard
{
    public partial class SettingPage : ContentPage
    {

        public ICommand ItemDragged { get; }

        public ICommand ItemDraggedOver { get; }

        public ICommand ItemDragLeave { get; }

        public ICommand ItemDropped { get; }

        public SettingPage()
        {
            InitializeComponent();
            PositionAccounts.ItemsSource = IO.Files;
            ItemDraggedOver = new Command<AFile>(OnItemDraggedOver);
            ItemDragLeave = new Command<AFile>(OnItemDragLeave);
            ItemDropped = new Command<AFile>(i => OnItemDropped(i));
            ItemDragged = new Command<AFile>(OnItemDragged);
            this.BindingContext = this;
        }

        private void OnItemDragged(AFile item)
        {
            Debug.WriteLine($"OnItemDragged: {item?.Name}");
            IO.Files.ForEach(i => i.IsBeingDragged = item == i);
        }

        private void OnItemDraggedOver(AFile item)
        {
            Debug.WriteLine($"OnItemDraggedOver: {item?.Name}");
            var itemBeingDragged = IO.Files.FirstOrDefault(i => i.IsBeingDragged);
            IO.Files.ForEach(i => i.IsBeingDraggedOver = item == i && item != itemBeingDragged);
        }

        private void OnItemDragLeave(AFile item)
        {
            Debug.WriteLine($"OnItemDragLeave: {item?.Name}");
            IO.Files.ForEach(i => i.IsBeingDraggedOver = false);
        }

        private async Task OnItemDropped(AFile item)
        {
            var itemToMove = IO.Files.First(i => i.IsBeingDragged);
            var itemToInsertBefore = item;

            if (itemToMove == null || itemToInsertBefore == null || itemToMove == itemToInsertBefore)
                return;

            // Wait for remove animation to be completed
            // https://github.com/xamarin/Xamarin.Forms/issues/13791
            // await Task.Delay(1000);

            var insertAtIndex = IO.Files.IndexOf(itemToInsertBefore);
            var itemAtIndex = IO.Files.IndexOf(itemToMove);
            itemToMove.IsBeingDragged = false;
            itemToInsertBefore.IsBeingDraggedOver = false;
            Debug.WriteLine($"OnItemDropped: [{itemToMove?.Name}] => [{itemToInsertBefore?.Name}], target index = [{insertAtIndex}]");

            IO.Files.Move(itemAtIndex, insertAtIndex);
            /*PrintItemsState();*/

            var mp = (IAccountMove)Application.Current.MainPage;
            if(mp != null)
                mp.AccountMove(itemAtIndex, insertAtIndex);
        }

        void ResetBTN_Clicked(System.Object sender, System.EventArgs e)
        {
            Account.Remove();
        }
    }
}
