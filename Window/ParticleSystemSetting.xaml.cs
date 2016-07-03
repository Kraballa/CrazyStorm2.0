﻿/*
 * The MIT License (MIT)
 * Copyright (c) StarX 2015 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CrazyStorm.Core;

namespace CrazyStorm
{
    public partial class ParticleSystemSetting : Window
    {
        #region Private Members
        File file;
        ParticleSystem selectedParticle;
        ParticleType selectedType;
        TabItem selectedTab;
        #endregion

        #region Constructor
        public ParticleSystemSetting(File file, ParticleSystem particleSystem, TabItem selectedTab)
        {
            this.file = file;
            this.selectedParticle = particleSystem;
            this.selectedTab = selectedTab;
            InitializeComponent();
            InitializeDataBinding();
            UpdatePreview();
        }
        #endregion

        #region Private Methods
        void InitializeDataBinding()
        {
            ParticleSystemName.DataContext = selectedParticle;
            TypeList.ItemsSource = selectedParticle.CustomTypes;
            file.UpdateResource();
            //Load images.
            foreach (FileResource item in file.Images)
            {
                if (item.IsValid)
                    ImageCombo.Items.Add(item);
            }
        }
        void UpdateColor()
        {
            for (int i = 0; i < ColorPanel.Children.Count;++i )
            {
                var item = ColorPanel.Children[i] as Label;
                if (selectedType != null && (int)selectedType.Color == i)
                    item.BorderThickness = new Thickness(1);
                else
                    item.BorderThickness = new Thickness(0);
            }
        }
        void UpdatePreview()
        {
            var type = Setting.DataContext as ParticleType;
            if (type == null)
                return;

            int frame;
            if (!int.TryParse(Frames.Text, out frame))
                return;

            for (int i = 0; i < Preview.Children.Count;++i)
            {
                if (((FrameworkElement)Preview.Children[i]).Name == "FrameRect")
                {
                    Preview.Children.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 1;i < frame;++i)
            {
                var rect = new Rectangle();
                rect.Name = "FrameRect";
                rect.Width = type.Width;
                rect.Height = type.Height;
                rect.Stroke = new SolidColorBrush(Colors.Red);
                Preview.Children.Add(rect);
                Canvas.SetLeft(rect, type.StartPointX + i * type.Width);
                Canvas.SetTop(rect, type.StartPointY);
                rect.Opacity = 0.8f;
            }
        }
        #endregion

        #region Window EventHandlers
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ParticleSystemName.Text))
            {
                MessageBox.Show((string)FindResource("ParticleSystemNameCanNotBeEmptyStr"), (string)FindResource("TipTitleStr"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            selectedParticle.Name = ParticleSystemName.Text;
            selectedTab.Header = ParticleSystemName.Text;
            MessageBox.Show((string)FindResource("ParticleSystemNameSuccessfullyChangedStr"), (string)FindResource("TipTitleStr"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void TypeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                selectedType = e.AddedItems[0] as ParticleType;
                Setting.IsEnabled = true;
                Setting.DataContext = selectedType;
            }
            else
            {
                selectedType = null;
                Setting.IsEnabled = false;
                Setting.DataContext = null;
            }
            UpdateColor();
        }
        private void AddNewType_Click(object sender, RoutedEventArgs e)
        {
            selectedParticle.CustomTypes.Add(new ParticleType(selectedParticle.CustomTypeIndex));
        }
        private void ColorPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var selectedColor = e.Source as Label;
            for (int i = 0; i < ColorPanel.Children.Count; ++i)
            {
                var item = ColorPanel.Children[i] as Label;
                if (selectedColor == item)
                {
                    selectedType.Color = (ParticleColor)i;
                    UpdateColor();
                }
            }
        }
        private void DeleteType_Click(object sender, RoutedEventArgs e)
        {
            selectedParticle.CustomTypes.Remove(selectedType);
        }
        private void Frames_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }
        #endregion
    }
}