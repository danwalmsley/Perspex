﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Collections.Specialized;
using System.Linq;
using Perspex.Collections;
using Perspex.Controls.Presenters;
using Perspex.Controls.Templates;
using Perspex.LogicalTree;
using Perspex.Styling;
using Perspex.VisualTree;
using Xunit;

namespace Perspex.Controls.UnitTests
{
    public class ItemsControlTests
    {
        [Fact]
        public void Panel_Should_Have_TemplatedParent_Set_To_ItemsControl()
        {
            var target = new ItemsControl();

            target.Template = GetTemplate();
            target.Items = new[] { "Foo" };
            target.ApplyTemplate();

            var presenter = target.GetTemplateChildren().OfType<ItemsPresenter>().Single();
            var panel = target.GetTemplateChildren().OfType<StackPanel>().Single();

            Assert.Equal(target, panel.TemplatedParent);
        }

        [Fact]
        public void Item_Should_Have_TemplatedParent_Set_To_Null()
        {
            var target = new ItemsControl();

            target.Template = GetTemplate();
            target.Items = new[] { "Foo" };
            target.ApplyTemplate();

            var presenter = target.GetTemplateChildren().OfType<ItemsPresenter>().Single();
            var panel = target.GetTemplateChildren().OfType<StackPanel>().Single();
            var item = (TextBlock)panel.GetVisualChildren().First();

            Assert.Null(item.TemplatedParent);
        }

        [Fact]
        public void Control_Item_Should_Have_Parent_Set()
        {
            var target = new ItemsControl();
            var child = new Control();

            target.Template = GetTemplate();
            target.Items = new[] { child };
            target.ApplyTemplate();

            Assert.Equal(target, child.Parent);
            Assert.Equal(target, ((ILogical)child).LogicalParent);
        }

        [Fact]
        public void Clearing_Control_Item_Should_Clear_Child_Controls_Parent()
        {
            var target = new ItemsControl();
            var child = new Control();

            target.Template = GetTemplate();
            target.Items = new[] { child };
            target.ApplyTemplate();
            target.Items = null;

            Assert.Null(child.Parent);
            Assert.Null(((ILogical)child).LogicalParent);
        }

        [Fact]
        public void Adding_Control_Item_Should_Make_Control_Appear_In_LogicalChildren()
        {
            var target = new ItemsControl();
            var child = new Control();

            target.Template = GetTemplate();
            target.Items = new[] { child };
            target.ApplyTemplate();

            Assert.Equal(new[] { child }, ((ILogical)target).LogicalChildren.ToList());
        }

        [Fact]
        public void Adding_String_Item_Should_Make_TextBlock_Appear_In_LogicalChildren()
        {
            var target = new ItemsControl();
            var child = new Control();

            target.Template = GetTemplate();
            target.Items = new[] { "Foo" };
            target.ApplyTemplate();

            var logical = (ILogical)target;
            Assert.Equal(1, logical.LogicalChildren.Count);
            Assert.IsType<TextBlock>(logical.LogicalChildren[0]);
        }

        [Fact]
        public void Setting_Items_To_Null_Should_Remove_LogicalChildren()
        {
            var target = new ItemsControl();
            var child = new Control();

            target.Template = GetTemplate();
            target.Items = new[] { "Foo" };
            target.ApplyTemplate();
            target.Items = null;

            Assert.Equal(new ILogical[0], ((ILogical)target).LogicalChildren.ToList());
        }

        [Fact]
        public void Setting_Items_Should_Fire_LogicalChildren_CollectionChanged()
        {
            var target = new ItemsControl();
            var child = new Control();
            var called = false;

            target.Template = GetTemplate();
            target.ApplyTemplate();

            ((ILogical)target).LogicalChildren.CollectionChanged += (s, e) =>
                called = e.Action == NotifyCollectionChangedAction.Add;

            target.Items = new[] { child };

            Assert.True(called);
        }

        [Fact]
        public void Setting_Items_To_Null_Should_Fire_LogicalChildren_CollectionChanged()
        {
            var target = new ItemsControl();
            var child = new Control();
            var called = false;

            target.Template = GetTemplate();
            target.Items = new[] { child };
            target.ApplyTemplate();

            ((ILogical)target).LogicalChildren.CollectionChanged += (s, e) =>
                called = e.Action == NotifyCollectionChangedAction.Remove;

            target.Items = null;

            Assert.True(called);
        }

        [Fact]
        public void Changing_Items_Should_Fire_LogicalChildren_CollectionChanged()
        {
            var target = new ItemsControl();
            var child = new Control();
            var called = false;

            target.Template = GetTemplate();
            target.Items = new[] { child };
            target.ApplyTemplate();

            ((ILogical)target).LogicalChildren.CollectionChanged += (s, e) => called = true;

            target.Items = new[] { "Foo" };

            Assert.True(called);
        }

