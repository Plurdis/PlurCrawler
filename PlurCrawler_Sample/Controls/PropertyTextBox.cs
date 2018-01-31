﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFExtension;

namespace PlurCrawler_Sample.Controls
{
    class PropertyTextBox : TextBox
    {
        public PropertyTextBox()
        {
            this.Style = (Style)FindResource("PropertyTextBoxStyle");
        }

        public static DependencyProperty PropertyNameProperty = DependencyHelper.Register();
        public static DependencyProperty DescriptionProperty = DependencyHelper.Register();
        public static DependencyProperty AccentNameProperty = DependencyHelper.Register(new PropertyMetadata(true));

        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public bool AccentName
        {
            get => (bool)GetValue(AccentNameProperty);
            set => SetValue(AccentNameProperty, value);
        }
    }
}
