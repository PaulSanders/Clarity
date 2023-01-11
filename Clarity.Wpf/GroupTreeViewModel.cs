// ****************************************************************************
// <copyright>
// Copyright © Paul Sanders 2014
// </copyright>
// ****************************************************************************
// <author>Paul Sanders</author>
// <project>Clarity</project>
// <web>http://clarity.codeplex.com</web>
// <license>
// See license.txt in this solution
// </license>
// ****************************************************************************
// Based on original code done by Josh Smith
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clarity.Wpf
{
    public class GroupTreeViewModel : TreeViewItemViewModel
    {
        public GroupTreeViewModel(string name)
            : base(name)
        {
        }

        protected GroupTreeViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren, string name)
            : base(parent, lazyLoadChildren, name)
        {
        }

        public new BindableCollection<TreeViewItemViewModel> Children
        {
            get
            {
                if (HasDummyChild)
                {
                    EnsureChildren();
                }

                return base.Children;
            }
        }
    }
}
