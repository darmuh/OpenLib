using System;
using System.Collections.Generic;
using static OpenLib.Events.Events;

namespace OpenLib.InteractiveMenus
{
    public abstract class MenuItem
    {
        public abstract string Name { get; set; }
        public abstract bool ShowIfEmptyNest { get; set; }
        private string _prefix = string.Empty;
        public virtual string Prefix
        {
            get => _prefix;
            set => _prefix = value;
        }
        private string _suffix = string.Empty;
        public virtual string Suffix
        {
            get => _suffix;
            set => _suffix = value;
        }
        private bool _isActive = false; //set true when using this menu
        public virtual bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
        private bool _LoadOnSelect = true;
        public virtual bool LoadPageOnSelect
        {
            get => _LoadOnSelect;
            set => _LoadOnSelect = value;
        }

        public virtual Action OnPageLoad { get; set; }
        public abstract CustomEvent SelectionEvent { get; set; }
        public abstract List<MenuItem> NestedMenus { get; set; }
        private MenuItem _parent = null!;
        public virtual MenuItem Parent
        {
            get => _parent;
            set => _parent = value;
        }

        private Func<string> _header = () => string.Empty;
        public virtual Func<string> Header
        {
            get => _header;
            set => _header = value;
        }

        private Func<string> _footer = () => string.Empty;
        public virtual Func<string> Footer
        {
            get => _footer;
            set => _footer = value;
        }

        public virtual void SetParentMenu(MenuItem parent)
        {
            if (parent == null)
                return;

            Parent = parent;
            if (!parent.NestedMenus.Contains(this))
                parent.NestedMenus.Add(this);
        }

        public virtual void AddNestedItem(MenuItem parent)
        {
            if (parent == null)
                return;

            if (!parent.NestedMenus.Contains(this))
                parent.NestedMenus.Add(this);
        }
    }

}
