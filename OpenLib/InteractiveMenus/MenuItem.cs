using System;
using System.Collections.Generic;
using static OpenLib.Events.Events;

namespace OpenLib.InteractiveMenus;

public abstract class MenuItem<TMenu>(TMenu betterMenu) : MenuItem(betterMenu) where TMenu : BetterMenuBase
{
    public new TMenu BetterMenu => (TMenu)base.BetterMenu!;
}

public abstract class MenuItem
{
    protected BetterMenuBase? BetterMenu { get; }
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

    public virtual Action OnPageLoad { get; set; } = null!;
    public abstract CustomEvent SelectionEvent { get; set; }
    public virtual CustomEventRef<List<MenuItem>> AdjustNestedMenuList { get; set; } = new();
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

    [Obsolete("Added for compatibility with older versions, please use the constructor with the bettermenubase")]
    protected MenuItem() => Loggers.LogInfo("MenuItem created from obsolete constructor! Please use constructor with BetterMenuBase!");

    protected MenuItem(BetterMenuBase betterMenu)
    {
        if (betterMenu == null)
        {
            Loggers.ERROR("Unable to assign menu item to NULL betterMenu!");
            return;
        }

        BetterMenu = betterMenu;
        betterMenu.AllMenuItemsOfType.Add(this);
    }

    public override string ToString()
    {
        return Name;
    }

    public virtual void SetParentMenu(MenuItem parent)
    {
        //remove from existing parent's list
        Parent?.NestedMenus.RemoveAll(m => m == this);

        //allow setting parent to null
        Parent = parent;

        if (parent == null!)
            return;

        if (!parent.NestedMenus.Contains(this))
            parent.NestedMenus.Add(this);
    }

    public virtual void AddNestedItem(MenuItem child)
    {
        if (child == null!)
            return;

        child.Parent = this;
        if (!NestedMenus.Contains(child))
            NestedMenus.Add(child);
    }

    public virtual void RemoveFromParent()
    {
        if (Parent == null)
            return;

        Parent.NestedMenus.RemoveAll(m => m == this);

        Parent = null!;
    }

    public virtual void RemoveChild(MenuItem child)
    {
        if (child == null) return;

        if (child.Parent != null)
            child.Parent = null!;

        NestedMenus.Remove(child);
    }
}
