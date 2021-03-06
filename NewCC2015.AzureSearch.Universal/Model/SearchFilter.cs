﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GabrhelDigital.Universal.Core.Binding;

namespace NewCC2015.AzureSearch.Universal.Model
{
    public class SearchFilter : BindableBase
    {
        public string Label { get; set; }
        
        public string Value { get; set; }

        public SearchFilter(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }
}
