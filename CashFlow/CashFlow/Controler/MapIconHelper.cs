using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using CashFlow.GameLogic;

namespace CashFlow.Controler
{
    public static class MapElementData
    {
        #region Attached Dependency Property ObjectData
        public static readonly DependencyProperty ObjectDataProperty =
             DependencyProperty.RegisterAttached("ObjectData",
             typeof(Building),
             typeof(MapElementData),
             new PropertyMetadata(default(object), null));

        public static object GetObjectData(MapElement obj)
        {
            try
            {
                return obj.GetValue(ObjectDataProperty);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
           
        }

        public static void SetObjectData(
           DependencyObject obj,
           object value)
        {
            obj.SetValue(ObjectDataProperty, value);
        }
        #endregion

        public static void AddData(this MapElement element, object data)
        {
            SetObjectData(element, data);
        }

        public static Building ReadData(this MapElement element)
        {
            return (Building) GetObjectData(element);
        }
    }
}
