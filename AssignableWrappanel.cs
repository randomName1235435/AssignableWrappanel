using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssignableWrapPanel
    {
      public class AssignableWrapPanel : Panel
      {
          public static readonly DependencyProperty MaxItemPerColumnProperty = DependencyProperty.Register(
              "MaxItemPerColumnProperty", typeof(int), typeof(AssignableWrapPanel), new PropertyMetadata(int.MaxValue));

          public int MaxItemPerColumn
          {
              get { return (int)GetValue(MaxItemPerColumnProperty); }
              set { SetValue(MaxItemPerColumnProperty, value); }
          }

          public Size GetMeasuredSize()
          {
              return MeasureOverride(new Size(double.MaxValue, double.MaxValue));
          }
          protected override Size MeasureOverride(Size availableSize)
          {
              if(InternalChildren.Count == 0)
                  return new Size(0,0);
              var width = 0.0;
              var height = 0.0;

              foreach (UIElement child in InternalChildren)
              {
                  child.Measure(availableSize);
              }
              if (InternalChildren.Count <= MaxItemPerColumn)
              {
                  height = InternalChildren.Cast<UIElement>().Sum(child => child.DesiredSize.Height);
                  height = height > availableSize.Height ? availableSize.Height : height;
              }
              else
              {
                  int itemCountColumn = 0;
                  var heightColumn = 0.0;
                  foreach (UIElement child in InternalChildren)
                  {
                      itemCountColumn++;
                      heightColumn += child.DesiredSize.Height;
                      if (itemCountColumn % MaxItemPerColumn == 0)
                      {
                          height = height > heightColumn ? height : heightColumn;
                          heightColumn = 0;
                      }
                  }
                  height = height > availableSize.Height ? availableSize.Height : height;
              }
              var maxWidth = InternalChildren.Cast<UIElement>().Max(child => child.DesiredSize.Width);
              var columnCount = (Math.Ceiling(((decimal)InternalChildren.Count / (decimal)MaxItemPerColumn)));
              width = maxWidth * (double)columnCount;
              width = width > availableSize.Width ? availableSize.Width : width;

              return new Size(width, height);
          }
          public Size ArrangeItems(Size finalSize)
          {
              return ArrangeOverride(finalSize);
          }
          protected override Size ArrangeOverride(Size finalSize)
          {
              if (InternalChildren.Count == 0)
                  return new Size(0, 0);

              var columnWidth = InternalChildren.Cast<UIElement>().Max(child => child.DesiredSize.Width);
              var itemHeight = InternalChildren.Cast<UIElement>().Max(child => child.DesiredSize.Height);
              int currentColumn = 0;
              int itemCount = 0;
              var maxItemPerColumn = MaxItemPerColumn;
              if (MaxItemPerColumn * itemHeight > finalSize.Height)
                  maxItemPerColumn = (int)(finalSize.Height / itemHeight);
              foreach (UIElement child in InternalChildren)
              {
                  var x = currentColumn * columnWidth;
                  var y = itemCount * itemHeight;
                  child.Arrange(new Rect(x, y, columnWidth, itemHeight));

                  itemCount++;
                  if (itemCount == maxItemPerColumn)
                  {
                      currentColumn++;
                      itemCount = 0;
                  }
              }
              return finalSize;
          }
    }
}
