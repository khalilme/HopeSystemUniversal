﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace HopeUWP.Extensions
{
    public class InputTypeTrigger : StateTriggerBase
    {
        private FrameworkElement _targetElement;
        private PointerDeviceType _lastPointerType, _triggerPointerType;

        public FrameworkElement TargetElement
        {
            get
            {
                return _targetElement;
            }
            set
            {
                _targetElement = value;
                _targetElement.AddHandler(FrameworkElement.PointerPressedEvent, new PointerEventHandler(_targetElement_PointerPressed), true);
            }
        }

    
        public PointerDeviceType PointerType
        {
            get
            {
                return _triggerPointerType;
            }
            set
            {
                _triggerPointerType = value;
            }
        }

        private void _targetElement_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _lastPointerType = e.Pointer.PointerDeviceType;
            SetActive(_triggerPointerType == _lastPointerType);
        }



    }
}
