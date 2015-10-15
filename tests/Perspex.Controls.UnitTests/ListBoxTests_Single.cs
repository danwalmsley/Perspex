﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Linq;
using Perspex.Controls.Presenters;
using Perspex.Controls.Templates;
using Perspex.Input;
using Perspex.LogicalTree;
using Perspex.Styling;
using Xunit;

namespace Perspex.Controls.UnitTests
{
    public class ListBoxTests_Single
    {
        [Fact]
        public void Focusing_Item_With_Tab_Should_Not_Select_It()
        {
            var target = new ListBox
            {
                Template = new ControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            target.ApplyTemplate();

            target.Presenter.Panel.Children[0].RaiseEvent(new GotFocusEventArgs
            {
                RoutedEvent = InputElement.GotFocusEvent,
                NavigationMethod = NavigationMethod.Tab,
            });

            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void Focusing_Item_With_Arrow_Key_Should_Select_It()
        {
            var target = new ListBox
            {
                Template = new ControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            target.ApplyTemplate();

            target.Presenter.Panel.Children[0].RaiseEvent(new GotFocusEventArgs
            {
                RoutedEvent = InputElement.GotFocusEvent,
                NavigationMethod = NavigationMethod.Directional,
            });

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Item_Should_Select_It()
        {
            var target = new ListBox
            {
                Template = new ControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            target.ApplyTemplate();

            target.Presenter.Panel.Children[0].RaiseEvent(new PointerPressEventArgs
            {
                RoutedEvent = InputElement.PointerPressedEvent,
                MouseButton = MouseButton.Left,
            });

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Selected_Item_Should_Not_Deselect_It()
        {
            var target = new ListBox
            {
                Template = new ControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            target.ApplyTemplate();
            target.SelectedIndex = 0;

            target.Presenter.Panel.Children[0].RaiseEvent(new PointerPressEventArgs
            {
                RoutedEvent = InputElement.PointerPressedEvent,
                MouseButton = MouseButton.Left,
            });

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Item_Should_Select_It_When_SelectionMode_Toggle()
        {
            var target = new ListBox
            {
                Template = new ControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Single | SelectionMode.Toggle,
            };

            target.ApplyTemplate();

            target.Presenter.Panel.Children[0].RaiseEvent(new PointerPressEventArgs
            {
                RoutedEvent = InputElement.PointerPressedEvent,
                MouseButton = MouseButton.Left,
            });

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Selected_Item_Should_Deselect_It_When_SelectionMode_Toggle()
        {
            var target = new ListBox
            {
                Template = new ControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Toggle,
            };

            target.ApplyTemplate();
            target.SelectedIndex = 0;

            target.Presenter.Panel.Children[0].RaiseEvent(new PointerPressEventArgs
            {
                RoutedEvent = InputElement.PointerPressedEvent,
                MouseButton = MouseButton.Left,
            });

            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Selected_Item_Should_Not_Deselect_It_When_SelectionMode_ToggleAlwaysSelected()
        {
            var target = new ListBox
            {
                Template = new ControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Toggle | SelectionMode.AlwaysSelected,
            };

            target.ApplyTemplate();
            target.SelectedIndex = 0;

            target.Presenter.Panel.Children[0].RaiseEvent(new PointerPressEventArgs
            {
                RoutedEvent = InputElement.PointerPressedEvent,
                MouseButton = MouseButton.Left,
            });

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Another_Item_Should_Select_It_When_SelectionMode_Toggle()
        {
            var target = new ListBox
            {
                Template = new ControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Single | SelectionMode.Toggle,
            };

            target.ApplyTemplate();
            target.SelectedIndex = 1;

            target.Presenter.Panel.Children[0].RaiseEvent(new PointerPressEventArgs
            {
                RoutedEvent = InputElement.PointerPressedEvent,
                MouseButton = MouseButton.Left,
            });

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Setting_Item_IsSelected_Sets_ListBox_Selection()
        {
            var target = new ListBox
            {
                Template = new ControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            target.ApplyTemplate();

            ((ListBoxItem)target.GetLogicalChildren().ElementAt(1)).IsSelected = true;

            Assert.Equal("Bar", target.SelectedItem);
            Assert.Equal(1, target.SelectedIndex);
        }

        private Control CreateListBoxTemplate(ITemplatedControl parent)
        {
            return new ScrollViewer
            {
                Template = new ControlTemplate(CreateScrollViewerTemplate),
                Content = new ItemsPresenter
                {
                    Name = "itemsPresenter",
                    [~ItemsPresenter.ItemsProperty] = parent.GetObservable(ItemsControl.ItemsProperty),
                }
            };
        }

        private Control CreateScrollViewerTemplate(ITemplatedControl parent)
        {
            return new ScrollContentPresenter
            {
                [~ContentPresenter.ContentProperty] = parent.GetObservable(ContentControl.ContentProperty),
            };
        }

        private class Item
        {
            public Item(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }
    }
}
