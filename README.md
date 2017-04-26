ObservableCollectionExtensions
==============================

Add extension method to ObservableCollection for bulk add a range of collection.

## Usage

Source code.
```cs
var collection = new ObservableCollection<string>();
var list = new List<string>() { "a", "b", "c" };
((INotifyPropertyChanged)collection).PropertyChanged += (s, e) => {
    Console.WriteLine($"PropertyChanged: {e.PropertyName}.");
};
((INotifyCollectionChanged)collection).CollectionChanged += (s, e) => {
    Console.WriteLine($"CollectionChanged: {e.Action}, {e.NewItems.Count}.");
};
collection.AddRange(list);
```

Console.
```
PropertyChanged: Count.
PropertyChanged: Item[].
CollectionChanged: Add, 3.
```

## Licence

[WTFPL](http://www.wtfpl.net/)

## Author

[mik14a](https://github.com/mik14a)
