﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Linq;
using Perspex.Collections;
using Perspex.Controls.Generators;
using Perspex.Controls.Presenters;
using Perspex.Controls.Templates;
using Perspex.Input;
using Perspex.VisualTree;
using Xunit;

namespace Perspex.Controls.UnitTests.Presenters
{
    public class ItemsPresenterTests
    {
        [Fact]
        public void Should_Add_Containers()
        {
            var target = new ItemsPresenter
            {
                Items = new[] { "foo", "bar" },
            };

            target.ApplyTemplate();

            Assert.Equal(2, target.Panel.Children.Count);
            Assert.IsType<TextBlock>(target.Panel.Children[0]);
            Assert.IsType<TextBlock>(target.Panel.Children[1]);
            Assert.Equal("foo", ((TextBlock)target.Panel.Children[0]).Text);
            Assert.Equal("bar", ((TextBlock)target.Panel.Children[1]).Text);
        }

        [Fact]
        public void Should_Add_Containers_Of_Correct_Type()
        {
            var target = new ItemsPresenter
            {
                Items = new[] { "foo", "bar" },
            };

            target.ItemContainerGenerator = new ItemContainerGenerator<ListBoxItem>(
                target, 
                ListBoxItem.ContentProperty);
            target.ApplyTemplate();

            Assert.Equal(2, target.Panel.Children.Count);
            Assert.IsType<ListBoxItem>(target.Panel.Children[0]);
            Assert.IsType<ListBoxItem>(target.Panel.Children[1]);
        }

        [Fact]
        public void ItemContainerGenerator_Should_Be_Picked_Up_From_TemplatedControl()
        {
            var parent = new TestItemsControl();
            var target = new ItemsPresenter
            {
                TemplatedParent = parent,
            };

            Assert.IsType<ItemContainerGenerator<TestItem>>(target.ItemContainerGenerator);
        }

        [Fact]
        public void Should_Remove_Containers()
        {
            var items = new PerspexList<string>(new[] { "foo", "bar" });
            var target = new ItemsPresenter
            {
                Items = items,
            };

            target.ApplyTemplate();
            items.RemoveAt(0);

            Assert.Equal(1, target.Panel.Children.Count);
            Assert.Equal("bar", ((TextBlock)target.Panel.Children[0]).Text);
            Assert.Equal("bar", ((TextBlock)target.ItemContainerGenerator.ContainerFromIndex(0)).Text);
        }

        [Fact]
        public void Clearing_Items_Should_Remove_Containers()
        {
            var items = new ObservableCollection<string> { "foo", "bar" };
            var target = new ItemsPresenter
            {
                Items = items,
            };

            target.ApplyTemplate();
            items.Clear();

            Assert.Empty(target.Panel.Children);
            Assert.Empty(target.ItemContainerGenerator.Containers);
        }

        [Fact]
        public void Replacing_Items_Should_Update_Containers()
        {
            var items = new ObservableCollection<string> { "foo", "bar", "baz" };
            var target = new ItemsPresenter
            {
                Items = items,
            };

            target.ApplyTemplate();
            items[1] = "baz";

            var text = target.Panel.Children
                .OfType<TextBlock>()
                .Select(x => x.Text)
                .ToList();

            Assert.Equal(new[] { "foo", "baz", "baz" }, text);
        }

        [Fact]
        public void Moving_Items_Should_Update_Containers()
        {
            var items = new ObservableCollection<string> { "foo", "bar", "baz" };
            var target = new ItemsPresenter
            {
                Items = items,
            };

            target.ApplyTemplate();
            items.Move(2, 1);

            var text = target.Panel.Children
                .OfType<TextBlock>()
                .Select(x => x.Text)
                .ToList();

            Assert.Equal(new[] { "foo", "baz", "bar" }, text);
        }

        [Fact]
        public void Setting_Items_To_Null_Should_Remove_Containers()
        {
            var target = new ItemsPresenter
            {
                Items = new[] { "foo", "bar" },
            };

            target.ApplyTemplate();
            target.Items = null;

            Assert.Empty(target.Panel.Children);
            Assert.Empty(target.ItemContainerGenerator.Containers);
        }

