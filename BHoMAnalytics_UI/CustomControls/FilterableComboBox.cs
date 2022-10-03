/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BH.UI.Analytics.CustomControls
{
    public class FilterableComboBox : ComboBox
    {
        #region Properties
        private string _oldFilter = string.Empty;

        private string _currentFilter = string.Empty;

        protected TextBox EditableTextBox => GetTemplateChild("PART_EditableTextBox") as TextBox;

        private bool _haveAddedItem = false;
        #endregion

        #region Ctor
        public FilterableComboBox()
        {

        }
        #endregion

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue != null)
            {
                var view = CollectionViewSource.GetDefaultView(newValue);
                view.Filter += FilterItem;
            }

            if (oldValue != null)
            {
                var view = CollectionViewSource.GetDefaultView(oldValue);
                if (view != null) view.Filter -= FilterItem;
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                case Key.Enter:
                    IsDropDownOpen = false;
                    break;
                case Key.Escape:
                    IsDropDownOpen = false;
                    SelectedIndex = -1;
                    Text = _currentFilter;
                    break;
                default:
                    if (e.Key == Key.Down)
                        IsDropDownOpen = true;
                    base.OnPreviewKeyDown(e);
                    break;
            }

            _oldFilter = Text;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    break;
                case Key.Tab:
                case Key.Enter:
                    ClearFilter();
                    break;
                default:
                    if (Text != _oldFilter)
                    {
                        var temp = Text;
                        RefreshFilter(); //RefreshFilter will change Text property
                        Text = temp;

                        if (SelectedIndex != -1 && Text != Items[SelectedIndex].ToString())
                        {
                            SelectedIndex = -1; //Clear selection. This line will also clear Text property
                            Text = temp;
                        }

                        IsDropDownOpen = true;
                        EditableTextBox.SelectionStart = int.MaxValue;
                    }

                    //Automatically select the item when the input text matches it
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Text == Items[i].ToString())
                            SelectedIndex = i;
                    }

                    if (_haveAddedItem)
                        (this.ItemsSource as ObservableCollection<string>).RemoveAt((ItemsSource as ObservableCollection<string>).Count - 1);

                    if (Items.Count == 0)
                    {
                        var temp = Text;
                        (this.ItemsSource as ObservableCollection<string>).Add(temp);
                        RefreshFilter();
                        Text = temp;
                        _haveAddedItem = true;
                    }

                    base.OnKeyUp(e);
                    _currentFilter = Text;
                    break;
            }
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            ClearFilter();
            var temp = SelectedIndex;
            SelectedIndex = -1;
            Text = string.Empty;
            SelectedIndex = temp;
            base.OnPreviewLostKeyboardFocus(e);
        }

        private void RefreshFilter()
        {
            if (ItemsSource == null)
                return;

            var view = CollectionViewSource.GetDefaultView(ItemsSource);
            view.Refresh();
        }

        private void ClearFilter()
        {
            _currentFilter = string.Empty;
            RefreshFilter();
        }

        private bool FilterItem(object value)
        {
            if (value == null)
                return false;

            if (Text.Length == 0)
                return true;

            return value.ToString().ToLower().Contains(Text.ToLower());
        }
    }
}