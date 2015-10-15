﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Perspex.Collections;
using Xunit;

namespace Perspex.Base.UnitTests.Collections
{
    public class PerspexListTests
    {
        [Fact]
        public void Items_Passed_To_Constructor_Should_Appear_In_List()
        {
            var items = new[] { 1, 2, 3 };
            var target = new PerspexList<int>(items);

            Assert.Equal(items, target);
        }

        [Fact]
        public void AddRange_With_Null_Should_Throw_Exception()
        {
            var target = new PerspexList<int>();

            Assert.Throws<ArgumentNullException>(() => target.AddRange(null));
        }

        [Fact]
        public void RemoveAll_With_Null_Should_Throw_Exception()
        {
            var target = new PerspexList<int>();

            Assert.Throws<ArgumentNullException>(() => target.RemoveAll(null));
        }

        [Fact]
        public void InsertRange_With_Null_Should_Throw_Exception()
        {
            var target = new PerspexList<int>();

            Assert.Throws<ArgumentNullException>(() => target.InsertRange(1, null));
        }

        [Fact]
        public void InsertRange_Past_End_Should_Throw_Exception()
        {
            var target = new PerspexList<int>();

            Assert.Throws<ArgumentOutOfRangeException>(() => target.InsertRange(1, new List<int>() { 1 }));
        }

        [Fact]
        public void Adding_Item_Should_Raise_CollectionChanged()
        {
            var target = new PerspexList<int>(new[] { 1, 2 });
            var raised = false;

            target.CollectionChanged += (s, e) =>
            {
                Assert.Equal(target, s);
                Assert.Equal(NotifyCollectionChangedAction.Add, e.Action);
                Assert.Equal(new[] { 3 }, e.NewItems.Cast<int>());
                Assert.Equal(2, e.NewStartingIndex);

                raised = true;
            };

            target.Add(3);

            Assert.True(raised);
        }

        [Fact]
        public void Adding_Items_Should_Raise_CollectionChanged()
        {
            var target = new PerspexList<int>(new[] { 1, 2 });
            var raised = false;

            target.CollectionChanged += (s, e) =>
            {
                Assert.Equal(target, s);
                Assert.Equal(NotifyCollectionChangedAction.Add, e.Action);
                Assert.Equal(new[] { 3, 4 }, e.NewItems.Cast<int>());
                Assert.Equal(2, e.NewStartingIndex);

                raised = true;
            };

            target.AddRange(new[] { 3, 4 });

            Assert.True(raised);
        }

        [Fact]
        public void Inserting_Item_Should_Raise_CollectionChanged()
        {
            var target = new PerspexList<int>(new[] { 1, 2 });
            var raised = false;

            target.CollectionChanged += (s, e) =>
            {
                Assert.Equal(target, s);
                Assert.Equal(NotifyCollectionChangedAction.Add, e.Action);
                Assert.Equal(new[] { 3 }, e.NewItems.Cast<int>());
                Assert.Equal(1, e.NewStartingIndex);

                raised = true;
            };

            target.Insert(1, 3);

            Assert.True(raised);
        }

        [Fact]
        public void Inserting_Items_Should_Raise_CollectionChanged()
        {
            var target = new PerspexList<int>(new[] { 1, 2 });
            var raised = false;

            target.CollectionChanged += (s, e) =>
            {
                Assert.Equal(target, s);
                Assert.Equal(NotifyCollectionChangedAction.Add, e.Action);
                Assert.Equal(new[] { 3, 4 }, e.NewItems.Cast<int>());
                Assert.Equal(1, e.NewStartingIndex);

                raised = true;
            };

            target.InsertRange(1, new[] { 3, 4 });

            Assert.True(raised);
        }

        [Fact]
        public void Removing_Item_Should_Raise_CollectionChanged()
        {
            var target = new PerspexList<int>(new[] { 1, 2, 3 });
            var raised = false;

            target.CollectionChanged += (s, e) =>
            {
                Assert.Equal(target, s);
                Assert.Equal(NotifyCollectionChangedAction.Remove, e.Action);
                Assert.Equal(new[] { 3 }, e.OldItems.Cast<int>());
                Assert.Equal(2, e.OldStartingIndex);

                raised = true;
            };

            target.Remove(3);

            Assert.True(raised);
        }

        [Fact]
        public void Clearing_Items_Should_Raise_CollectionChanged_Reset()
        {
            var target = new PerspexList<int>(new[] { 1, 2, 3 });
            var raised = false;

            target.CollectionChanged += (s, e) =>
            {
                Assert.Equal(target, s);
                Assert.Equal(NotifyCollectionChangedAction.Reset, e.Action);

                raised = true;
            };

            target.Clear();

            Assert.True(raised);
        }

        [Fact]
        public void Clearing_Items_Should_Raise_CollectionChanged_Remove()
        {
            var target = new PerspexList<int>(new[] { 1, 2, 3 });
            var raised = false;

            target.ResetBehavior = ResetBehavior.Remove;
            target.CollectionChanged += (s, e) =>
            {
                Assert.Equal(target, s);
                Assert.Equal(NotifyCollectionChangedAction.Remove, e.Action);
                Assert.Equal(new[] { 1, 2, 3 }, e.OldItems.Cast<int>());
                Assert.Equal(0, e.OldStartingIndex);

                raised = true;
            };

            target.Clear();

            Assert.True(raised);
        }
    }
}