        [Fact]
        public void Should_Handle_Null_Items()
        {
            var items = new PerspexList<string>(new[] { "foo", null, "bar" });

            var target = new ItemsPresenter
            {
                Items = items,
            };

            target.ApplyTemplate();

            var text = target.Panel.Children.Cast<TextBlock>().Select(x => x.Text).ToList();

            Assert.Equal(new[] { "foo", "bar" }, text);
            Assert.NotNull(target.ItemContainerGenerator.ContainerFromIndex(0));
            Assert.Null(target.ItemContainerGenerator.ContainerFromIndex(1));
            Assert.NotNull(target.ItemContainerGenerator.ContainerFromIndex(2));

            items.RemoveAt(1);

            text = target.Panel.Children.Cast<TextBlock>().Select(x => x.Text).ToList();

            Assert.Equal(new[] { "foo", "bar" }, text);
            Assert.NotNull(target.ItemContainerGenerator.ContainerFromIndex(0));
            Assert.NotNull(target.ItemContainerGenerator.ContainerFromIndex(1));
        }

        [Fact]
        public void Should_Handle_Duplicate_Items()
        {
            var items = new PerspexList<int>(new[] { 1, 2, 1 });

            var target = new ItemsPresenter
            {
                Items = items,
            };

            target.ApplyTemplate();
            items.RemoveAt(2);

            var text = target.Panel.Children.OfType<TextBlock>().Select(x => x.Text);
            Assert.Equal(new[] { "1", "2" }, text);
        }

        [Fact]
        public void Panel_Should_Be_Created_From_ItemsPanel_Template()
        {
            var panel = new Panel();
            var target = new ItemsPresenter
            {
                ItemsPanel = new FuncTemplate<IPanel>(() => panel),
            };

            target.ApplyTemplate();

            Assert.Equal(panel, target.Panel);
        }

        [Fact]
        public void Panel_TabNavigation_Should_Be_Set_To_Once()
        {
            var target = new ItemsPresenter();

            target.ApplyTemplate();

            Assert.Equal(KeyboardNavigationMode.Once, KeyboardNavigation.GetTabNavigation((InputElement)target.Panel));
        }

        [Fact]
        public void Panel_TabNavigation_Should_Be_Set_To_ItemsPresenter_Value()
        {
            var target = new ItemsPresenter();

            KeyboardNavigation.SetTabNavigation(target, KeyboardNavigationMode.Cycle);
            target.ApplyTemplate();

            Assert.Equal(KeyboardNavigationMode.Cycle, KeyboardNavigation.GetTabNavigation((InputElement)target.Panel));
        }

        [Fact]
        public void Panel_Should_Be_Visual_Child()
        {
            var target = new ItemsPresenter();

            target.ApplyTemplate();

            var child = target.GetVisualChildren().Single();

            Assert.Equal(target.Panel, child);
        }

        [Fact]
        public void MemberSelector_Should_Select_Member()
        {
            var target = new ItemsPresenter
            {
                Items = new[] { new Item("Foo"), new Item("Bar") },
                MemberSelector = new FuncMemberSelector<Item, string>(x => x.Value),
            };

            target.ApplyTemplate();

            var text = target.Panel.Children
                .Cast<TextBlock>()
                .Select(x => x.Text)
                .ToList();

            Assert.Equal(new[] { "Foo", "Bar" }, text);
        }

        [Fact]
        public void MemberSelector_Should_Set_DataContext()
        {
            var items = new[] { new Item("Foo"), new Item("Bar") };
            var target = new ItemsPresenter
            {
                Items = items,
                MemberSelector = new FuncMemberSelector<Item, string>(x => x.Value),
            };

            target.ApplyTemplate();

            var dataContexts = target.Panel.Children
                .Cast<TextBlock>()
                .Select(x => x.DataContext)
                .ToList();

            Assert.Equal(new[] { "Foo", "Bar" }, dataContexts);
        }

        private class Item
        {
            public Item(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }

        private class TestItem : ContentControl
        {
        }

        private class TestItemsControl : ItemsControl
        {
            protected override IItemContainerGenerator CreateItemContainerGenerator()
            {
                return new ItemContainerGenerator<TestItem>(this, TestItem.ContentProperty);
            }
        }
    }
}