        [Fact]
        public void Adding_Items_Should_Fire_LogicalChildren_CollectionChanged()
        {
            var target = new ItemsControl();
            var items = new PerspexList<string> { "Foo" };
            var called = false;

            target.Template = GetTemplate();
            target.Items = items;
            target.ApplyTemplate();

            ((ILogical)target).LogicalChildren.CollectionChanged += (s, e) =>
                called = e.Action == NotifyCollectionChangedAction.Add;

            items.Add("Bar");

            Assert.True(called);
        }

        [Fact]
        public void Removing_Items_Should_Fire_LogicalChildren_CollectionChanged()
        {
            var target = new ItemsControl();
            var items = new PerspexList<string> { "Foo", "Bar" };
            var called = false;

            target.Template = GetTemplate();
            target.Items = items;
            target.ApplyTemplate();

            ((ILogical)target).LogicalChildren.CollectionChanged += (s, e) =>
                called = e.Action == NotifyCollectionChangedAction.Remove;

            items.Remove("Bar");

            Assert.True(called);
        }

        [Fact]
        public void LogicalChildren_Should_Not_Change_Instance_When_Template_Changed()
        {
            var target = new ItemsControl()
            {
                Template = GetTemplate(),
            };

            var before = ((ILogical)target).LogicalChildren;

            target.Template = null;
            target.Template = GetTemplate();

            var after = ((ILogical)target).LogicalChildren;

            Assert.NotNull(before);
            Assert.NotNull(after);
            Assert.Same(before, after);
        }

        [Fact]
        public void Empty_Class_Should_Initially_Be_Applied()
        {
            var target = new ItemsControl()
            {
                Template = GetTemplate(),
            };

            Assert.True(target.Classes.Contains(":empty"));
        }

        [Fact]
        public void Empty_Class_Should_Be_Cleared_When_Items_Added()
        {
            var target = new ItemsControl()
            {
                Template = GetTemplate(),
                Items = new[] { 1, 2, 3 },
            };

            Assert.False(target.Classes.Contains(":empty"));
        }

        [Fact]
        public void Empty_Class_Should_Be_Set_When_Empty_Collection_Set()
        {
            var target = new ItemsControl()
            {
                Template = GetTemplate(),
                Items = new[] { 1, 2, 3 },
            };

            target.Items = new int[0];

            Assert.True(target.Classes.Contains(":empty"));
        }

        [Fact]
        public void Setting_Presenter_Explicitly_Should_Set_Item_Parent()
        {
            var target = new TestItemsControl();
            var child = new Control();

            var presenter = new ItemsPresenter
            {
                TemplatedParent = target,
                [~ItemsPresenter.ItemsProperty] = target[~ItemsControl.ItemsProperty],
            };

            presenter.ApplyTemplate();
            target.Presenter = presenter;
            target.Items = new[] { child };
            target.ApplyTemplate();

            Assert.Equal(target, child.Parent);
            Assert.Equal(target, ((ILogical)child).LogicalParent);
        }

        [Fact]
        public void DataContexts_Should_Be_Correctly_Set()
        {
            var items = new object[]
            {
                "Foo",
                new Item("Bar"),
                new TextBlock { Text = "Baz" },
                new ListBoxItem { Content = "Qux" },
            };

            var target = new ItemsControl
            {
                Template = GetTemplate(),
                DataContext = "Base",
                DataTemplates = new DataTemplates
                {
                    new FuncDataTemplate<Item>(x => new Button { Content = x })
                },
                Items = items,
            };

            target.ApplyTemplate();

            var dataContexts = target.Presenter.Panel.Children
                .Cast<Control>()
                .Select(x => x.DataContext)
                .ToList();

            Assert.Equal(
                new object[] { items[0], items[1], "Base", "Base" },
                dataContexts);
        }

        [Fact]
        public void MemberSelector_Should_Select_Member()
        {
            var target = new ItemsControl
            {
                Template = GetTemplate(),
                Items = new[] { new Item("Foo"), new Item("Bar") },
                MemberSelector = new FuncMemberSelector<Item, string>(x => x.Value),
            };

            target.ApplyTemplate();

            var text = target.Presenter.Panel.Children
                .Cast<TextBlock>()
                .Select(x => x.Text)
                .ToList();

            Assert.Equal(new[] { "Foo", "Bar" }, text);
        }

        private class Item
        {
            public Item(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }

        private ControlTemplate GetTemplate()
        {
            return new ControlTemplate<ItemsControl>(parent =>
            {
                return new Border
                {
                    Background = new Media.SolidColorBrush(0xffffffff),
                    Child = new ItemsPresenter
                    {
                        Name = "itemsPresenter",
                        MemberSelector = parent.MemberSelector,
                        [~ItemsPresenter.ItemsProperty] = parent[~ItemsControl.ItemsProperty],
                    }
                };
            });
        }

        private class TestItemsControl : ItemsControl
        {
            public new IItemsPresenter Presenter
            {
                get { return base.Presenter; }
                set { base.Presenter = value; }
            }
        }
    }
}
