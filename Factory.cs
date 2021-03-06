﻿using System.Reflection;
using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

namespace LiveSplit.OriAndTheBlindForest
{
    public class Factory : IComponentFactory
    {
        public string ComponentName {
            get { return "Ori and the Blind Forest Autosplitter v" + this.Version.ToString(); }
        }

        public string Description {
            get { return "Autosplitter for Ori and the Blind Forest"; }
        }

        public ComponentCategory Category {
            get { return ComponentCategory.Control; }
        }

        public IComponent Create(LiveSplitState state) {
            return new OriComponent();
        }

        public string UpdateName {
            get { return this.ComponentName; }
        }

        public string UpdateURL {
            get { return "https://raw.githubusercontent.com/AdamPrimer/LiveSplit.OriAndTheBlindForest/master/"; }
        }

        public Version Version {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public string XMLURL {
            get { return this.UpdateURL + "Components/LiveSplit.OriAndTheBlindForest.Updates.xml"; }
        }
    }
}
