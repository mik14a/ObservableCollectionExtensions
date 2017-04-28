/*
 * Copyright © 2017 mik14a <mik14a@gmail.com>
 * This work is free. You can redistribute it and/or modify it under the
 * terms of the Do What The Fuck You Want To Public License, Version 2,
 * as published by Sam Hocevar. See the COPYING file for more details.
 */

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.ObjectModel
{
    [TestClass()]
    public class ObservableCollectionExtensionsTests
    {
        [TestMethod]
        public void AddTest() {
            var collection = new ObservableCollection<string>();
            var list = new List<string>() { "a", "b", "c" };
            var propertyChanged = 0;
            ((INotifyPropertyChanged)collection).PropertyChanged += (s, e) => {
                propertyChanged++;
                Console.WriteLine($"PropertyChanged: {e.PropertyName}.");
            };
            var collectionChanged = 0;
            ((INotifyCollectionChanged)collection).CollectionChanged += (s, e) => {
                collectionChanged++;
                Console.WriteLine($"CollectionChanged: {e.Action}, {e.NewItems.Count}.");
            };
            foreach (var item in list) {
                collection.Add(item);
            }
            Assert.AreEqual(propertyChanged, list.Count * 2);
            Assert.AreEqual(collectionChanged, list.Count);
            CollectionAssert.AreEqual(collection, list);
        }

        [TestMethod]
        public void AddRangeTest() {
            var collection = new ObservableCollection<string>();
            var list = new List<string>() { "a", "b", "c" };
            var propertyChanged = 0;
            ((INotifyPropertyChanged)collection).PropertyChanged += (s, e) => {
                propertyChanged++;
                Console.WriteLine($"PropertyChanged: {e.PropertyName}.");
            };
            var collectionChanged = 0;
            ((INotifyCollectionChanged)collection).CollectionChanged += (s, e) => {
                collectionChanged++;
                Console.WriteLine($"CollectionChanged: {e.Action}, {e.NewItems.Count}.");
            };
            collection.AddRange(list);
            Assert.AreEqual(propertyChanged, 2);
            Assert.AreEqual(collectionChanged, 1);
            CollectionAssert.AreEqual(collection, list);
        }

        [TestMethod]
        public void AddOperationTest() {
            var random = new Random(0);
            var collection = new ObservableCollection<int>();
            ((INotifyPropertyChanged)collection).PropertyChanged += (s, e) => { };
            ((INotifyCollectionChanged)collection).CollectionChanged += (s, e) => { };
            var list = Enumerable.Range(0, 1000000)
                .Select(_ => random.Next())
                .ToList();
            var stopWatch = new Stopwatch();

            Console.WriteLine($"Add");
            for (int i = 0; i < 5; ++i) {
                collection.Clear();
                stopWatch.Restart();
                foreach (var item in list) {
                    collection.Add(item);
                }
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedTicks} ticks.");
                CollectionAssert.AreEqual(collection, list);
            }

            Console.WriteLine($"AddRange");
            for (int i = 0; i < 5; ++i) {
                collection.Clear();
                stopWatch.Restart();
                collection.AddRange(list);
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedTicks} ticks.");
                CollectionAssert.AreEqual(collection, list);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddRangeTestWithNullObservableCollection() {
            var collection = (ObservableCollection<int>)null;
            var list = new List<int>() { 1, 2, 3, 4, 5 };
            collection.AddRange(list);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddRangeTestWithNullCollection() {
            var collection = new ObservableCollection<int>();
            var list = (List<int>)null;
            collection.AddRange(list);
        }

        [TestMethod]
        public void AddRangeTestWithEmptyCollection() {
            var collection = new ObservableCollection<string>();
            var list = new List<string>();
            var propertyChanged = 0;
            ((INotifyPropertyChanged)collection).PropertyChanged += (s, e) => {
                propertyChanged++;
                Console.WriteLine($"PropertyChanged: {e.PropertyName}.");
            };
            var collectionChanged = 0;
            ((INotifyCollectionChanged)collection).CollectionChanged += (s, e) => {
                collectionChanged++;
                Console.WriteLine($"CollectionChanged: {e.Action}, {e.NewItems.Count}.");
            };
            collection.AddRange(list);
            Assert.AreEqual(propertyChanged, 0);
            Assert.AreEqual(collectionChanged, 0);
            CollectionAssert.AreEqual(collection, list);
        }

    }
}
