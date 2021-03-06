﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Moq;
using Perspex.Controls;
using Perspex.Markup.Xaml.Binding;
using Xunit;

namespace Perspex.Markup.Xaml.UnitTests.Binding
{
    public class XamlBindingTests
    {
        [Fact]
        public void OneWay_Binding_Should_Be_Set_Up()
        {
            var target = CreateTarget();
            var binding = new XamlBinding
            {
                SourcePropertyPath = "Foo",
                BindingMode = BindingMode.OneWay,
            };

            binding.Bind(target.Object, TextBox.TextProperty);

            target.Verify(x => x.Bind(
                TextBox.TextProperty, 
                It.IsAny<IObservable<object>>(), 
                BindingPriority.LocalValue));
        }

        [Fact]
        public void TwoWay_Binding_Should_Be_Set_Up()
        {
            var target = CreateTarget();
            var binding = new XamlBinding
            {
                SourcePropertyPath = "Foo",
                BindingMode = BindingMode.TwoWay,
            };

            binding.Bind(target.Object, TextBox.TextProperty);

            target.Verify(x => x.BindTwoWay(
                TextBox.TextProperty,
                It.IsAny<ISubject<object>>(),
                BindingPriority.LocalValue));
        }

        [Fact]
        public void OneTime_Binding_Should_Be_Set_Up()
        {
            var dataContext = new BehaviorSubject<object>(null);
            var expression = new BehaviorSubject<object>(null);
            var target = CreateTarget(dataContext: dataContext);
            var binding = new XamlBinding
            {
                SourcePropertyPath = "Foo",
                BindingMode = BindingMode.OneTime,
            };

            binding.Bind(target.Object, TextBox.TextProperty, expression);

            target.Verify(x => x.SetValue(
                (PerspexProperty)TextBox.TextProperty, 
                null, 
                BindingPriority.LocalValue));
            target.ResetCalls();

            expression.OnNext("foo");
            dataContext.OnNext(1);

            target.Verify(x => x.SetValue(
                (PerspexProperty)TextBox.TextProperty,
                "foo",
                BindingPriority.LocalValue));
        }

        [Fact]
        public void OneWayToSource_Binding_Should_Be_Set_Up()
        {
            var textObservable = new Mock<IObservable<string>>();
            var expression = new Mock<ISubject<object>>();
            var target = CreateTarget(text: textObservable.Object);
            var binding = new XamlBinding
            {
                SourcePropertyPath = "Foo",
                BindingMode = BindingMode.OneWayToSource,
            };

            binding.Bind(target.Object, TextBox.TextProperty, expression.Object);

            textObservable.Verify(x => x.Subscribe(expression.Object));
        }

        [Fact]
        public void Default_BindingMode_Should_Be_Used()
        {
            var target = CreateTarget(null);
            var binding = new XamlBinding
            {
                SourcePropertyPath = "Foo",
            };

            binding.Bind(target.Object, TextBox.TextProperty);

            // Default for TextBox.Text is two-way.
            target.Verify(x => x.BindTwoWay(
                TextBox.TextProperty,
                It.IsAny<ISubject<object>>(),
                BindingPriority.LocalValue));
        }

        [Fact]
        public void DataContext_Binding_Should_Use_Parent_DataContext()
        {
            var parentDataContext = Mock.Of<IHeadered>(x => x.Header == (object)"Foo");

            var parent = new Decorator
            {
                Child = new Control(),
                DataContext = parentDataContext,
            };

            var binding = new XamlBinding
            {
                SourcePropertyPath = "Header",
            };

            binding.Bind(parent.Child, Control.DataContextProperty);

            Assert.Equal("Foo", parent.Child.DataContext);

            parentDataContext = Mock.Of<IHeadered>(x => x.Header == (object)"Bar");
            parent.DataContext = parentDataContext;
            Assert.Equal("Bar", parent.Child.DataContext);
        }

        private Mock<IObservablePropertyBag> CreateTarget(object dataContext)
        {
            return CreateTarget(dataContext: Observable.Never<object>().StartWith(dataContext));
        }

        private Mock<IObservablePropertyBag> CreateTarget(
            IObservable<object> dataContext = null,
            IObservable<string> text = null)
        {
            var result = new Mock<IObservablePropertyBag>();

            dataContext = dataContext ?? Observable.Never<object>().StartWith((object)null);
            text = text ?? Observable.Never<string>().StartWith((string)null);

            result.Setup(x => x.GetObservable(Control.DataContextProperty)).Returns(dataContext);
            result.Setup(x => x.GetObservable((PerspexProperty)Control.DataContextProperty)).Returns(dataContext);
            result.Setup(x => x.GetObservable((PerspexProperty)TextBox.TextProperty)).Returns(text);
            return result;
        }
    }
}
