/*
 * Copyright © 2017 mik14a <mik14a@gmail.com>
 * This work is free. You can redistribute it and/or modify it under the
 * terms of the Do What The Fuck You Want To Public License, Version 2,
 * as published by Sam Hocevar. See the COPYING file for more details.
 */

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// Extensions for ObservableCollection{T}.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Bulk add a range of collection to an ObservableCollection{T}.
        /// </summary>
        /// <typeparam name="T">The type of elements in the observable collection.</typeparam>
        /// <param name="self">The ObservableCollection{T} to add collection to.</param>
        /// <param name="collection">The collection to add to the ObservableCollection{T}.</param>
        public static void AddRange<T>(this ObservableCollection<T> self, IReadOnlyList<T> collection) {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (collection.Count == 0) return;

            ObservableCollectionInvoker<T>.CheckReentrancy(self);
            var items = (List<T>)ObservableCollectionInvoker<T>.GetItems(self);
            items.AddRange(collection);
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)collection);
            ObservableCollectionInvoker<T>.OnPropertyChanged(self, "Count");
            ObservableCollectionInvoker<T>.OnPropertyChanged(self, "Item[]");
            ObservableCollectionInvoker<T>.OnCollectionChanged(self, e);
        }

        static class ObservableCollectionInvoker<T>
        {
            public static void CheckReentrancy(ObservableCollection<T> source) {
                checkReentrancy(source);
            }

            public static IList<T> GetItems(ObservableCollection<T> source) {
                return getItems(source);
            }

            public static void OnPropertyChanged(ObservableCollection<T> source, string propertyName) {
                onPropertyChanged(source, propertyName);
            }

            public static void OnCollectionChanged(ObservableCollection<T> source, NotifyCollectionChangedEventArgs e) {
                onCollectionChanged(source, e);
            }

            static ObservableCollectionInvoker() {
                checkReentrancy = CreateCheckReentrancy();
                getItems = CreateGetItems();
                onPropertyChanged = CreateOnPropertyChanged();
                onCollectionChanged = CreateOnCollectionChanged();
            }

            static Action<ObservableCollection<T>> CreateCheckReentrancy() {
                const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var type = typeof(ObservableCollection<T>);
                var method = type.GetMethod("CheckReentrancy", bindingFlags);
                var instance = Expression.Parameter(type);
                var call = Expression.Call(instance, method);
                return Expression.Lambda<Action<ObservableCollection<T>>>(call, instance).Compile();
            }

            static Func<ObservableCollection<T>, IList<T>> CreateGetItems() {
                const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var type = typeof(ObservableCollection<T>);
                var property = type.GetProperty("Items", bindingFlags);
                var method = property.GetGetMethod(true);
                var instance = Expression.Parameter(property.DeclaringType);
                var call = Expression.Call(instance, method);
                return Expression.Lambda<Func<ObservableCollection<T>, IList<T>>>(call, instance).Compile();
            }

            static Action<ObservableCollection<T>, string> CreateOnPropertyChanged() {
                const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var type = typeof(ObservableCollection<T>);
                var method = type.GetMethod("OnPropertyChanged", bindingFlags, null, new Type[] { typeof(string) }, null);
                var instance = Expression.Parameter(type);
                var parameter = Expression.Parameter(typeof(string));
                var call = Expression.Call(instance, method, parameter);
                return Expression.Lambda<Action<ObservableCollection<T>, string>>(call, instance, parameter).Compile();
            }

            static Action<ObservableCollection<T>, NotifyCollectionChangedEventArgs> CreateOnCollectionChanged() {
                const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var type = typeof(ObservableCollection<T>);
                var method = type.GetMethod("OnCollectionChanged", bindingFlags, null, new Type[] { typeof(NotifyCollectionChangedEventArgs) }, null);
                var instance = Expression.Parameter(type);
                var parameter = Expression.Parameter(typeof(NotifyCollectionChangedEventArgs));
                var call = Expression.Call(instance, method, parameter);
                return Expression.Lambda<Action<ObservableCollection<T>, NotifyCollectionChangedEventArgs>>(call, instance, parameter).Compile();
            }

            static Action<ObservableCollection<T>> checkReentrancy;
            static Func<ObservableCollection<T>, IList<T>> getItems;
            static Action<ObservableCollection<T>, string> onPropertyChanged;
            static Action<ObservableCollection<T>, NotifyCollectionChangedEventArgs> onCollectionChanged;
        }
    }
}
